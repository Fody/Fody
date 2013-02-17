using System;
using System.Reflection;

public static class InstanceConstructor
{
    public static Func<object> ConstructInstance(this Type weaverType)
    {
        if (weaverType.IsNested)
        {
            throw new WeavingException(String.Format("'{0}' is a nested class which is not supported.",  weaverType.FullName));
        }
        if (weaverType.IsAbstract || !weaverType.IsClass || !weaverType.IsPublic)
        {
            throw new WeavingException(String.Format("'{0}' is not a public instance class.",  weaverType.FullName));
        }
        if (weaverType.GetConstructor(BindingFlags.Instance| BindingFlags.Public,null,new Type[]{}, null) == null)
        {
            throw new WeavingException(String.Format("'{0}' does not have a public instance constructor with no parameters.",  weaverType.FullName));
        }
	    return () =>
		    {
			    try
			    {
				    return Activator.CreateInstance(weaverType);
			    }
			    catch (Exception exception)
			    {
				    throw new Exception(String.Format("Could not construct instance of '{0}'.", weaverType.FullName), exception);
			    }
		    };
    }
}