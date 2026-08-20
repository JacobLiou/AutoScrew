using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Infrastructure.Lan;
using AutoScrew.Infrastructure.ProcessLibrary;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UDL.Delta.IemdSd;
using UDL.Delta.IemdSd.Protocol;
using Xunit;

namespace AutoScrew.Tests;

public sealed class SequenceExcelParserTests
{
    [Fact]
    public void Parse_ValidSheet_MapsStepsAndParameterCode()
    {
        using var stream = CreateWorkbook(ws =>
        {
            WriteHeaders(ws);
            ws.Cell(2, 1).Value = 1;
            ws.Cell(2, 2).Value = "pump1锁附";
            ws.Cell(2, 3).Value = "1830331949";
            ws.Cell(2, 4).Value = "1830331949-00";
            ws.Cell(2, 5).Value = 4;
            ws.Cell(2, 6).Value = 1;
            ws.Cell(2, 7).Value = "预紧";

            ws.Cell(3, 1).Value = 2;
            ws.Cell(3, 2).Value = "pump1锁附";
            ws.Cell(3, 3).Value = "1830331949";
            ws.Cell(3, 4).Value = "1830331949-01";
            ws.Cell(3, 5).Value = 4;
            ws.Cell(3, 6).Value = 1;
        });

        var result = SequenceExcelParser.Parse(stream);
        Assert.True(result.IsSuccess, string.Join("; ", result.Errors));
        Assert.Equal(2, result.Steps.Count);
        Assert.Equal(0, result.Steps[0].SlotId);
        Assert.Equal(1, result.Steps[1].SlotId);
        Assert.Equal(4, result.Steps[0].Quantity);
        Assert.Equal("预紧", result.Steps[0].Remark);
    }

    [Fact]
    public void Parse_ScrewPnMismatch_ReportsRowError()
    {
        using var stream = CreateWorkbook(ws =>
        {
            WriteHeaders(ws);
            ws.Cell(2, 1).Value = 1;
            ws.Cell(2, 2).Value = "loc";
            ws.Cell(2, 3).Value = "999";
            ws.Cell(2, 4).Value = "1830331949-00";
            ws.Cell(2, 5).Value = 1;
            ws.Cell(2, 6).Value = 1;
        });

        var result = SequenceExcelParser.Parse(stream);
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e.Contains("不一致", StringComparison.Ordinal));
    }

    [Fact]
    public async Task UploadSequenceExcel_MissingSlot_ThrowsWithRowInfo()
    {
        var dataDir = Path.Combine(Path.GetTempPath(), "autoscrew-seq-excel-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dataDir);
        try
        {
            var productPn = "PN-TEST";
            var productDir = Path.Combine(dataDir, "process", productPn);
            Directory.CreateDirectory(Path.Combine(productDir, "screws"));
            await File.WriteAllTextAsync(
                Path.Combine(productDir, "product.json"),
                """{"productPn":"PN-TEST","slots":[{"slotId":0,"screwPn":"1830331949","fileName":"screws/00.txt","displayName":"1830331949"}],"sequences":[]}""");

            var xlsx = Path.Combine(dataDir, "seq.xlsx");
            SaveMinimalSheet(xlsx, parameterCode: "1830331949-99");

            var svc = CreateService(dataDir);
            var ex = await Assert.ThrowsAsync<InvalidDataException>(() =>
                svc.UploadSequenceExcelAsync(productPn, xlsx, sequenceId: 1));
            Assert.Contains("99", ex.Message, StringComparison.Ordinal);
            Assert.Contains("第 2 行", ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(dataDir))
                Directory.Delete(dataDir, recursive: true);
        }
    }

    [Fact]
    public async Task UploadSequenceExcel_ExistingSlot_SavesPackage()
    {
        var dataDir = Path.Combine(Path.GetTempPath(), "autoscrew-seq-excel-ok-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dataDir);
        try
        {
            var productPn = "PN-OK";
            var productDir = Path.Combine(dataDir, "process", productPn);
            Directory.CreateDirectory(Path.Combine(productDir, "screws"));
            await File.WriteAllTextAsync(
                Path.Combine(productDir, "product.json"),
                """{"productPn":"PN-OK","slots":[{"slotId":0,"screwPn":"1830331949","fileName":"screws/00.txt","displayName":"1830331949"},{"slotId":1,"screwPn":"1830331949","fileName":"screws/01.txt","displayName":"1830331949"}],"sequences":[]}""");

            var xlsx = Path.Combine(dataDir, "my-seq.xlsx");
            using (var wb = new XLWorkbook())
            {
                var ws = wb.AddWorksheet("Sheet1");
                WriteHeaders(ws);
                ws.Cell(2, 1).Value = 1;
                ws.Cell(2, 2).Value = "pump1";
                ws.Cell(2, 3).Value = "1830331949";
                ws.Cell(2, 4).Value = "1830331949-00";
                ws.Cell(2, 5).Value = 4;
                ws.Cell(2, 6).Value = 1;
                ws.Cell(3, 1).Value = 2;
                ws.Cell(3, 2).Value = "pump1";
                ws.Cell(3, 3).Value = "1830331949";
                ws.Cell(3, 4).Value = "1830331949-01";
                ws.Cell(3, 5).Value = 4;
                ws.Cell(3, 6).Value = 1;
                wb.SaveAs(xlsx);
            }

            var svc = CreateService(dataDir);
            var info = await svc.UploadSequenceExcelAsync(productPn, xlsx, sequenceId: 1);
            Assert.Equal(1, info.SequenceId);
            Assert.Equal("my-seq", info.DisplayName);

            var pkg = await svc.LoadProductSequenceAsync(productPn, 1);
            Assert.Equal(2, pkg.Core.Steps.Count);
            Assert.Equal(1, pkg.Core.Steps[0].ParameterId);
            Assert.Equal(2, pkg.Core.Steps[1].ParameterId);
            Assert.Equal(4, pkg.Core.Steps[0].Quantity);
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

    private static void SaveMinimalSheet(string path, string parameterCode)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Sheet1");
        WriteHeaders(ws);
        ws.Cell(2, 1).Value = 1;
        ws.Cell(2, 2).Value = "loc";
        ws.Cell(2, 3).Value = "1830331949";
        ws.Cell(2, 4).Value = parameterCode;
        ws.Cell(2, 5).Value = 1;
        ws.Cell(2, 6).Value = 1;
        wb.SaveAs(path);
    }

    private static void WriteHeaders(IXLWorksheet ws)
    {
        ws.Cell(1, 1).Value = "拧紧顺序";
        ws.Cell(1, 2).Value = "位置";
        ws.Cell(1, 3).Value = "螺钉PN";
        ws.Cell(1, 4).Value = "拧紧参数";
        ws.Cell(1, 5).Value = "数量";
        ws.Cell(1, 6).Value = "批头";
        ws.Cell(1, 7).Value = "备注（可以不用填写）";
    }

    private static MemoryStream CreateWorkbook(Action<IXLWorksheet> fill)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Sheet1");
        fill(ws);
        var ms = new MemoryStream();
        wb.SaveAs(ms);
        ms.Position = 0;
        return ms;
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
