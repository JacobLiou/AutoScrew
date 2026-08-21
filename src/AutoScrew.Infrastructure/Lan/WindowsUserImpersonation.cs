using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace AutoScrew.Infrastructure.Lan;

/// <summary>
/// PRED-TESTING 等 Windows 账号：LogonUser 校验 + 模拟身份执行本地 ACL 受限文件操作。
/// </summary>
internal static class WindowsUserImpersonation
{
    private const int LogonInteractive = 2;
    private const int LogonNetwork = 3;
    private const int LogonNewCredentials = 9;
    private const int ProviderDefault = 0;

    private const uint CreateUnicodeEnvironment = 0x00000400;
    private const uint CreateDefaultErrorMode = 0x04000000;

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool LogonUser(
        string lpszUsername,
        string? lpszDomain,
        string lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        out SafeAccessTokenHandle phToken);

    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CreateProcessWithLogonW(
        string lpUsername,
        string? lpDomain,
        string lpPassword,
        uint dwLogonFlags,
        string? lpApplicationName,
        string lpCommandLine,
        uint dwCreationFlags,
        IntPtr lpEnvironment,
        string? lpCurrentDirectory,
        ref StartupInfo lpStartupInfo,
        out ProcessInformation lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct StartupInfo
    {
        public int cb;
        public string? lpReserved;
        public string? lpDesktop;
        public string? lpTitle;
        public int dwX;
        public int dwY;
        public int dwXSize;
        public int dwYSize;
        public int dwXCountChars;
        public int dwYCountChars;
        public int dwFillAttribute;
        public int dwFlags;
        public short wShowWindow;
        public short cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessInformation
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    public static void SplitUser(string userWithOptionalDomain, out string? domain, out string user) =>
        WindowsAccountName.Split(userWithOptionalDomain, out domain, out user);

    /// <summary>尝试交互式登录（本地 ACL）；失败再试 Network / NewCredentials。</summary>
    public static bool TryCreateToken(
        string userWithOptionalDomain,
        string? configuredDomain,
        string password,
        out SafeAccessTokenHandle? token,
        out string? error)
    {
        token = null;
        error = null;
        SplitUser(userWithOptionalDomain, out var domainFromUser, out var user);
        var domain = !string.IsNullOrWhiteSpace(domainFromUser)
            ? domainFromUser
            : (string.IsNullOrWhiteSpace(configuredDomain) ? "." : configuredDomain.Trim());

        foreach (var logonType in new[] { LogonInteractive, LogonNetwork, LogonNewCredentials })
        {
            if (LogonUser(user, domain, password, logonType, ProviderDefault, out var handle) &&
                !handle.IsInvalid)
            {
                token = handle;
                return true;
            }

            handle.Dispose();
        }

        error = $"LogonUser failed ({Marshal.GetLastWin32Error()}) for {domain}\\{user}.";
        return false;
    }

    public static T RunImpersonated<T>(SafeAccessTokenHandle token, Func<T> action)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(action);
        return WindowsIdentity.RunImpersonated(token, action);
    }

    public static void RunImpersonated(SafeAccessTokenHandle token, Action action) =>
        RunImpersonated(token, () =>
        {
            action();
            return 0;
        });

    /// <summary>以指定账号启动进程（用于以 PRED-TESTING 打开资源管理器访问本地受限目录）。</summary>
    public static string? StartProcessWithLogon(
        string userWithOptionalDomain,
        string? configuredDomain,
        string password,
        string applicationName,
        string arguments)
    {
        SplitUser(userWithOptionalDomain, out var domainFromUser, out var user);
        var domain = !string.IsNullOrWhiteSpace(domainFromUser)
            ? domainFromUser
            : (string.IsNullOrWhiteSpace(configuredDomain) ? "." : configuredDomain.Trim());

        var cmd = string.IsNullOrWhiteSpace(arguments)
            ? $"\"{applicationName}\""
            : $"\"{applicationName}\" {arguments}";

        var si = new StartupInfo { cb = Marshal.SizeOf<StartupInfo>() };
        if (!CreateProcessWithLogonW(
                user,
                domain,
                password,
                0,
                null,
                cmd,
                CreateUnicodeEnvironment | CreateDefaultErrorMode,
                IntPtr.Zero,
                null,
                ref si,
                out var pi))
        {
            return $"CreateProcessWithLogonW failed ({Marshal.GetLastWin32Error()}).";
        }

        if (pi.hThread != IntPtr.Zero)
            CloseHandle(pi.hThread);
        if (pi.hProcess != IntPtr.Zero)
            CloseHandle(pi.hProcess);
        return null;
    }
}
