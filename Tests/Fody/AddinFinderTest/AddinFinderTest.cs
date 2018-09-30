// TODO: re-include in project when ObjectApproval supports .NET Core
#if NET472
using System;
using System.IO;
using System.Linq;
using ApprovalTests.Reporters;

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
    public void FindFromProbingPaths()
    {
        var packageRoot = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "Fody/FakeNuGetPackageRoot"));

        var probingPaths = $@";{packageRoot}\weaver1.fody\1.0.7\build\;{packageRoot}\weaver2.fody\2.0.7\build\;{packageRoot}\weaver_bad.fody\1.0.7\build\;";

        var addins = AddinFinder.BuildWeaversDictionary(AddinFinder.EnumerateAddinsFromProbingPaths(probingPaths));

        var expected = @"[Weaver1, \weaver1.fody\1.0.7\Weaver1.Fody.dll]|[Weaver2, \weaver2.fody\2.0.7\Weaver2.Fody.dll]";

        var result = string.Join("|", addins.Select(item => item.ToString().Replace(packageRoot, string.Empty)).OrderBy(item => item));

        Assert.Equal(expected, result, StringComparer.OrdinalIgnoreCase);
        Assert.Equal(@"\weaver1.fody\1.0.7\Weaver1.Fody.dll", addins["WeAvEr1"].Replace(packageRoot, string.Empty), StringComparer.OrdinalIgnoreCase);
        Assert.Equal(@"\weaver1.fody\1.0.7\Weaver1.Fody.dll", addins["weaver1"].Replace(packageRoot, string.Empty), StringComparer.OrdinalIgnoreCase);
        Assert.Equal(@"\weaver1.fody\1.0.7\Weaver1.Fody.dll", addins["WEAVER1"].Replace(packageRoot, string.Empty), StringComparer.OrdinalIgnoreCase);
    }

    static void Verify(string combine)
    {
        var addinFinder = new AddinFinder(
            log: s => { },
            solutionDirectory: Path.Combine(combine, "Solution"),
            msBuildTaskDirectory: Path.Combine(combine, "MsBuildDirectory/1/2/3"),
            nuGetPackageRoot: Path.Combine(combine, "NuGetPackageRoot"),
            weaverProbingPaths: null);
        addinFinder.FindAddinDirectories();
        ObjectApprover.VerifyWithJson(addinFinder.FodyFiles.Select(x => x.Replace(combine, "").Replace("packages", "Packages")));
    }
}
#endif