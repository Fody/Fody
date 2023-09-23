using System.Diagnostics;

public class AssemblyLocationTests
{
    [Fact]
    public void Foo()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        Trace.WriteLine(currentDirectory);
    }
}