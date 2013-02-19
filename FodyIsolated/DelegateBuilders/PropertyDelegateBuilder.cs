using System;
using System.Linq.Expressions;
using System.Reflection;

public static class PropertyDelegateBuilder
{
    public static Action<object, T> BuildPropertySetDelegate<T>(this Type type, string propertyName)
    {
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.Public, null, typeof (T), new Type[] {}, null);
        if (propertyInfo != null)
        {
            var setMethod = propertyInfo.GetSetMethod();
            var target = Expression.Parameter(typeof (object));
            var value = Expression.Parameter(typeof (T));
            var property = Expression.Property(Expression.Convert(target, type), setMethod);
            var body = Expression.Assign(property, value);
            return Expression.Lambda<Action<object, T>>(body, target, value)
                             .Compile();
        }
        var fieldInfo = GetField<T>(type, propertyName);
        if (fieldInfo != null)
        {
            var target = Expression.Parameter(typeof (object), "target");
            var value = Expression.Parameter(typeof (T), "value");
            var fieldExp = Expression.Field(Expression.Convert(target, type), fieldInfo);
            var body = Expression.Assign(fieldExp, value);
            return Expression.Lambda<Action<object, T>>(body, target, value)
                             .Compile();

        }
        return (x, y) => { };
    }

    //http://stackoverflow.com/questions/321650/how-do-i-set-a-field-value-in-an-c-sharp-expression-tree
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