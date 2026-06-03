using AutoScrew.Hmi.Services;

namespace AutoScrew.Hmi.ViewModels;

/// <summary>兼容旧引用；逻辑已迁移至 <see cref="SurfaceBoardEditorViewModel"/>。</summary>
public sealed class TemplateBoardViewModel(LocalizationService localization) : SurfaceBoardEditorViewModel(localization);
