using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

public class AssemblyLocationTests :
    XunitLoggingBase
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