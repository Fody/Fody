using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class AssemblyLocationTests
{
    [Test]
    public void Foo()
    {
        var currentDirectory = AssemblyLocation.CurrentDirectory;
        Debug.WriteLine(currentDirectory);
    }
}

