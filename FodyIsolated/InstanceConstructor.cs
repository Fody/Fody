using System;

public static class InstanceConstructor
{
    public static object ConstructInstance(this Type weaverType)
    {
        if (weaverType.IsAbstract || !weaverType.IsClass || !weaverType.IsPublic)
        {
            throw new WeavingException(String.Format("'{0}' is not public instance class.",  weaverType.FullName));
        }
        try
        {
            return Activator.CreateInstance(weaverType);
        }
        catch (Exception exception)
        {
            throw new Exception(String.Format("Could not construct instance of '{0}'.", weaverType.FullName), exception);
        }
    }
}