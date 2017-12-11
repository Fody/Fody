using System.IO;
using NUnit.Framework;

[TestFixture]
public class NugetConfigReaderTest
{
    [Test]
    public void WithNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void FakeSolutionWithNestedNugetConfig()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNestedNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void WithNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionWithNugetConfig/Foo"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsTrue(packagesPathFromConfig.EndsWith("FromNugetConfig"));
    }

    [Test]
    public void WithNoNugetConfigInTree()
    {
        var solutionDir = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../Fody/NugetPackagePathFinder/FakeSolutionNoNugetConfig"));
        var packagesPathFromConfig = NugetConfigReader.GetPackagesPathFromConfig(solutionDir);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithRepoNode()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithRepoNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Fody\repositoryPathValue"), packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithKeyNodeEmpty()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithKeyNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithRepoNodeEmpty()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithRepoNodeEmpty.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.IsNull(packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithKeyNode()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithKeyNode.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Fody\repositoryPathValue"), packagesPathFromConfig);
    }

    [Test]
    public void NugetConfigWithPlaceholderRemovesToken()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.False(packagesPathFromConfig.Contains("$"));
    }

    [Test]
    public void NugetConfigWithPlaceholderUsesDirectory()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Fody/NugetConfigWithPlaceholder.txt");
        var packagesPathFromConfig = NugetConfigReader.GetPackagePath(configPath);
        Assert.AreEqual(Path.Combine(TestContext.CurrentContext.TestDirectory, @"Fody\Packages"), packagesPathFromConfig);
    }
}