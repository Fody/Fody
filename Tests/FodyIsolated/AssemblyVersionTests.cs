using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

public class AssemblyVersionTests :
    XunitLoggingBase
{
    [Fact]
    public void ShouldReadTheSameFodyCommonVersionInfoFromAssemblyAttributeAndFile()
    {
        var asm = Assembly.Load("FodyCommon");
        var attrs = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
        var asmFileVersionAttribute = (AssemblyFileVersionAttribute)attrs.FirstOrDefault();

        Assert.NotNull(asmFileVersionAttribute);

        var fileVersion = FileVersionInfo.GetVersionInfo(Path.GetFullPath(asm.Location));

        Assert.Equal(fileVersion.FileVersion, asmFileVersionAttribute.Version);
    }

    public AssemblyVersionTests(ITestOutputHelper output) :
        base(output)
    {
    }
}