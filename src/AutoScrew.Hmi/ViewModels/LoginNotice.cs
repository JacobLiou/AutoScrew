namespace AutoScrew.Hmi.ViewModels;

/// <summary>由 <see cref="LoginViewModel"/> 发出，由 View 以对话框等方式展示（保持 VM 不引用 Window）。</summary>
public sealed record LoginNotice(string Title, string Body);
