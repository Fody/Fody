using System.IO;
#if NET472
using System;

public class IsolatedAssemblyLoadContext
{
    AppDomain appDomain;

    public IsolatedAssemblyLoadContext()
    {
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = AssemblyLocation.CurrentDirectory,
        };
        appDomain = AppDomain.CreateDomain("Fody AppDomain", null, appDomainSetup);
    }

    public object CreateInstanceFromAndUnwrap()
    {
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        return appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver");
    }

    public void Unload()
    {
        AppDomain.Unload(appDomain);
    }
}
#else
using System.Reflection;
using System.Runtime.Loader;

public class IsolatedAssemblyLoadContext : AssemblyLoadContext
{
    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }

    public object CreateInstanceFromAndUnwrap()
    {
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        var assembly = LoadFromAssemblyPath(assemblyFile);
        return assembly.CreateInstance("InnerWeaver");
    }

    public void Unload()
    {
        //TODO: Not supported on .NET Core yet
    }
}
#endif