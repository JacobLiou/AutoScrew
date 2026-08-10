using AutoScrew.Common.Host;
using Xunit;

namespace AutoScrew.Tests;

public sealed class HostIdentityTests
{
    [Theory]
    [InlineData("AABBCCDDEEFF", "AA-BB-CC-DD-EE-FF")]
    [InlineData("aa:bb:cc:dd:ee:ff", "AA-BB-CC-DD-EE-FF")]
    [InlineData("aa-bb-cc-dd-ee-ff", "AA-BB-CC-DD-EE-FF")]
    [InlineData("", HostIdentity.UnknownHostFolder)]
    [InlineData("short", HostIdentity.UnknownHostFolder)]
    [InlineData(null, HostIdentity.UnknownHostFolder)]
    public void NormalizeMacFolderName_FormatsOrFallsBack(string? input, string expected) =>
        Assert.Equal(expected, HostIdentity.NormalizeMacFolderName(input));

    [Fact]
    public void LanArchivePath_UsesMacThenSerial()
    {
        var lanRoot = @"\\server\AutoScrew";
        var mac = HostIdentity.NormalizeMacFolderName("11:22:33:44:55:66");
        var sn = "SN-001";
        var dest = Path.Combine(lanRoot, mac, sn);
        Assert.Equal(Path.Combine(@"\\server\AutoScrew", "11-22-33-44-55-66", "SN-001"), dest);
    }
}
