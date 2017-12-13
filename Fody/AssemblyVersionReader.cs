using System;
using System.Reflection;
using Fody;

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
            throw new WeavingException($"Could not get version number from '{path}'. It is possible the file is corrupt.");
        }
    }
}