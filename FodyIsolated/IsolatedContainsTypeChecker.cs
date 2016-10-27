using System;
using System.Linq;
using System.Security.Permissions;
using Mono.Cecil;
using SecurityAction = System.Security.Permissions.SecurityAction;

public class IsolatedContainsTypeChecker :
    MarshalByRefObject,
    IContainsTypeChecker
{

    public bool Check(string assemblyPath, string typeName)
    {
        var parameters = new ReaderParameters
        {
            ReadWrite = false,
            ReadingMode = ReadingMode.Deferred,
            ReadSymbols = false,
        };
        using (var module = ModuleDefinition.ReadModule(assemblyPath, parameters))
        {
            var types = module.Types;
            return types.Any(x => string.Equals(x.Name, typeName, StringComparison.OrdinalIgnoreCase));
        }
    }

    [SecurityPermission(
        SecurityAction.Demand,
        Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}