using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fody
{
    public static class ReferenceAssemblyChecker
    {
        public static bool IsReferenceAssembly(string path)
        {
            try
            {
                var asm = AssemblyDefinition.ReadAssembly(path);
                var isRefAssembly = asm.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.ReferenceAssemblyAttribute");
                return isRefAssembly;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not load assembly '{path}': {ex.Message}", ex);
            }
        }

        public static bool IsImplementationAssembly(string path) => false == IsReferenceAssembly(path);
    }
}
