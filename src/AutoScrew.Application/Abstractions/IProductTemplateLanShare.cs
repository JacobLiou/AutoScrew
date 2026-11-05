namespace AutoScrew.Application.Abstractions;

public enum ProductTemplateLanPublishOutcome
{
    /// <summary>局域网原先无 template，新建发布。</summary>
    Created = 0,

    /// <summary>局域网已有且与本地内容不同，已用本地覆盖更新。</summary>
    Updated = 1,

    /// <summary>局域网与本地内容哈希一致，无需传送。</summary>
    Unchanged = 2,
}

public sealed record ProductTemplateLanPublishResult(
    string TargetFolder,
    string TemplateFilePath,
    ProductTemplateLanPublishOutcome Outcome,
    string LocalContentHash,
    string? PreviousLanContentHash);

public sealed record ProductTemplateLanEntry(
    string PartNumber,
    string TemplateFilePath,
    int SurfaceCount);

/// <summary>
/// 工艺库局域网 PN 下的螺钉模板（process/{PN}/template）。
/// </summary>
public interface IProductTemplateLanShare
{
    /// <summary>
    /// 将本机 Templates/{PN}/ 发布到 process/{PN}/template/。
    /// 允许新建 PN；局域网已存在时按内容哈希决定新建/覆盖更新/跳过。
    /// </summary>
    Task<ProductTemplateLanPublishResult> PublishAsync(
        string partNumber,
        CancellationToken cancellationToken = default);

    /// <summary>列出 process 根下已有 template/{PN}.product-template.json 的产品。</summary>
    IReadOnlyList<ProductTemplateLanEntry> ListLanTemplates();

    /// <summary>
    /// 强制将局域网 template 镜像到本机 Templates/{PN}/（覆盖已有）。
    /// 返回本机 JSON 路径。
    /// </summary>
    Task<string> PullToLocalAsync(string partNumber, CancellationToken cancellationToken = default);

    /// <summary>解析 process/{PN}/template/{PN}.product-template.json；不存在则 null。</summary>
    string? TryResolveLanTemplate(string partNumber);

    /// <summary>
    /// 若本机无模板且局域网有，则镜像复制到 Templates/{PN}/ 并返回本机 JSON 路径；
    /// 拷贝在线程池执行，透传取消令牌。
    /// </summary>
    Task<string?> TryMaterializeLanTemplateToLocalAsync(string partNumber, CancellationToken cancellationToken = default);
}
