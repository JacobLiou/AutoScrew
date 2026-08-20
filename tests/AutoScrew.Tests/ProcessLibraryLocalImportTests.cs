using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Hardware;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessLibraryLocalImportTests
{
    [Fact]
    public void Allocator_SameIdentity_ReusesId()
    {
        var existing = new List<LocalPresetOrigin>
        {
            new(4, "PN-A", 0),
            new(1, "PN-B", 0),
        };

        var id = ProcessLibraryLocalIdAllocator.Resolve(existing, "PN-A", sourceIdentity: 0, preferredId: 1);
        Assert.Equal(4, id);
    }

    [Fact]
    public void Allocator_OtherPnOccupiesPreferred_AllocatesNextFree()
    {
        var existing = new List<LocalPresetOrigin>
        {
            new(1, "PN-A", 0),
            new(2, "PN-A", 1),
        };

        var id = ProcessLibraryLocalIdAllocator.Resolve(existing, "PN-B", sourceIdentity: 0, preferredId: 1);
        Assert.Equal(3, id);
    }

    [Fact]
    public void Allocator_UnownedOccupant_AllocatesNextFree()
    {
        var existing = new List<LocalPresetOrigin> { new(1, null, null) };
        var id = ProcessLibraryLocalIdAllocator.Resolve(existing, "PN-A", sourceIdentity: 0, preferredId: 1);
        Assert.Equal(2, id);
    }

    [Fact]
    public void Allocator_WhenFull_ThrowsWithoutReuse()
    {
        var existing = Enumerable.Range(1, 500)
            .Select(i => new LocalPresetOrigin(i, "OTHER", i - 1))
            .ToList();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            ProcessLibraryLocalIdAllocator.Resolve(existing, "PN-B", sourceIdentity: 0, preferredId: 1));
        Assert.Contains("已满", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ImportSlots_CrossPn_AddsNewIdWithoutOverwriting()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedProduct(dataDir, "PN-A", slots: [0, 1], screwPn: "SCREWA");
            SeedProduct(dataDir, "PN-B", slots: [0], screwPn: "SCREWB");
            var (library, parameters, _) = CreateLibrary(dataDir);

            var first = await library.ImportSlotsToLocalAsync("PN-A", [0, 1]);
            Assert.Equal(new[] { 1, 2 }, first.Items.Select(i => i.LocalId).ToArray());

            var second = await library.ImportSlotsToLocalAsync("PN-B", [0]);
            Assert.Equal(3, second.Items[0].LocalId);
            Assert.True(second.Items[0].WasNew);

            var list = await parameters.ListLocalPresetsAsync();
            Assert.Equal("PN-A", list.Single(p => p.ParameterId == 1).SourceProductPn);
            Assert.Equal("SCREWA", list.Single(p => p.ParameterId == 1).Name);
            Assert.Equal("PN-B", list.Single(p => p.ParameterId == 3).SourceProductPn);
            Assert.Equal("SCREWB", list.Single(p => p.ParameterId == 3).Name);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task ImportSlots_SamePn_OverwritesExistingLocalId()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedProduct(dataDir, "PN-A", slots: [0], screwPn: "SCREWA");
            SeedProduct(dataDir, "PN-B", slots: [0], screwPn: "SCREWB");
            var (library, parameters, _) = CreateLibrary(dataDir);

            await library.ImportSlotsToLocalAsync("PN-A", [0]);
            await library.ImportSlotsToLocalAsync("PN-B", [0]);
            var again = await library.ImportSlotsToLocalAsync("PN-A", [0]);

            Assert.Equal(1, again.Items[0].LocalId);
            Assert.False(again.Items[0].WasNew);
            var list = await parameters.ListLocalPresetsAsync();
            Assert.Equal(2, list.Count);
            Assert.Equal("PN-A", list.Single(p => p.ParameterId == 1).SourceProductPn);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task ImportSlots_PartialSamePn_DoesNotDeleteOtherSlots()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedProduct(dataDir, "PN-A", slots: [0, 1], screwPn: "SCREWA");
            var (library, parameters, _) = CreateLibrary(dataDir);

            await library.ImportSlotsToLocalAsync("PN-A", [0, 1]);
            await library.ImportSlotsToLocalAsync("PN-A", [1]);

            var list = await parameters.ListLocalPresetsAsync();
            Assert.Contains(list, p => p.ParameterId == 1 && p.SourceSlotId == 0);
            Assert.Contains(list, p => p.ParameterId == 2 && p.SourceSlotId == 1);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task ImportSequences_RemapsStepParameterIdToLocal()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedProduct(dataDir, "PN-A", slots: [0], screwPn: "SCREWA");
            SeedProduct(dataDir, "PN-B", slots: [0], screwPn: "SCREWB");
            var (library, _, sequences) = CreateLibrary(dataDir);
            await SeedLibrarySequenceAsync(dataDir, "PN-B", sequenceId: 1, libraryParameterId: 1);

            await library.ImportSlotsToLocalAsync("PN-A", [0]);
            await library.ImportSlotsToLocalAsync("PN-B", [0]);
            var imported = await library.ImportSequencesToLocalAsync("PN-B", [1]);

            Assert.Equal(1, imported.Items[0].LocalId);
            var pkg = await sequences.LoadLocalPresetAsync(1);
            Assert.Equal(2, pkg.Core.Steps[0].ParameterId);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task ImportSlots_WhenIdsFull_ThrowsAndLeavesExisting()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedProduct(dataDir, "PN-B", slots: [0], screwPn: "SCREWB");
            var (library, parameters, _) = CreateLibrary(dataDir);
            for (var id = 1; id <= 500; id++)
            {
                var template = CreateCardTemplate($"OTH{id}");
                template.ParameterId = id;
                await parameters.SaveLocalPresetWithOriginAsync(template, "OTHER", id - 1);
            }

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                library.ImportSlotsToLocalAsync("PN-B", [0]));
            Assert.Contains("无法为产品 PN-B", ex.Message, StringComparison.Ordinal);

            var kept = await parameters.LoadLocalPresetAsync(1);
            Assert.Equal("OTH1", kept.Core.Name);
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    private static TighteningParameterTemplate CreateCardTemplate(string screwPn)
    {
        var template = new TighteningParameterTemplate
        {
            Core = new TighteningParameterCore { Name = screwPn },
        };
        template.Core.Stages[0].SpeedRpm = 80;
        template.Core.Stages[0].TargetAngleDeg = 10;
        template.Core.Stages[0].ControlMode = TighteningControlMode.Angle;
        return template;
    }

    private static void SeedProduct(string dataDir, string productPn, int[] slots, string screwPn)
    {
        var productDir = Path.Combine(dataDir, "process", productPn);
        var screwsDir = Path.Combine(productDir, "screws");
        Directory.CreateDirectory(screwsDir);
        Directory.CreateDirectory(Path.Combine(productDir, "sequences"));

        var slotJson = new List<string>();
        foreach (var slot in slots)
        {
            var file = Path.Combine(screwsDir, $"{slot:D2}.txt");
            ProcessCardTxtWriter.WriteFile(file, CreateCardTemplate(screwPn), screwPn, slot);
            slotJson.Add(
                $"{{\"slotId\":{slot},\"screwPn\":\"{screwPn}\",\"fileName\":\"screws/{slot:D2}.txt\",\"displayName\":\"{screwPn}\"}}");
        }

        File.WriteAllText(
            Path.Combine(productDir, "product.json"),
            $"{{\"productPn\":\"{productPn}\",\"slots\":[{string.Join(",", slotJson)}],\"sequences\":[]}}");
    }

    private static async Task SeedLibrarySequenceAsync(
        string dataDir,
        string productPn,
        int sequenceId,
        int libraryParameterId)
    {
        var app = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDir });
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        var store = new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
        var package = new TighteningSequencePackage
        {
            SequenceId = sequenceId,
            Core = new TighteningSequenceCore
            {
                Name = $"{productPn}-SEQ",
                Steps = [new TighteningSequenceStepCore { ParameterId = libraryParameterId, Quantity = 1 }],
            },
        };
        await store.SaveSequenceAsync(productPn, package, CancellationToken.None);
    }

    private static (ProcessLibraryService Library, IControllerParameterPresetService Parameters, IControllerSequencePresetService Sequences)
        CreateLibrary(string dataDir)
    {
        var app = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDir });
        var paramStore = new LocalJsonControllerParameterPresetStore(app);
        var seqStore = new LocalJsonControllerSequencePresetStore(app);
        IControllerParameterPresetService parameters = new StoreParameterService(paramStore);
        IControllerSequencePresetService sequences = new StoreSequenceService(seqStore);
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        var processStore = new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
        var library = new ProcessLibraryService(
            processStore,
            parameters,
            sequences,
            NullLogger<ProcessLibraryService>.Instance);
        return (library, parameters, sequences);
    }

    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-lib-import-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void DeleteDir(string dir)
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
    }

    private sealed class StubMesSettings : IMesSettingsService
    {
        public MesRuntimeSettings GetSnapshot() => new() { LanShareRoot = null };

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(GetSnapshot());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }

    private sealed class StoreParameterService(LocalJsonControllerParameterPresetStore store)
        : IControllerParameterPresetService
    {
        public bool IsDeviceAvailable => false;

        public async Task<IReadOnlyList<ControllerParameterPresetSummary>> ListLocalPresetsAsync(
            CancellationToken cancellationToken = default)
        {
            var docs = await store.ListAsync(cancellationToken);
            return docs.Select(d =>
            {
                var t = d.ToTemplate();
                return new ControllerParameterPresetSummary(
                    t.ParameterId, t.Core.Name, t.ToolIndex, d.SourceProductPn, d.SourceSlotId);
            }).ToList();
        }

        public Task<TighteningParameterTemplate> LoadLocalPresetAsync(
            int parameterId,
            CancellationToken cancellationToken = default) =>
            store.LoadAsync(parameterId, cancellationToken);

        public Task SaveLocalPresetAsync(
            TighteningParameterTemplate template,
            CancellationToken cancellationToken = default) =>
            store.SaveAsync(template, cancellationToken);

        public Task SaveLocalPresetWithOriginAsync(
            TighteningParameterTemplate template,
            string sourceProductPn,
            int sourceSlotId,
            CancellationToken cancellationToken = default) =>
            store.SaveAsync(template, cancellationToken, sourceProductPn, sourceSlotId);

        public Task DeleteLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default) =>
            store.DeleteAsync(parameterId, cancellationToken);

        public Task<TighteningParameterTemplate> ImportFromFileAsync(
            string filePath,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task ExportToFileAsync(
            TighteningParameterTemplate template,
            string filePath,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<TighteningParameterTemplate> ReadFromDeviceAsync(
            int parameterId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<DefaultTorqueUnit> ReadDefaultTorqueUnitAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(DefaultTorqueUnit.NewtonMeter);

        public Task<IReadOnlyList<int>> ListDeviceParameterIdsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<int>>([]);

        public Task<TighteningParameterTemplate> ImportFromDeviceAsync(
            int parameterId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ControllerParameterBulkImportResult> ImportAllFromDeviceAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ControllerParameterBulkImportResult([], []));

        public Task WriteToDeviceAsync(
            TighteningParameterTemplate template,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteFromDeviceAsync(int parameterId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task ActivateOnDeviceAsync(
            int parameterId,
            uint screwCount = 1,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class StoreSequenceService(LocalJsonControllerSequencePresetStore store)
        : IControllerSequencePresetService
    {
        public bool IsDeviceAvailable => false;

        public async Task<IReadOnlyList<ControllerSequencePresetSummary>> ListLocalPresetsAsync(
            CancellationToken cancellationToken = default)
        {
            var docs = await store.ListAsync(cancellationToken);
            return docs.Select(d =>
            {
                var pkg = d.ToPackage();
                var bitId = pkg.Core.Steps.Count > 0 ? pkg.Core.Steps[0].BitId : 0;
                return new ControllerSequencePresetSummary(
                    pkg.SequenceId, pkg.Core.Name, pkg.Core.Steps.Count, bitId, d.SourceProductPn, d.SourceSequenceId);
            }).ToList();
        }

        public Task<TighteningSequencePackage> LoadLocalPresetAsync(
            int sequenceId,
            CancellationToken cancellationToken = default) =>
            store.LoadAsync(sequenceId, cancellationToken);

        public Task SaveLocalPresetAsync(
            TighteningSequencePackage package,
            CancellationToken cancellationToken = default) =>
            store.SaveAsync(package, cancellationToken);

        public Task SaveLocalPresetWithOriginAsync(
            TighteningSequencePackage package,
            string sourceProductPn,
            int sourceSequenceId,
            CancellationToken cancellationToken = default) =>
            store.SaveAsync(package, cancellationToken, sourceProductPn, sourceSequenceId);

        public Task DeleteLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default) =>
            store.DeleteAsync(sequenceId, cancellationToken);

        public Task<TighteningSequencePackage> ImportFromFileAsync(
            string filePath,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task ExportToFileAsync(
            TighteningSequencePackage package,
            string filePath,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<TighteningSequencePackage> ReadFromDeviceAsync(
            int sequenceId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<int>> ListDeviceSequenceIdsAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<int>>([]);

        public Task<TighteningSequencePackage> ImportFromDeviceAsync(
            int sequenceId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task WriteToDeviceAsync(
            TighteningSequencePackage package,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteFromDeviceAsync(int sequenceId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task ActivateOnDeviceAsync(int sequenceId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
