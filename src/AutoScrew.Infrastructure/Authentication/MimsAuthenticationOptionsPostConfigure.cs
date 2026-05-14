using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoScrew.Infrastructure.Authentication;

public sealed class MimsAuthenticationOptionsPostConfigure(ILogger<MimsAuthenticationOptionsPostConfigure> logger)
    : IPostConfigureOptions<MimsAuthenticationOptions>
{
    public void PostConfigure(string? name, MimsAuthenticationOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionStringDpapiBase64) && string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            try
            {
                var scope = ParseDpapiScope(options.DpapiScope);
                var plain = MimsConnectionStringDpapi.UnprotectFromBase64(options.ConnectionStringDpapiBase64, scope);
                options.ConnectionString = plain.Trim();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "无法解密 Authentication:Mims:ConnectionStringDpapiBase64（DPAPI 作用域或密文是否与当前 Windows 用户/本机一致？）。");
                options.ConnectionString = "";
            }
        }
        else if (!string.IsNullOrWhiteSpace(options.ConnectionStringDpapiBase64) && !string.IsNullOrWhiteSpace(options.ConnectionString))
            logger.LogInformation("MIMS: 已配置明文 ConnectionString，忽略 ConnectionStringDpapiBase64。");

        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            options.ConnectionString = MimsConnectionStringNormalizer.EnsureCharsetGbkIfMissing(options.ConnectionString.Trim());
    }

    private static DataProtectionScope ParseDpapiScope(string? value)
    {
        if (string.Equals(value, "CurrentUser", StringComparison.OrdinalIgnoreCase))
            return DataProtectionScope.CurrentUser;

        return DataProtectionScope.LocalMachine;
    }
}
