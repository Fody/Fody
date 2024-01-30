namespace Fody;

[Obsolete("No longer required as BaseModuleWeaver.TryFindType has been replace with BaseModuleWeaver.TryFindTypeDefinition", false)]
public delegate bool TryFindTypeFunc(string typeName, out TypeDefinition? type);