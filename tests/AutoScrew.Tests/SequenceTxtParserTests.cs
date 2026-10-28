using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class SequenceTxtParserTests
{
    [Fact]
    public void Parse_TwoStepsOutOfOrder_SortsAndMapsSlots()
    {
        const string text = """
            # head
            步骤
            拧紧顺序：2
            位置：b
            螺钉PN：1830331949
            拧紧参数：1830331949-01
            数量：4
            批头：1

            步骤
            拧紧顺序：1                 <从 1 起>
            位置：pump1锁附              <人读>
            螺钉PN：1830331949
            拧紧参数：1830331949-00
            数量：4                      <1–999999>
            批头：1
            备注：预紧                   <可省略>
            // tail
            """;

        var result = SequenceTxtParser.Parse(text);
        Assert.True(result.IsSuccess, string.Join("; ", result.Errors));
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal(1, result.Steps[0].Order);
        Assert.Equal(0, result.Steps[0].SlotId);
        Assert.Equal(4, result.Steps[0].Quantity);
        Assert.Equal("预紧", result.Steps[0].Remark);
        Assert.Equal("pump1锁附", result.Steps[0].Location);
        Assert.Equal(2, result.Steps[1].Order);
        Assert.Equal(1, result.Steps[1].SlotId);
        Assert.Null(result.Steps[1].Remark);
        Assert.True(result.Steps[0].ExcelRowNumber > result.Steps[1].ExcelRowNumber);
    }

    [Fact]
    public void Parse_HalfwidthColonAndOmittedScrewPn_UsesCodePrefix()
    {
        const string text = """
            步骤
            拧紧顺序:1
            拧紧参数:ALLTPL-00
            数量:2
            批头:0
            """;

        var result = SequenceTxtParser.Parse(text);
        Assert.True(result.IsSuccess, string.Join("; ", result.Errors));
        Assert.Equal("ALLTPL", result.Steps[0].ScrewPn);
        Assert.Equal(0, result.Steps[0].SlotId);
        Assert.Equal(0, result.Steps[0].BitId);
        Assert.Equal(string.Empty, result.Steps[0].Location);
    }

    [Fact]
    public void Parse_ScrewPnMismatch_ReportsStepLine()
    {
        const string text = """
            # comment
            步骤
            拧紧顺序：1
            螺钉PN：999
            拧紧参数：1830331949-00
            数量：1
            批头：1
            """;

        var result = SequenceTxtParser.Parse(text);
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("不一致", StringComparison.Ordinal));
        Assert.Contains(result.Errors, e => e.StartsWith("第 2 行", StringComparison.Ordinal));
    }

    [Fact]
    public void Parse_MissingQuantity_ReportsRequiredKey()
    {
        const string text = """
            步骤
            拧紧顺序：1
            拧紧参数：1830331949-00
            批头：1
            """;

        var result = SequenceTxtParser.Parse(text);
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("缺少「数量」", StringComparison.Ordinal));
    }

    [Fact]
    public void Parse_SampleTemplate_MapsBothSlots()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Fixtures", "AllTemplateSequence.txt");
        Assert.True(File.Exists(path), $"Missing fixture: {path}");

        var result = SequenceTxtParser.ParseFile(path);
        Assert.True(result.IsSuccess, string.Join("; ", result.Errors));
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal(0, result.Steps[0].SlotId);
        Assert.Equal(1, result.Steps[1].SlotId);
        Assert.Equal("ALLTPL", result.Steps[0].ScrewPn);
        Assert.Equal(4, result.Steps[0].Quantity);
        Assert.Equal("预紧", result.Steps[0].Remark);
        Assert.Null(result.Steps[1].Remark);
        Assert.Equal("pump1锁附", result.Steps[0].Location);
    }

    [Fact]
    public void Write_RoundTrip_PreservesStepFields()
    {
        var original = new SequenceExcelStepRow(
            ExcelRowNumber: 2,
            Order: 1,
            Location: "pump1锁附",
            ScrewPn: "ALLTPL",
            ParameterCode: "ALLTPL-00",
            ScrewPnFromCode: "ALLTPL",
            SlotId: 0,
            Quantity: 4,
            BitId: 1,
            Remark: "预紧");

        var parsed = SequenceTxtParser.Parse(SequenceTxtWriter.Format([original]));
        Assert.True(parsed.IsSuccess, string.Join("; ", parsed.Errors));
        var step = Assert.Single(parsed.Steps);
        Assert.Equal(original.Order, step.Order);
        Assert.Equal(original.Location, step.Location);
        Assert.Equal(original.ScrewPn, step.ScrewPn);
        Assert.Equal(original.ParameterCode, step.ParameterCode);
        Assert.Equal(original.SlotId, step.SlotId);
        Assert.Equal(original.Quantity, step.Quantity);
        Assert.Equal(original.BitId, step.BitId);
        Assert.Equal(original.Remark, step.Remark);
    }

    [Fact]
    public async Task UploadSequenceTxt_MissingSlot_ThrowsWithLineInfo()
    {
        var dataDir = Path.Combine(Path.GetTempPath(), "autoscrew-seq-txt-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dataDir);
        try
        {
            var productPn = "PN-TEST";
            var productDir = Path.Combine(dataDir, "process", productPn);
            Directory.CreateDirectory(Path.Combine(productDir, "screws"));
            await File.WriteAllTextAsync(
                Path.Combine(productDir, "product.json"),
                """{"productPn":"PN-TEST","slots":[{"slotId":0,"screwPn":"1830331949","fileName":"screws/00.txt","displayName":"1830331949"}],"sequences":[]}""");

            var txt = Path.Combine(dataDir, "seq.txt");
            await File.WriteAllTextAsync(
                txt,
                """
                步骤
                拧紧顺序：1
                螺钉PN：1830331949
                拧紧参数：1830331949-99
                数量：1
                批头：1
                """);

            var svc = CreateService(dataDir);
            var ex = await Assert.ThrowsAsync<InvalidDataException>(() =>
                svc.UploadSequenceTxtAsync(productPn, txt, sequenceId: 1));
            Assert.Contains("99", ex.Message, StringComparison.Ordinal);
            Assert.Contains("第 1 行", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(dataDir))
                Directory.Delete(dataDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadSequenceTxt_ExistingSlot_SavesPackage()
    {
        var dataDir = Path.Combine(Path.GetTempPath(), "autoscrew-seq-txt-ok-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dataDir);
        try
        {
            var productPn = "PN-OK";
            var productDir = Path.Combine(dataDir, "process", productPn);
            Directory.CreateDirectory(Path.Combine(productDir, "screws"));
            await File.WriteAllTextAsync(
                Path.Combine(productDir, "product.json"),
                """{"productPn":"PN-OK","slots":[{"slotId":0,"screwPn":"1830331949","fileName":"screws/00.txt","displayName":"1830331949"},{"slotId":1,"screwPn":"1830331949","fileName":"screws/01.txt","displayName":"1830331949"}],"sequences":[]}""");

            var txt = Path.Combine(dataDir, "my-seq.txt");
            await File.WriteAllTextAsync(
                txt,
                """
                步骤
                拧紧顺序：1
                位置：pump1
                螺钉PN：1830331949
                拧紧参数：1830331949-00
                数量：4
                批头：1
                步骤
                拧紧顺序：2
                位置：pump1
                螺钉PN：1830331949
                拧紧参数：1830331949-01
                数量：4
                批头：1
                """);

            var svc = CreateService(dataDir);
            var info = await svc.UploadSequenceTxtAsync(productPn, txt, sequenceId: 1);
            Assert.Equal(1, info.SequenceId);
            Assert.Equal("my-seq", info.DisplayName);

            var pkg = await svc.LoadProductSequenceAsync(productPn, 1);
            Assert.Equal(2, pkg.Core.Steps.Count);
            Assert.Equal(1, pkg.Core.Steps[0].ParameterId);
            Assert.Equal(2, pkg.Core.Steps[1].ParameterId);
            Assert.Equal(4, pkg.Core.Steps[0].Quantity);
            Assert.Equal(1, pkg.Core.Steps[0].BitId);
        }
        finally
        {
            if (Directory.Exists(dataDir))
                Directory.Delete(dataDir, recursive: true);
        }
    }

    private static ProcessLibraryService CreateService(string dataDir)
    {
        var app = Options.Create(new AutoScrewAppOptions { DataDirectory = dataDir });
        var mes = new StubMesSettings();
        var lan = new LanShareAccess(mes, app, NullLogger<LanShareAccess>.Instance);
        var store = new ProcessLibraryStore(lan, app, NullLogger<ProcessLibraryStore>.Instance);
        return new ProcessLibraryService(
            store,
            new NoOpParameterService(),
            new NoOpSequenceService(),
            NullLogger<ProcessLibraryService>.Instance);
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

    private sealed class NoOpParameterService : IControllerParameterPresetService
    {
        public bool IsDeviceAvailable => false;

        public Task<IReadOnlyList<ControllerParameterPresetSummary>> ListLocalPresetsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ControllerParameterPresetSummary>>([]);

        public Task<TighteningParameterTemplate> LoadLocalPresetAsync(
            int parameterId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task SaveLocalPresetAsync(
            TighteningParameterTemplate template,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task SaveLocalPresetWithOriginAsync(
            TighteningParameterTemplate template,
            string sourceProductPn,
            int sourceSlotId,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteLocalPresetAsync(int parameterId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

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

        public Task<IReadOnlyList<ControllerDeviceParameterEntry>> ListDeviceParameterEntriesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ControllerDeviceParameterEntry>>([]);

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

    private sealed class NoOpSequenceService : IControllerSequencePresetService
    {
        public bool IsDeviceAvailable => false;

        public Task<IReadOnlyList<ControllerSequencePresetSummary>> ListLocalPresetsAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ControllerSequencePresetSummary>>([]);

        public Task<TighteningSequencePackage> LoadLocalPresetAsync(
            int sequenceId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task SaveLocalPresetAsync(
            TighteningSequencePackage package,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task SaveLocalPresetWithOriginAsync(
            TighteningSequencePackage package,
            string sourceProductPn,
            int sourceSequenceId,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteLocalPresetAsync(int sequenceId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

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

        public Task<IReadOnlyList<ControllerDeviceSequenceEntry>> ListDeviceSequenceEntriesAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<ControllerDeviceSequenceEntry>>([]);

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
