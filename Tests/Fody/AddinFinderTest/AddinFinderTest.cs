using System.IO;
using System.Linq;
using Fody;
using Xunit;
using ObjectApproval;

public class AddinFinderTest : TestBase
{
    [Fact]
    public void WithNuGetPackageRoot()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/FakeNuGetPackageRoot"));
        var nuGetPackageRoot = Path.GetFullPath(combine);
        var result = AddinFinder.ScanDirectoryForPackages(nuGetPackageRoot)
            .Select(s => s.Replace(@"\\", @"\").Replace(combine, ""))
            .ToList();
        ObjectApprover.VerifyWithJson(result);
    }

    [Fact]
    public void Integration_OldNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/OldNugetStructure"));
        Verify(combine);
    }

    [Fact]
    public void Integration_NewNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/NewNugetStructure"));
        Verify(combine);
    }

    [Fact]
    public void Integration_PaketStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/PaketStructure"));
        Verify(combine);
    }

    [Fact(Skip = "Need valid weavers dlls, or better mocking")]
    public void WhenVersionFilterIsSpecifiedItLimitsMatchedWeavers()
    {
        var addinFinder = new AddinFinder(null, null, null, null, null);
        addinFinder.FodyFiles.Add("");
        var filter = VersionFilter.Parse("[1.0.7, 1.0.8)");

        string found = addinFinder.FindAddinAssembly("Weaver1", filter);
        Assert.Equal("", found);
    }

    private static void Verify(string combine)
    {
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(combine, "Solution"),
            msBuildTaskDirectory: Path.Combine(combine, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(combine, "NuGetPackageRoot"),
            packageDefinitions: null);
        addinFinder.FindAddinDirectories();
        ObjectApprover.VerifyWithJson(addinFinder.FodyFiles.Select(x => x.Replace(combine, "")));
    }
}