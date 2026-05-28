using System.Security.Cryptography;
using System.Text;
using AutoScrew.Infrastructure.Authentication;
using Xunit;

namespace AutoScrew.Tests;

public class MimsConnectionStringDpapiTests
{
    [Fact]
    public void ProtectToBase64_uses_portable_prefix_and_roundtrips()
    {
        const string plain = "Server=127.0.0.1;Database=mims;User ID=u;Password=p;";

        var cipher = MimsConnectionStringDpapi.ProtectToBase64(plain, DataProtectionScope.LocalMachine);

        Assert.StartsWith("aes256:", cipher, StringComparison.Ordinal);
        Assert.Equal(plain, MimsConnectionStringDpapi.UnprotectFromBase64(cipher, DataProtectionScope.LocalMachine));
    }

    [Fact]
    public void UnprotectFromBase64_still_supports_legacy_dpapi_payload()
    {
        const string plain = "Server=10.0.0.5;Database=mims;User ID=legacy;Password=legacy;";
        var entropy = Encoding.UTF8.GetBytes("AutoScrew.MimsConnection.v1");
        var cipher = ProtectedData.Protect(Encoding.UTF8.GetBytes(plain), entropy, DataProtectionScope.CurrentUser);
        var base64 = Convert.ToBase64String(cipher);

        var roundtrip = MimsConnectionStringDpapi.UnprotectFromBase64(base64, DataProtectionScope.CurrentUser);

        Assert.Equal(plain, roundtrip);
    }
}