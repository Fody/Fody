using System;
using System.Reflection;
using NUnit.Framework;

[SetUpFixture]
public class GlobalSetUp
{
    [OneTimeSetUp]
    public void Setup()
    {
        AssemblyLocation.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (string.Equals(assembly.FullName, args.Name, StringComparison.OrdinalIgnoreCase))
            {
                return assembly;
            }
        }
        return null;
    }
}