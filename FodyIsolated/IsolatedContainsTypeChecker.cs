using System;
using System.Linq;
using System.Security.Permissions;
using Mono.Cecil;
using SecurityAction = System.Security.Permissions.SecurityAction;

public class IsolatedContainsTypeChecker : MarshalByRefObject, IContainsTypeChecker
{
   
      //  new DependencyLoader().LoadDependencies();
    public bool Check(string assemblyPath, string typeName)
    {
        var module = ModuleDefinition.ReadModule(assemblyPath);
        var types = module.Types;
        return types.Any(x => x.Name == typeName);
    }

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
    public override object InitializeLifetimeService()
    {
        return null;
    }
}