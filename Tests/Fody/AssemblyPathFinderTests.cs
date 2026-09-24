using System.Diagnostics;

public class AssemblyLocationTests
{
    [Test]
    public async Task Foo()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        Trace.WriteLine(currentDirectory);
    }
}