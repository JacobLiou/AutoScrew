using System.Security.Cryptography;
using System.Text;

// 必须与 src/AutoScrew.Infrastructure/Authentication/MimsConnectionStringDpapi.cs 中 Entropy 一致。
var entropy = Encoding.UTF8.GetBytes("AutoScrew.MimsConnection.v1");

if (args.Length < 1)
{
    Console.Error.WriteLine("用法: dotnet run --project tools/EncryptMimsConnectionString -- <明文连接串文件路径> [LocalMachine|CurrentUser]");
    Console.Error.WriteLine("输出一行 Base64，粘贴到 appsettings 的 Authentication:Mims:ConnectionStringDpapiBase64。");
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

var cipher = ProtectedData.Protect(Encoding.UTF8.GetBytes(plain), entropy, scope);
Console.WriteLine(Convert.ToBase64String(cipher));
return 0;
