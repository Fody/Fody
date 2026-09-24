using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

public class AssemblyVersionTests
{
    [Test]
    public async Task ShouldReadTheSameFodyCommonVersionInfoFromAssemblyAttributeAndFile()
    {
        var asm = System.Reflection.Assembly.Load("FodyCommon");
        var attrs = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
        var asmFileVersionAttribute = (AssemblyFileVersionAttribute?)attrs.FirstOrDefault();

        await Assert.That(asmFileVersionAttribute).IsNotNull();

        var fileVersion = FileVersionInfo.GetVersionInfo(Path.GetFullPath(asm.Location));

        await Assert.That(asmFileVersionAttribute!.Version).IsEqualTo(fileVersion.FileVersion);
    }
}