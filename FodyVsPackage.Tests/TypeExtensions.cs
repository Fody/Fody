using System;

public static class TypeExtensions
{
    public static object GetDefault(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }


}