using System.Security.Cryptography;
using AutoScrew.Infrastructure.Authentication;

if (args.Length >= 1 && string.Equals(args[0], "--migrate-dpapi", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 2)
    {
        Console.Error.WriteLine("用法: dotnet run --project tools/EncryptMimsConnectionString -- --migrate-dpapi <旧DPAPI密文> [LocalMachine|CurrentUser]");
        Console.Error.WriteLine("将旧版 Windows DPAPI 密文转为可跨机部署的 aes256: 格式。");
        return 1;
    }

    var scope = args.Length >= 3 && string.Equals(args[2], "CurrentUser", StringComparison.OrdinalIgnoreCase)
        ? DataProtectionScope.CurrentUser
        : DataProtectionScope.LocalMachine;

    try
    {
        var plain = MimsConnectionStringDpapi.UnprotectFromBase64(args[1], scope);
        Console.WriteLine(MimsConnectionStringDpapi.ProtectToBase64(plain, scope));
        Console.Error.WriteLine("已生成 aes256: 密文（可部署到任意 Windows PC）。请勿提交明文到版本库。");
        return 0;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"DPAPI 解密失败: {ex.Message}");
        Console.Error.WriteLine("请在生成旧密文的同一台电脑上运行此命令，或改用明文 conn.txt 重新加密。");
        return 4;
    }
}

if (args.Length < 1)
{
    Console.Error.WriteLine("用法: dotnet run --project tools/EncryptMimsConnectionString -- <明文连接串文件路径> [LegacyDpapiScope]");
    Console.Error.WriteLine("      dotnet run --project tools/EncryptMimsConnectionString -- --migrate-dpapi <旧DPAPI密文> [LocalMachine|CurrentUser]");
    Console.Error.WriteLine("输出一行密文，粘贴到 appsettings 的 Authentication:Mims:ConnectionStringDpapiBase64。默认生成可跨机器部署的新格式。");
    return 1;
}

var scope2 = args.Length >= 2 && string.Equals(args[1], "CurrentUser", StringComparison.OrdinalIgnoreCase)
    ? DataProtectionScope.CurrentUser
    : DataProtectionScope.LocalMachine;

var path = args[0];
if (!File.Exists(path))
{
    Console.Error.WriteLine($"文件不存在: {path}");
    return 2;
}

var plain2 = File.ReadAllText(path).Trim();
if (plain2.Length == 0)
{
    Console.Error.WriteLine("文件为空。");
    return 3;
}

Console.WriteLine(MimsConnectionStringDpapi.ProtectToBase64(plain2, scope2));
return 0;
