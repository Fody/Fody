using System.Diagnostics;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class AssemblyLocationTests :
    VerifyBase
{
    [Fact]
    public void Foo()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        Trace.WriteLine(currentDirectory);
    }

    public AssemblyLocationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}