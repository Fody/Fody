using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace Fody
{
    public delegate bool TryFindTypeFunc(string typeName, [NotNullWhen(true)]out TypeDefinition? type);
}