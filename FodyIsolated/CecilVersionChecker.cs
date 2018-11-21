using System;
using System.Linq;
using System.Reflection;
using Fody;
using Mono.Cecil;

public static class CecilVersionChecker
{
    static byte[] expectedCecilToken = typeof(ModuleDefinition).Assembly.GetName().GetPublicKeyToken();

    public static void VerifyCecilReference(Assembly assembly)
    {
        var cecilReference = assembly.GetReferencedAssemblies()
            .SingleOrDefault(x => x.Name == "Mono.Cecil");

        if (cecilReference == null)
        {
            throw new WeavingException($"Expected the weaver '{assembly}' to reference Mono.Cecil.dll. {GetNugetError()}");
        }

        var minCecilVersion = new Version(0, 10);
        if (cecilReference.Version < minCecilVersion)
        {
            throw new WeavingException($"The weaver assembly '{assembly}' references an out of date version of Mono.Cecil.dll {cecilReference.Version}. At least version {minCecilVersion} is expected. {GetNugetError()}");
        }

        var publicKeyToken = cecilReference.GetPublicKeyToken();
        if (!publicKeyToken.SequenceEqual(expectedCecilToken))
        {
            throw new WeavingException($"The weaver assembly '{assembly}' references an out of date version of Mono.Cecil.dll. Expected strong name token of '{BitConverter.ToString(expectedCecilToken)}' but got '{BitConverter.ToString(publicKeyToken)}'. The weaver needs to update to at least version 3.0 of FodyHelpers.");
        }
    }

    static string GetNugetError()
    {
        return $"The weaver needs to add a NuGet reference to FodyCecil version {typeof(CecilVersionChecker).Assembly.GetName().Version.Major}.0.";
    }
}