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

    public IInnerWeaver CreateInstanceFromAndUnwrap()
    {
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        var innerWeaver = (IInnerWeaver)appDomain.CreateInstanceFromAndUnwrap(assemblyFile, "InnerWeaver");
        #if(NETSTANDARD)
        innerWeaver.LoadContext = this;
        #endif
        return innerWeaver;
    }

    public void Unload() =>
        AppDomain.Unload(appDomain);
}
#else
using System.Reflection;
using System.Runtime.Loader;

public class IsolatedAssemblyLoadContext :
    AssemblyLoadContext
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == "FodyCommon")
        {
            return typeof(ILogger).Assembly;
        }
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, assemblyName.Name+".dll");

        if (File.Exists(assemblyFile))
        {
            return LoadFromAssemblyPath(assemblyFile);
        }

        return null;
    }

    public IInnerWeaver CreateInstanceFromAndUnwrap()
    {
        var assemblyFile = Path.Combine(AssemblyLocation.CurrentDirectory, "FodyIsolated.dll");
        var assembly = LoadFromAssemblyPath(assemblyFile);
        var innerWeaver = (IInnerWeaver)assembly.CreateInstance("InnerWeaver");
        innerWeaver.LoadContext = this;
        return innerWeaver;
    }

    public Assembly LoadNotLocked(string assemblyPath)
    {
        using var stream = File.OpenRead(assemblyPath);
        return LoadFromStream(stream);
    }

    public void Unload()
    {
        //TODO: Not supported on .NET Core yet
    }
}
#endif