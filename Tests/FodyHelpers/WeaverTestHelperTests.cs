using System.Collections.Generic;
using System.IO;
using Fody;
using ObjectApproval;
using Xunit;

#pragma warning disable 618
public class WeaverTestHelperTests : TestBase
{
    [Fact]
    public void Run()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(assemblyPath);
        ObjectApprover.VerifyWithJson(result);
    }

    [Fact]
    public void WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(
            assemblyPath: assemblyPath,
            assemblyName: "NewName");
        ObjectApprover.VerifyWithJson(result);
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