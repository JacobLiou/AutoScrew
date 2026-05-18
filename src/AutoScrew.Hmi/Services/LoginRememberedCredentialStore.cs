using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AutoScrew.Hmi.Services;

/// <summary>
/// 在本机当前用户目录下保存「记住密码」（DPAPI 加密口令）。
/// 主存为 <c>%LocalAppData%\AutoScrew\remembered_login.json</c>；部分宿主下
/// <see cref="IsolatedStorageFile"/> 写入不可靠，故仅作兼容读取并在成功读出后迁移到主存。
/// </summary>
internal static class LoginRememberedCredentialStore
{
    private const string FileName = "remembered_login.json";
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("AutoScrew.LoginRemembered.v1");
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    private static string PrimaryFilePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", FileName);

    public static bool TryLoad(out string userName, out string? plainPassword)
    {
        userName = "";
        plainPassword = null;
        if (TryDeserializeFromPath(PrimaryFilePath, out userName, out plainPassword))
            return true;

        if (!TryDeserializeFromIsolatedStorage(out userName, out plainPassword) || string.IsNullOrWhiteSpace(plainPassword))
            return false;

        Save(userName, plainPassword);
        return true;
    }

    public static void Save(string userName, string password)
    {
        var json = BuildJson(userName, password);
        try
        {
            var path = PrimaryFilePath;
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(path, json, Utf8NoBom);
        }
        catch
        {
            // ignore
        }

        TryWriteIsolatedStorage(json);
    }

    public static void Clear()
    {
        try
        {
            if (File.Exists(PrimaryFilePath))
                File.Delete(PrimaryFilePath);
        }
        catch
        {
            // ignore
        }

        try
        {
            using var store = IsolatedStorageFile.GetUserStoreForAssembly();
            if (store.FileExists(FileName))
                store.DeleteFile(FileName);
        }
        catch
        {
            // ignore
        }
    }

    private static string BuildJson(string userName, string password)
    {
        var plain = Encoding.UTF8.GetBytes(password ?? "");
        var cipher = ProtectedData.Protect(plain, Entropy, DataProtectionScope.CurrentUser);
        var dto = new RememberDto
        {
            UserName = userName.Trim(),
            PasswordProtectedBase64 = Convert.ToBase64String(cipher),
        };
        return JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = false });
    }

    private static bool TryDeserializeFromPath(string path, out string userName, out string? plainPassword)
    {
        userName = "";
        plainPassword = null;
        try
        {
            if (!File.Exists(path))
                return false;
            var json = File.ReadAllText(path, Utf8NoBom);
            return TryDeserializeJson(json, out userName, out plainPassword);
        }
        catch
        {
            return false;
        }
    }

    private static bool TryDeserializeFromIsolatedStorage(out string userName, out string? plainPassword)
    {
        userName = "";
        plainPassword = null;
        try
        {
            using var store = IsolatedStorageFile.GetUserStoreForAssembly();
            if (!store.FileExists(FileName))
                return false;

            using var stream = store.OpenFile(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var json = reader.ReadToEnd();
            return TryDeserializeJson(json, out userName, out plainPassword);
        }
        catch
        {
            return false;
        }
    }

    private static bool TryDeserializeJson(string json, out string userName, out string? plainPassword)
    {
        userName = "";
        plainPassword = null;
        try
        {
            var dto = JsonSerializer.Deserialize<RememberDto>(json);
            if (dto is null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.PasswordProtectedBase64))
                return false;

            userName = dto.UserName.Trim();
            var cipher = Convert.FromBase64String(dto.PasswordProtectedBase64.Trim());
            var plainBytes = ProtectedData.Unprotect(cipher, Entropy, DataProtectionScope.CurrentUser);
            plainPassword = Encoding.UTF8.GetString(plainBytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void TryWriteIsolatedStorage(string json)
    {
        try
        {
            using var store = IsolatedStorageFile.GetUserStoreForAssembly();
            using (var stream = store.OpenFile(FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream, Utf8NoBom))
                writer.Write(json);
        }
        catch
        {
            // ignore
        }
    }

    private sealed class RememberDto
    {
        public string UserName { get; set; } = "";
        public string PasswordProtectedBase64 { get; set; } = "";
    }
}
