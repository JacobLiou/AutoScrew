using UDL.Delta.IemdSd.Internal;

namespace UDL.Delta.IemdSd.Tests;

public class ParameterBlockValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(501)]
    public void ValidateParameterId_RejectsOutOfRange(int id)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ParameterBlockReader.ValidateParameterId(id));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(500)]
    public void ValidateParameterId_AcceptsInRange(int id)
    {
        var ex = Record.Exception(() => ParameterBlockReader.ValidateParameterId(id));
        Assert.Null(ex);
    }
}
