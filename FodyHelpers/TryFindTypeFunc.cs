using Mono.Cecil;

namespace Fody
{
    public delegate bool TryFindTypeFunc(string typeName, out TypeDefinition type);
}