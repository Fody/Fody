using System.Linq;
using System.Reflection;

public static class TypeFinder
{
    public static Type? FindType(this Assembly readAssembly, string typeName)
    {
        try
        {
            return readAssembly
                .GetTypes()
                .FirstOrDefault(_ => _.Name == typeName);
        }
        catch (ReflectionTypeLoadException exception)
        {
            throw new WeavingException(
                $"""
                 Could not load '{typeName}' from '{readAssembly.FullName}' due to ReflectionTypeLoadException.
                 It is possible the package needs to be updated.
                 exception.LoaderExceptions:
                 {exception.GetLoaderMessages()}
                 """);
        }
    }
}