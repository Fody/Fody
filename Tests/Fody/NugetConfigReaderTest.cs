using System.IO;
using Xunit;

public class NugetConfigReaderTest : TestBase
{
    [Fact]
    public void WithNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.EndsWith("FromNugetConfig", packagesPathFromConfig);
    }

    [Fact]
    public void FakeSolutionWithNestedNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNestedNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.EndsWith("FromNugetConfig", packagesPathFromConfig);
    }

    [Fact]
    public void WithNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNugetConfig/Foo"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.EndsWith("FromNugetConfig",packagesPathFromConfig);
    }

    [Fact]
    public void WithNoNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionNoNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.Null(packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithRepoNode()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithRepoNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.Equal(Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\repositoryPathValue"), packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithKeyNodeEmpty()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithKeyNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.Null(packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithRepoNodeEmpty()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithRepoNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.Null(packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithKeyNode()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithKeyNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.Equal(Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\repositoryPathValue"), packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithPlaceholderRemovesToken()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.DoesNotContain("$", packagesPathFromConfig);
    }

    [Fact]
    public void NugetConfigWithPlaceholderUsesDirectory()
    {
        var configPath = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.Equal(Path.Combine(AssemblyLocation.CurrentDirectory, @"Fody\Packages"), packagesPathFromConfig);
    }
}