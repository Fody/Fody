using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

public class PdbTests
{
    [Fact]
    public void ShouldContainNewFilePathWithFody()
    {
        var assembly = typeof(PdbIssueWithFody.Program).Assembly;
        var hasPath = Test(assembly);

        Assert.True(hasPath);
    }

    [Fact]
    public void ShouldContainNewFilePathWithoutFody()
    {
        var assembly = typeof(PdbIssueWithoutFody.Program).Assembly;
        var hasPath = Test(assembly);

        Assert.True(hasPath);
    }

    public static bool Test(Assembly assembly)
    {
        var myassembly = assembly.Location ?? "";
        var mypdb = myassembly.Replace(".dll", ".pdb");

        var pdbtext = File.ReadAllText(myassembly);

        var containsnewpath = pdbtext.Contains("MyNewPath");

        return containsnewpath;
    }
}
