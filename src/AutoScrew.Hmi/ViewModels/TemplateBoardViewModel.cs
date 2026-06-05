using AutoScrew.Application.Abstractions;
using AutoScrew.Application.Configuration;
using AutoScrew.Hmi.Services;
using Microsoft.Extensions.Options;

namespace AutoScrew.Hmi.ViewModels;

/// <summary>兼容旧引用；逻辑已迁移至 <see cref="SurfaceBoardEditorViewModel"/>。</summary>
public sealed class TemplateBoardViewModel(
    LocalizationService localization,
    IUserAuditService audit,
    IOptions<AutoScrewAppOptions> appOptions,
    ICurrentUser user)
    : SurfaceBoardEditorViewModel(localization, audit, appOptions, user);
