#if NET472
using System;
public class IsolatedAssemblyLoadContext
{
    AppDomain appDomain;

    public IsolatedAssemblyLoadContext(string friendlyName, string applicationBase)
    {
        var appDomainSetup = new AppDomainSetup
        {
            ApplicationBase = applicationBase,
        };
        appDomain = AppDomain.CreateDomain(friendlyName, null, appDomainSetup);
    }

    public object CreateInstanceFromAndUnwrap(string assemblyPath, string typeName)
    {
        return appDomain.CreateInstanceFromAndUnwrap(assemblyPath, typeName);
    }

    public void Unload()
    {
        AppDomain.Unload(appDomain);
    }
}
#else
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

public class IsolatedAssemblyLoadContext : AssemblyLoadContext
{
    // ReSharper disable UnusedParameter.Local
    public IsolatedAssemblyLoadContext(string friendlyName, string applicationBase)
    {
    }
    // ReSharper restore UnusedParameter.Local

    /// <inheritdoc />
    protected override Assembly Load(AssemblyName assemblyName)
    {
        var defaultAssembly = Default.LoadFromAssemblyName(assemblyName);
        if (defaultAssembly != null)
        {
            return defaultAssembly;
        }

        return LoadFromAssemblyPath(Path.Combine(AssemblyLocation.CurrentDirectory, assemblyName.Name + ".dll"));
    }

    public object CreateInstanceFromAndUnwrap(string assemblyPath, string typeName)
    {
        var assembly = LoadFromAssemblyPath(assemblyPath);
        return assembly.CreateInstance(typeName);
    }

    public void Unload()
    {
        //TODO: Not supported on .NET Core yet
    }
}
#endif