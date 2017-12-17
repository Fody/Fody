using System.Collections.Generic;
using System.IO;
using Fody;
using Xunit;

#pragma warning disable 618
public class WeaverTestHelperTests : TestBase
{
    [Fact]
    public void Run()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var result = new TargetWeaver().ExecuteTestRun(assemblyPath);
    }
}

public class TargetWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var type = FindType("System.Boolean");
        Assert.NotNull(type);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System";
    }
}