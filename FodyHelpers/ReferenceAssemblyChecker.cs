using Mono.Cecil;
using System;
using System.Linq;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public static class ReferenceAssemblyChecker
    {
        public static bool IsImplementationAssembly(string path)
        {
            try
            {
                var asm = AssemblyDefinition.ReadAssembly(path);
                return asm.CustomAttributes.All(a => a.AttributeType.FullName != "System.Runtime.CompilerServices.ReferenceAssemblyAttribute");
            }
            catch (Exception exception)
            {
                throw new Exception($"Could not load assembly '{path}': {exception.Message}", exception);
            }
        }
    }
}