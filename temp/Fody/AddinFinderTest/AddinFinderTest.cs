using System.IO;
using System.Linq;
using Xunit;
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
using ObjectApproval;
#endif

public partial class AddinFinderTest : TestBase
{
    [Fact]
    public void WithNuGetPackageRoot()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/FakeNuGetPackageRoot"));
        var nuGetPackageRoot = Path.GetFullPath(combine);
        var result = AddinFinder.ScanDirectoryForPackages(nuGetPackageRoot)
            .Select(s => s.Replace(@"\\", @"\").Replace(combine, ""))
            .ToList();
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
        ObjectApprover.VerifyWithJson(result);
#endif
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

    static void Verify(string combine)
    {
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(combine, "Solution"),
            msBuildTaskDirectory: Path.Combine(combine, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(combine, "NuGetPackageRoot"),
            packageDefinitions: null);
        addinFinder.FindAddinDirectories();
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
        ObjectApprover.VerifyWithJson(addinFinder.FodyFiles.Select(x => x.Replace(combine, "")));
#endif
    }
}