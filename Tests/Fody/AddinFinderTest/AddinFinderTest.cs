using System.Collections.Generic;
using System.IO;
using System.Linq;

// TODO: re-include in project when ObjectApproval supports .NET Core
#if NET472
using ApprovalTests.Reporters;

using Xunit;
using ObjectApproval;

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
        ObjectApprover.VerifyWithJson(result);
    }

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    public void Integration_OldNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/OldNugetStructure"));
        Verify(combine);
    }

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    public void Integration_NewNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/NewNugetStructure"));
        Verify(combine);
    }

    [Fact]
    [UseReporter(typeof(DiffReporter))]
    public void Integration_PaketStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/PaketStructure"));
        Verify(combine);
    }

    [Fact]
    public void IgnoreInvalidPackageDefinitions()
    {
        var root = Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/NewNugetStructure");
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(root, "Solution"),
            msBuildTaskDirectory: Path.Combine(root, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(root, "NuGetPackageRoot"),
            packageDefinitions: new List<string>
            {
                Path.Combine(root, "Solution/packages/Weaver.Fody/7.0.0"),
                Path.Combine(root, "Solution/packages/ThisIsATrap.Fody")
            },
            weaverProbingPaths: null);

        addinFinder.FindAddinDirectories();

        Assert.Contains(
            Path.Combine(root, "Solution/packages/Weaver.Fody/7.0.0/Weaver.Fody.dll").Replace('/', Path.DirectorySeparatorChar),
            addinFinder.FodyFiles.Select(i => i.Replace('/', Path.DirectorySeparatorChar))
        );
    }

    static void Verify(string combine)
    {
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(combine, "Solution"),
            msBuildTaskDirectory: Path.Combine(combine, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(combine, "NuGetPackageRoot"),
            packageDefinitions: null,
            weaverProbingPaths: null);
        addinFinder.FindAddinDirectories();
        ObjectApprover.VerifyWithJson(addinFinder.FodyFiles.Select(x => x.Replace(combine, "").Replace("packages", "Packages")));
    }
}
#endif