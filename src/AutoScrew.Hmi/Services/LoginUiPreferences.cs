using System.IO;
using System.Text.Json;

namespace AutoScrew.Hmi.Services;

/// <summary>登录界面偏好（仅记住用户名，不存储密码）。</summary>
internal static class LoginUiPreferences
{
    private static string PrefsPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AutoScrew", "login-ui.json");

    public static string? TryGetRememberedUserName()
    {
        try
        {
            if (!File.Exists(PrefsPath))
                return null;
            var json = File.ReadAllText(PrefsPath);
            var dto = JsonSerializer.Deserialize<PrefsDto>(json);
            return string.IsNullOrWhiteSpace(dto?.RememberedUserName) ? null : dto.RememberedUserName.Trim();
        }
        catch
        {
            return null;
        }
    }

    public static void SetRememberedUserName(string? userName)
    {
        try
        {
            var dir = Path.GetDirectoryName(PrefsPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            var dto = new PrefsDto { RememberedUserName = string.IsNullOrWhiteSpace(userName) ? null : userName.Trim() };
            File.WriteAllText(PrefsPath, JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch
        {
            // ignore
        }
    }

    public static void ClearRememberedUserName() => SetRememberedUserName(null);

    private sealed class PrefsDto
    {
        public string? RememberedUserName { get; set; }
    }
}
