using System.IO;
using System.Text.Json;

namespace AutoScrew.Hmi.Services;

/// <summary>登录界面偏好（仅记住用户名，不存储密码；最长 7 天）。</summary>
internal static class LoginUiPreferences
{
    private static readonly TimeSpan MaxRememberDuration = TimeSpan.FromDays(7);

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
            if (string.IsNullOrWhiteSpace(dto?.RememberedUserName))
                return null;

            var savedAt = dto.SavedAtUtc ?? File.GetLastWriteTimeUtc(PrefsPath);
            if (DateTime.UtcNow - savedAt.ToUniversalTime() > MaxRememberDuration)
            {
                ClearRememberedUserName();
                return null;
            }

            return dto.RememberedUserName.Trim();
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
            var dto = new PrefsDto
            {
                RememberedUserName = string.IsNullOrWhiteSpace(userName) ? null : userName.Trim(),
                SavedAtUtc = string.IsNullOrWhiteSpace(userName) ? null : DateTime.UtcNow,
            };
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

        public DateTime? SavedAtUtc { get; set; }
    }
}
