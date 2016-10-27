using System.IO;
using NUnit.Framework;

[TestFixture]
public class NugetConfigReaderTest
{
    [Test]
    public void WithNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void FakeSolutionWithNestedNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNestedNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void WithNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../NugetPackagePathFinder/FakeSolutionWithNugetConfig/Foo"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void WithNoNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../NugetPackagePathFinder/FakeSolutionNoNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithRepoNode()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithRepoNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, "repositoryPathValue"), packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithKeyNodeEmpty()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithKeyNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithRepoNodeEmpty()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithRepoNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithKeyNode()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithKeyNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, "repositoryPathValue"), packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithPlaceholderRemovesToken()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.False(packagesPathFromConfig.Contains("$"));
    }

    [Test]
    public void NugetConfigWithPlaceholderUsesDirectory()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, "Packages"), packagesPathFromConfig);
    }

}