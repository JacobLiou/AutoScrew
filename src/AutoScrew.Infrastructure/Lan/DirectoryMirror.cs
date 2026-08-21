namespace AutoScrew.Infrastructure.Lan;

/// <summary>目录覆盖镜像与路径安全校验（可单测，不依赖 UNC）。</summary>
public static class DirectoryMirror
{
    /// <summary>校验源/目标目录关系；非法时返回错误说明，合法返回 null。</summary>
    public static string? ValidateMirrorPaths(string? sourceDirectory, string? targetDirectory)
    {
        if (string.IsNullOrWhiteSpace(sourceDirectory))
            return "Source directory is empty.";
        if (string.IsNullOrWhiteSpace(targetDirectory))
            return "Target directory is empty.";

        string sourceFull;
        string targetFull;
        try
        {
            sourceFull = Path.GetFullPath(sourceDirectory.Trim());
            targetFull = Path.GetFullPath(targetDirectory.Trim());
        }
        catch (Exception ex)
        {
            return $"Invalid path: {ex.Message}";
        }

        if (!Directory.Exists(sourceFull))
            return $"Source directory does not exist: {sourceFull}";

        if (string.Equals(sourceFull, targetFull, StringComparison.OrdinalIgnoreCase))
            return "Target directory must not be the same as the source.";

        var sourcePrefix = sourceFull.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                           + Path.DirectorySeparatorChar;
        if (targetFull.StartsWith(sourcePrefix, StringComparison.OrdinalIgnoreCase))
            return "Target directory must not be inside the source directory.";

        return null;
    }

    public static (int Copied, int Overwritten, int DirsCreated, List<string> Errors) Mirror(
        string sourceDirectory,
        string targetDirectory,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateMirrorPaths(sourceDirectory, targetDirectory);
        if (validation is not null)
            return (0, 0, 0, [validation]);

        var sourceFull = Path.GetFullPath(sourceDirectory.Trim());
        var targetFull = Path.GetFullPath(targetDirectory.Trim());

        var copied = 0;
        var overwritten = 0;
        var dirsCreated = 0;
        var errors = new List<string>();

        try
        {
            if (!Directory.Exists(targetFull))
            {
                Directory.CreateDirectory(targetFull);
                dirsCreated++;
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Create target failed: {ex.Message}");
            return (0, 0, 0, errors);
        }

        foreach (var dir in Directory.EnumerateDirectories(sourceFull, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rel = Path.GetRelativePath(sourceFull, dir);
            var destDir = Path.Combine(targetFull, rel);
            try
            {
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    dirsCreated++;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"{rel}: {ex.Message}");
            }
        }

        foreach (var file in Directory.EnumerateFiles(sourceFull, "*", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var rel = Path.GetRelativePath(sourceFull, file);
            var destFile = Path.Combine(targetFull, rel);
            try
            {
                var destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                    dirsCreated++;
                }

                var existed = File.Exists(destFile);
                File.Copy(file, destFile, overwrite: true);
                if (existed)
                    overwritten++;
                else
                    copied++;
            }
            catch (Exception ex)
            {
                errors.Add($"{rel}: {ex.Message}");
            }
        }

        return (copied, overwritten, dirsCreated, errors);
    }
}
