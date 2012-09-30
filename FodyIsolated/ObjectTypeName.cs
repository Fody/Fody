using System;
using System.Reflection;

public static class ObjectTypeName
{
    public static string GetTypeName(this object o)
    {
        var type = o.GetType();
        return GetTypeName(type);
    }

    public static string GetTypeName(this Type type)
    {
        return string.Format("{1}, {0}", type.Assembly.GetName().Name, type.FullName);
    }

    public static PropertyInfo GetProperty<TProperty>(this Type type, string propertyName)
    {
        return type.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof (TProperty), new Type[]{}, null);
    }

    public static void SetProperty<TProperty>(this object instance, string propertyName, TProperty propertyValue)
    {
        var type = instance.GetType();
        var property = type.GetProperty<TProperty>(propertyName);
        if (property == null)
        {
            return;
        }
        property.SetValue(instance, propertyValue, null);
    }

    public static string GetAssemblyName(this object o)
    {
        var type = o.GetType();
        return type.Assembly.GetName().Name;
    }

}