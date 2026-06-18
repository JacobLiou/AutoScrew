using System.Security.Cryptography;
using System.Text;

namespace AutoScrew.Application.Services;

public static class ProductTemplatePackageHash
{
    public static string ComputePackageHash(string productFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productFolder);
        if (!Directory.Exists(productFolder))
            throw new DirectoryNotFoundException(productFolder);

        using var aggregate = new MemoryStream();
        var files = Directory.EnumerateFiles(productFolder, "*", SearchOption.AllDirectories)
            .Select(f => (Full: f, Rel: Path.GetRelativePath(productFolder, f).Replace('\\', '/')))
            .OrderBy(x => x.Rel, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var (full, rel) in files)
        {
            var relBytes = Encoding.UTF8.GetBytes(rel);
            aggregate.Write(relBytes);
            aggregate.WriteByte(0);
            var fileHash = SHA256.HashData(File.ReadAllBytes(full));
            aggregate.Write(fileHash);
        }

        return Convert.ToHexString(SHA256.HashData(aggregate.ToArray()));
    }

    public static DateTimeOffset GetPackageModifiedUtc(string productFolder)
    {
        if (!Directory.Exists(productFolder))
            return DateTimeOffset.MinValue;

        var max = DateTime.MinValue;
        foreach (var file in Directory.EnumerateFiles(productFolder, "*", SearchOption.AllDirectories))
        {
            var writeTime = File.GetLastWriteTimeUtc(file);
            if (writeTime > max)
                max = writeTime;
        }

        return max == DateTime.MinValue
            ? DateTimeOffset.MinValue
            : new DateTimeOffset(max, TimeSpan.Zero);
    }
}
