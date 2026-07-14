using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoScrew.Application.Abstractions;

namespace AutoScrew.Infrastructure;

public sealed class SessionCurrentUser : ICurrentUser, INotifyPropertyChanged
{
    public string UserId { get; private set; } = "operator";

    public string DisplayName { get; private set; } = "Operator";

    public UserRole Role { get; private set; } = UserRole.Operator;

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool CanAdjustParameters => Role >= UserRole.Technician;

    /// <summary>
    /// NG 锁定后允许当前登录用户解锁继续（含操作员）；弹框遮罩会挡住其它控件，必须保留可点按钮。
    /// </summary>
    public bool CanUnlockNg => true;

    public int? MimsPersonId { get; private set; }

    public int? MimsRoleId { get; private set; }

    public int? MimsRoleType { get; private set; }

    public void SetRole(
        UserRole role,
        string userId,
        string displayName,
        int? mimsPersonId = null,
        int? mimsRoleId = null,
        int? mimsRoleType = null)
    {
        Role = role;
        UserId = userId;
        DisplayName = displayName;
        MimsPersonId = mimsPersonId;
        MimsRoleId = mimsRoleId;
        MimsRoleType = mimsRoleType;

        OnPropertyChanged(nameof(Role));
        OnPropertyChanged(nameof(UserId));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(MimsPersonId));
        OnPropertyChanged(nameof(MimsRoleId));
        OnPropertyChanged(nameof(MimsRoleType));
        OnPropertyChanged(nameof(CanAdjustParameters));
        OnPropertyChanged(nameof(CanUnlockNg));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
