using System.Security.Cryptography;
using AutoScrew.Infrastructure.Authentication;

if (args.Length < 1)
{
    Console.Error.WriteLine("用法: dotnet run --project tools/EncryptMimsConnectionString -- <明文连接串文件路径> [LegacyDpapiScope]");
    Console.Error.WriteLine("输出一行密文，粘贴到 appsettings 的 Authentication:Mims:ConnectionStringDpapiBase64。默认生成可跨机器部署的新格式。第二参数仅为兼容旧命令行，不影响新格式。");
    return 1;
}

var scope = args.Length >= 2 && string.Equals(args[1], "CurrentUser", StringComparison.OrdinalIgnoreCase)
    ? DataProtectionScope.CurrentUser
    : DataProtectionScope.LocalMachine;

var path = args[0];
if (!File.Exists(path))
{
    Console.Error.WriteLine($"文件不存在: {path}");
    return 2;
}

var plain = File.ReadAllText(path).Trim();
if (plain.Length == 0)
{
    Console.Error.WriteLine("文件为空。");
    return 3;
}

Console.WriteLine(MimsConnectionStringDpapi.ProtectToBase64(plain, scope));
return 0;
