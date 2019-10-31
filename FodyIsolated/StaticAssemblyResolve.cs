using System;
using System.Reflection;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using Mono.Cecil.Rocks;

public static class StaticAssemblyResolve
{
    public static void Init()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name).Name;
        if (assemblyName == "FodyHelpers")
        {
            return typeof(BaseModuleWeaver).Assembly;
        }

        if (assemblyName == "Mono.Cecil")
        {
            return typeof(ModuleDefinition).Assembly;
        }

        if (assemblyName == "Mono.Cecil.Rocks")
        {
            return typeof(MethodBodyRocks).Assembly;
        }

        if (assemblyName == "Mono.Cecil.Pdb")
        {
            return typeof(PdbReaderProvider).Assembly;
        }

        return null;
    }
}