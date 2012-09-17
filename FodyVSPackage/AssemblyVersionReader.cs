using System;
using System.Reflection;

public class AssemblyVersionReader
{
    public static Version GetAssemblyVersion(string path)
    {
        try
        {
            return AssemblyName.GetAssemblyName(path).Version;
        }
        catch (BadImageFormatException)
        {
            throw new Exception(String.Format("Could not get version number from '{0}'. It is possible the file is corrupt.", path));
        }
    }
}