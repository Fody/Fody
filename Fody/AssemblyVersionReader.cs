using System;
using System.Reflection;
using MethodTimer;

public class AssemblyVersionReader
{
    [Time]
    public static Version GetAssemblyVersion(string path)
    {
        try
        {
            return AssemblyName.GetAssemblyName(path).Version;
        }
        catch (BadImageFormatException)
        {
            throw new WeavingException(String.Format("Could not get version number from '{0}'. It is possible the file is corrupt.", path));
        }
    }
}