using System;
using System.Linq.Expressions;
using System.Reflection;

public static class ExecuteDelegateBuilder
{
    public static Action<object> BuildExecuteDelegate(this Type weaverType)
    {
        var executeMethod = weaverType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public, null, new Type[] {}, null);
        if (executeMethod == null)
        {
            var message = $"'{weaverType.FullName}' must contain a public instance method named 'Execute'.";
            throw new WeavingException(message);
        }

        var target = Expression.Parameter(typeof (object));
        var execute = Expression.Call(Expression.Convert(target, weaverType), executeMethod);
        return Expression.Lambda<Action<object>>(execute, target)
                         .Compile();
    }
}