using AutoScrew.Infrastructure.Authentication;
using Xunit;

namespace AutoScrew.Tests;

public class MimsPasswordHasherTests
{
    /// <summary>ASCII "41122" → MD5 → X2 concat; verified against legacy MIMS algorithm.</summary>
    [Fact]
    public void Hash_41122_matches_legacy_mims_vector()
    {
        const string expected = "56A09A83344113CB847C81A3306809D5";
        Assert.Equal(expected, MimsPasswordHasher.Hash("41122"));
    }

    [Fact]
    public void Hash_produces_32_uppercase_hex()
    {
        var h = MimsPasswordHasher.Hash("demo");
        Assert.Equal(32, h.Length);
        Assert.Matches("^[0-9A-F]{32}$", h);
    }
}
