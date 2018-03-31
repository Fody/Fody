using System;
using System.Linq;
using System.Reflection;

class DomainAssemblyResolver: IDisposable
{
    ResolveEventHandler domainOnAssemblyResolve;
    public DomainAssemblyResolver()
    {
        domainOnAssemblyResolve = (sender, args) => GetAssembly(args.Name);
        AppDomain.CurrentDomain.AssemblyResolve += domainOnAssemblyResolve;
    }

    public static Assembly GetAssembly(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == name);
    }

    public void Dispose()
    {
        if (domainOnAssemblyResolve != null)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= domainOnAssemblyResolve;
        }
    }
}