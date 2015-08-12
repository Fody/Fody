using System;
using System.Linq.Expressions;
using System.Reflection;

public static class CancelDelegateBuilder
{

    public static Action<object> BuildCancelDelegate(this Type weaverType)
    {
        var afterWeavingMethod = weaverType.GetMethod("Cancel", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
        if (afterWeavingMethod == null)
        {
            return null;
        }

        var target = Expression.Parameter(typeof(object), "CancelWeavingDelegate Target Object");
        var execute = Expression.Call(Expression.Convert(target, weaverType), afterWeavingMethod);
        return Expression.Lambda<Action<object>>(execute, target)
                         .Compile();

    }

}