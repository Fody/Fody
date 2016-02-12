using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyDelegateBuilder
{
    public static Action<object, T> BuildPropertySetDelegate<T>(this Type targetType, string propertyName)
    {
        Action<object, T> action;
        TryBuildPropertySetDelegate(targetType, propertyName, out action);
        return action;
    }

    static void EmptySetter<T>(object o, T t) { }

    public static bool TryBuildPropertySetDelegate<T>(this Type targetType, string propertyName, out Action<object, T> action)
    {
        var propertyInfo = targetType.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof(T), new Type[] { }, null);

        if (propertyInfo != null)
        {
            var target = Expression.Parameter(typeof (object));
            var value = Expression.Parameter(typeof (T));
            var property = Expression.Property(Expression.Convert(target, targetType), propertyInfo);
            var body = Expression.Assign(property, value);
            action = Expression.Lambda<Action<object, T>>(body, target, value)
                             .Compile();
            return true;
        }
        var fieldInfo = GetField<T>(targetType, propertyName);
        if (fieldInfo != null)
        {
            var target = Expression.Parameter(typeof (object), "target");
            var value = Expression.Parameter(typeof (T), "value");
            var fieldExp = Expression.Field(Expression.Convert(target, targetType), fieldInfo);
            var body = Expression.Assign(fieldExp, value);
            action = Expression.Lambda<Action<object, T>>(body, target, value)
                             .Compile();
            return true;

        }
        action = EmptySetter;
        return false;
    }

    public static FieldInfo GetField<TField>(this Type type, string propertyName)
    {
        var fieldInfo = type.GetField(propertyName);
        if (fieldInfo == null)
        {
            return null;
        }
        if (!fieldInfo.IsPublic)
        {
            return null;
        }
        if (fieldInfo.IsStatic)
        {
            return null;
        }
        if (!typeof(TField).IsAssignableFrom(fieldInfo.FieldType))
        {
            return null;
        }
        return fieldInfo;
    }
}