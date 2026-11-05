using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class ProcessLibrarySequencePnGateTests
{
    [Fact]
    public async Task SaveSequenceAsync_WhenPnMissing_Throws()
    {
        var dataDir = CreateTempDir();
        try
        {
            var store = CreateStore(dataDir);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                store.SaveSequenceAsync("PN-NONE", CreatePackage(1), CancellationToken.None));
            Assert.Contains("不存在", ex.Message);
            Assert.False(Directory.Exists(Path.Combine(dataDir, "process", "PN-NONE")));
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task SaveSequenceAsync_WhenOnlyTemplate_NoParameters_Throws()
    {
        var dataDir = CreateTempDir();
        try
        {
            var pnDir = Path.Combine(dataDir, "process", "PN-TPL");
            var templateDir = Path.Combine(pnDir, "template");
            Directory.CreateDirectory(templateDir);
            File.WriteAllText(Path.Combine(templateDir, "PN-TPL.product-template.json"), "{}");

            var store = CreateStore(dataDir);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                store.SaveSequenceAsync("PN-TPL", CreatePackage(1), CancellationToken.None));
            Assert.Contains("拧紧参数", ex.Message);
            Assert.False(Directory.Exists(Path.Combine(pnDir, "sequences")));
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task SaveSequenceAsync_WhenParametersExist_Succeeds()
    {
        var dataDir = CreateTempDir();
        try
        {
            SeedParameters(dataDir, "PN-OK");
            var store = CreateStore(dataDir);
            var info = await store.SaveSequenceAsync("PN-OK", CreatePackage(1), CancellationToken.None);
            Assert.Equal(1, info.SequenceId);
            Assert.True(File.Exists(Path.Combine(dataDir, "process", "PN-OK", "sequences", "01.json")));
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    [Fact]
    public async Task SaveProcessCard_CanCreateNewPn()
    {
        var dataDir = CreateTempDir();
        try
        {
            var store = CreateStore(dataDir);
            var cardPath = Path.Combine(dataDir, "src.txt");
            ProcessCardTxtWriter.WriteFile(cardPath, CreateCardTemplate("SCREW-A"), "SCREW-A", 0);
            var parsed = ProcessCardTxtParser.ParseFile(cardPath);

            var slot = await store.SaveProcessCardAsync("PN-NEW", cardPath, parsed, CancellationToken.None);

            Assert.Equal(0, slot.SlotId);
            Assert.True(Directory.Exists(Path.Combine(dataDir, "process", "PN-NEW", "screws")));
        }
        finally
        {
            DeleteDir(dataDir);
        }
    }

    private static ProcessLibraryStore CreateStore(string dataDir)
    {
        var app = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDir });
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        return new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
    }

    private static void SeedParameters(string dataDir, string productPn)
    {
        var productDir = Path.Combine(dataDir, "process", productPn);
        var screwsDir = Path.Combine(productDir, "screws");
        Directory.CreateDirectory(screwsDir);
        var file = Path.Combine(screwsDir, "00.txt");
        ProcessCardTxtWriter.WriteFile(file, CreateCardTemplate("SCREW-A"), "SCREW-A", 0);
        File.WriteAllText(
            Path.Combine(productDir, "product.json"),
            $$"""{"productPn":"{{productPn}}","slots":[{"slotId":0,"screwPn":"SCREW-A","fileName":"screws/00.txt","displayName":"SCREW-A"}],"sequences":[]}""");
    }

    private static TighteningSequencePackage CreatePackage(int sequenceId) =>
        new()
        {
            SequenceId = sequenceId,
            Core = new TighteningSequenceCore
            {
                Name = "SEQ",
                Steps = [new TighteningSequenceStepCore { ParameterId = 1, Quantity = 1 }],
            },
        };

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

    private static string CreateTempDir()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-seq-gate-" + Guid.NewGuid().ToString("N"));
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
        public MesRuntimeSettings GetSnapshot() => new() { MesMode = MesProviderMode.Mock, UseMockMes = true };

        public Task<MesRuntimeSettings> LoadAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(GetSnapshot());

        public Task SaveAsync(MesRuntimeSettings settings, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public void ApplySnapshot(MesRuntimeSettings settings)
        {
        }
    }
}
