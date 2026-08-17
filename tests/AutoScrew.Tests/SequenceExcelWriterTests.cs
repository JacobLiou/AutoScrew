using AutoScrew.Application.Abstractions;
using AutoScrew.Infrastructure.ProcessLibrary;
using Xunit;

namespace AutoScrew.Tests;

public sealed class SequenceExcelWriterTests
{
    [Fact]
    public void Write_RoundTrip_ParseSucceeds()
    {
        var steps = new List<SequenceExcelStepRow>
        {
            new(
                ExcelRowNumber: 2,
                Order: 1,
                Location: "步骤1",
                ScrewPn: "1830331949",
                ParameterCode: "1830331949-00",
                ScrewPnFromCode: "1830331949",
                SlotId: 0,
                Quantity: 4,
                BitId: 1,
                Remark: "预紧"),
            new(
                ExcelRowNumber: 3,
                Order: 2,
                Location: "步骤2",
                ScrewPn: "1830331949",
                ParameterCode: "1830331949-01",
                ScrewPnFromCode: "1830331949",
                SlotId: 1,
                Quantity: 2,
                BitId: 0,
                Remark: null),
        };

        using var stream = new MemoryStream();
        SequenceExcelWriter.Write(stream, steps);
        stream.Position = 0;

        var parsed = SequenceExcelParser.Parse(stream);
        Assert.True(parsed.IsSuccess, string.Join("; ", parsed.Errors));
        Assert.Equal(2, parsed.Steps.Count);
        Assert.Equal(0, parsed.Steps[0].SlotId);
        Assert.Equal(1, parsed.Steps[1].SlotId);
        Assert.Equal(4, parsed.Steps[0].Quantity);
        Assert.Equal(1, parsed.Steps[0].BitId);
        Assert.Equal("预紧", parsed.Steps[0].Remark);
        Assert.Equal("步骤1", parsed.Steps[0].Location);
        Assert.Equal("1830331949-00", parsed.Steps[0].ParameterCode);
    }

    [Fact]
    public void Write_EmptySteps_Throws()
    {
        using var stream = new MemoryStream();
        Assert.Throws<InvalidDataException>(() => SequenceExcelWriter.Write(stream, []));
    }
}
