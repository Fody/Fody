using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyDelegateBuilder
{
    public static Action<object, T> BuildPropertyGetter<T>(this Type type, string propertyName)
    {
        var property2 = type.GetProperty<T>(propertyName);
        return BuildPropertyGetter<T>(type, property2);
    }

    public static MethodInfo GetProperty<TProperty>(this Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof(TProperty), new Type[] { }, null);
        if (propertyInfo == null)
        {
            return null;
        }
        return propertyInfo.GetSetMethod();
    }

    public static Action<object, T> BuildPropertyGetter<T>(this Type type, MethodInfo property)
    {
        if (property == null)
        {
            return (o, arg2) => { };
        }
        var target = Expression.Parameter(typeof (object));
        var value = Expression.Parameter(typeof (T));
        var body = Expression.Assign(
            Expression.Property(Expression.Convert(target, type), property),
            value);
        return Expression.Lambda<Action<object, T>>(body, target, value)
                         .Compile();
    }

}