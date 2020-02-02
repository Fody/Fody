using System;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace Fody
{
    [Obsolete("No longer required as BaseModuleWeaver.TryFindType has been replace with BaseModuleWeaver.TryFindTypeDefinition", false)]
    public delegate bool TryFindTypeFunc(string typeName, [NotNullWhen(true)] out TypeDefinition? type);
}