using System.IO;

public static class AssemblyLocation
{
    public static string CurrentDirectory()
    {
        //Use codebase because location fails for unit tests.
        // in unt tests returns a path like 
        // C:\Users\Simon\AppData\Local\Temp\o2ehfpqw.x01\Fody.Tests\assembly\dl3\0e7cab25\21728d4f_da04cd01\Fody.dll
        // And that path contains only Fody.dll and no other assemblies
		var assembly = typeof(AssemblyLocation).Assembly;
		var location = assembly.CodeBase.Replace("file:", "");
		location = location.Replace("//", "/").Replace("//", "/");

        return Path.GetDirectoryName(location);
    }
}