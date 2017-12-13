using System;
using System.Linq.Expressions;
using System.Reflection;
using Fody;

public static class AfterWeavingDelegateBuilder
{
    public static Action<object> BuildAfterWeavingDelegate(this Type weaverType)
    {
        if (weaverType.InheritsFromBaseWeaver())
        {
            return weaver =>
            {
                var baseModuleWeaver = (BaseModuleWeaver)weaver;
                baseModuleWeaver.AfterWeaving();
            };
        }
        var afterWeavingMethod = weaverType.GetMethod("AfterWeaving", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
        if (afterWeavingMethod == null)
        {
            return null;
        }

        var target = Expression.Parameter(typeof (object));
        var execute = Expression.Call(Expression.Convert(target, weaverType), afterWeavingMethod);
        return Expression.Lambda<Action<object>>(execute, target)
                         .Compile();
    }
}