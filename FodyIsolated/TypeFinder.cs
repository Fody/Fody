using System;
using System.Linq;
using System.Reflection;

public static class TypeFinder
{
    public static Type FindType(this Assembly readAssembly, string typeName)
    {
        try
        {
            return readAssembly
                .GetTypes()
                .FirstOrDefault(x => x.Name == typeName);
        }
        catch (ReflectionTypeLoadException exception)
        {
            var message = string.Format(
                @"Could not load '{1}' from '{0}' due to ReflectionTypeLoadException.
It is possible you need to update the package.
exception.LoaderExceptions:
{2}", readAssembly.FullName, typeName, exception.GetLoaderMessages());
            throw new WeavingException(message);
        }
    }
}