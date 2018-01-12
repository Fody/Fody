using System.Collections.Generic;
using System.IO;
using Fody;
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
using ObjectApproval;
#endif
using Xunit;

//using Xunit;

#pragma warning disable 618
public class WeaverTestHelperTests : TestBase
{
    [Fact]
    public void Run()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(assemblyPath);
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
        ObjectApprover.VerifyWithJson(result, ScrubCurrentDirectory);
#endif
    }

    static string ScrubCurrentDirectory(string s)
    {
        return s.Replace(@"\\", @"\").Replace(CodeBaseLocation.CurrentDirectory, "");
    }

    [Fact]
    public void WithCustomAssemblyName()
    {
        var assemblyPath = Path.Combine(CodeBaseLocation.CurrentDirectory, "DummyAssembly.dll");
        var weaver = new TargetWeaver();
        var result = weaver.ExecuteTestRun(
            assemblyPath: assemblyPath,
            assemblyName: "NewName");
#if NET46 // TODO: Remove when ObjectApproval supports .NET Core
        ObjectApprover.VerifyWithJson(result, ScrubCurrentDirectory);
#endif
    }
}

public class TargetWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var type = FindType("System.Boolean");
        Assert.NotNull(type);
        var found = TryFindType("System.Boolean", out type);
        Assert.NotNull(type);
        Assert.True(found);
        var notFound = TryFindType("FOo", out type);
        Assert.Null(type);
        Assert.False(notFound);
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
    }
}