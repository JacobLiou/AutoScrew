using AutoScrew.Infrastructure.Lan;
using Xunit;

namespace AutoScrew.Tests;

public sealed class DirectoryMirrorTests
{
    [Fact]
    public void ValidateMirrorPaths_RejectsSamePath()
    {
        var dir = Path.Combine(Path.GetTempPath(), "autoscrew-mirror-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var err = DirectoryMirror.ValidateMirrorPaths(dir, dir);
            Assert.NotNull(err);
            Assert.Contains("same", err, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void ValidateMirrorPaths_RejectsTargetInsideSource()
    {
        var root = Path.Combine(Path.GetTempPath(), "autoscrew-mirror-" + Guid.NewGuid().ToString("N"));
        var child = Path.Combine(root, "child");
        Directory.CreateDirectory(child);
        try
        {
            var err = DirectoryMirror.ValidateMirrorPaths(root, child);
            Assert.NotNull(err);
            Assert.Contains("inside", err, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void ValidateMirrorPaths_RejectsMissingSource()
    {
        var missing = Path.Combine(Path.GetTempPath(), "autoscrew-missing-" + Guid.NewGuid().ToString("N"));
        var target = Path.Combine(Path.GetTempPath(), "autoscrew-target-" + Guid.NewGuid().ToString("N"));
        var err = DirectoryMirror.ValidateMirrorPaths(missing, target);
        Assert.NotNull(err);
        Assert.Contains("does not exist", err, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Mirror_CopiesNewAndOverwritesExisting()
    {
        var root = Path.Combine(Path.GetTempPath(), "autoscrew-mirror-" + Guid.NewGuid().ToString("N"));
        var source = Path.Combine(root, "src");
        var target = Path.Combine(root, "dst");
        Directory.CreateDirectory(Path.Combine(source, "sub"));
        File.WriteAllText(Path.Combine(source, "a.txt"), "new-a");
        File.WriteAllText(Path.Combine(source, "sub", "b.txt"), "new-b");
        Directory.CreateDirectory(target);
        File.WriteAllText(Path.Combine(target, "a.txt"), "old-a");

        try
        {
            var (copied, overwritten, dirs, errors) = DirectoryMirror.Mirror(source, target);
            Assert.Empty(errors);
            Assert.Equal(1, copied);
            Assert.Equal(1, overwritten);
            Assert.True(dirs >= 1);
            Assert.Equal("new-a", File.ReadAllText(Path.Combine(target, "a.txt")));
            Assert.Equal("new-b", File.ReadAllText(Path.Combine(target, "sub", "b.txt")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void GetShareRoot_ExtractsServerShare()
    {
        Assert.Equal(@"\\fileserver\AutoScrew", NetworkShareConnect.GetShareRoot(@"\\fileserver\AutoScrew\MAC\SN"));
        Assert.Equal(@"\\fileserver\share", NetworkShareConnect.GetShareRoot(@"\\fileserver\share"));
        Assert.Equal(@"C:\local\path", NetworkShareConnect.GetShareRoot(@"C:\local\path"));
    }

    [Theory]
    [InlineData(@"PRED-TESTING", null, "PRED-TESTING")]
    [InlineData(@"CORP\PRED-TESTING", "CORP", "PRED-TESTING")]
    [InlineData(@"PRED-TESTING@corp.local", "corp.local", "PRED-TESTING")]
    public void WindowsAccountName_Split(string input, string? domain, string user)
    {
        WindowsAccountName.Split(input, out var d, out var u);
        Assert.Equal(domain, d);
        Assert.Equal(user, u);
    }
}
