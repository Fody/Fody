using System;
using System.IO;

public class TestBase
{
    static TestBase()
    {
        var assembly = typeof(TestBase).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        var currentAssemblyPath = Uri.UnescapeDataString(uri.Path);
        AssemblyLocation.CurrentDirectory = Path.GetDirectoryName(currentAssemblyPath);
    }
}