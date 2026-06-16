using AutoScrew.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoScrew.Infrastructure.Templates;

public sealed class MesTemplateUploadService : IMesTemplateUploadService
{
    private readonly ILogger<MesTemplateUploadService> _logger;

    public MesTemplateUploadService(ILogger<MesTemplateUploadService> logger) => _logger = logger;

    public Task UploadTemplateAsync(string partNumber, string localProductFolder, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "MES template upload not implemented; PN={PartNumber}, folder={Folder}",
            partNumber,
            localProductFolder);
        return Task.CompletedTask;
    }
}
