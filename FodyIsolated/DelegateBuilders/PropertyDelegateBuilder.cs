using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyDelegateBuilder
{
    public static Action<object, T> BuildPropertySetDelegate<T>(this Type type, string propertyName)
    {
        var setMethod = type.GetPropertySetMethod<T>(propertyName);
	    if (setMethod == null)
	    {
		    return (o, arg2) => { };
	    }
	    var target = Expression.Parameter(typeof (object));
	    var value = Expression.Parameter(typeof (T));
	    var body = Expression.Assign(
		    Expression.Property(Expression.Convert(target, type), setMethod),
		    value);
	    return Expression.Lambda<Action<object, T>>(body, target, value)
	                     .Compile();
    }

    public static MethodInfo GetPropertySetMethod<TProperty>(this Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof(TProperty), new Type[] { }, null);
        if (propertyInfo == null)
        {
            return null;
        }
        return propertyInfo.GetSetMethod();
    }
}