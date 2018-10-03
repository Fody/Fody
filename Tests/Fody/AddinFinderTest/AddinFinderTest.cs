using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApprovalTests.Namers;
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
        using (ApprovalResults.UniqueForRuntime())
        {
            ObjectApprover.VerifyWithJson(result);
        }
    }

    [Fact]
    public void Integration_OldNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/OldNugetStructure"));
        using (ApprovalResults.UniqueForRuntime())
        {
            Verify(combine);
        }
    }

    [Fact]
    public void Integration_NewNugetStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/NewNugetStructure"));
        using (ApprovalResults.UniqueForRuntime())
        {
            Verify(combine);
        }
    }

    [Fact]
    public void Integration_PaketStructure()
    {
        var combine = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/AddinFinderTest/PaketStructure"));
        using (ApprovalResults.UniqueForRuntime())
        {
            Verify(combine);
        }
    }

    static void Verify(string combine)
    {
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(combine, "Solution"),
            msBuildTaskDirectory: Path.Combine(combine, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(combine, "NuGetPackageRoot"),
            weaverFilesFromProps: new List<string>());
        addinFinder.FindAddinDirectories();
        ObjectApprover.VerifyWithJson(addinFinder.FodyFiles.Select(x => x.Replace(combine, "").Replace("packages", "Packages")));
    }
}