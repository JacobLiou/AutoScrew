namespace AutoScrew.Hmi.Services;

/// <summary>主窗与会话级导航（登出返回登录等）。</summary>
public interface IAppSessionCoordinator
{
    void RequestLogout();
}
