using System.IO;
using System.Linq;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class NuGetPackageRootTest
{
    [Test]
    public void WithNuGetPackageRoot()
    {
        var combine = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../FakeNuGetPackageRoot"));
        var nuGetPackageRoot = Path.GetFullPath(combine);
        var result = AddinFinder.ScanNuGetPackageRoot(nuGetPackageRoot)
            .Select(s=>s.Replace(@"\\", @"\").Replace(combine, "")).ToList();
        ObjectApprover.VerifyWithJson(result);
    }

}