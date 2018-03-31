using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Fody;

public static class DelegateBuilder
{
    public static bool InheritsFromBaseWeaver(this Type weaverType)
    {
        return typeof(BaseModuleWeaver).IsAssignableFrom(weaverType);
    }

    public static Func<BaseModuleWeaver> GetDelegateHolderFromCache(this Type weaverType)
    {
        if (!weaverDelegates.TryGetValue(weaverType.TypeHandle, out var @delegate))
        {
            weaverDelegates[weaverType.TypeHandle] = @delegate = BuildDelegateHolder(weaverType);
        }

        return @delegate;
    }

    static Dictionary<RuntimeTypeHandle, Func<BaseModuleWeaver>> weaverDelegates = new Dictionary<RuntimeTypeHandle, Func<BaseModuleWeaver>>();

    public static Func<BaseModuleWeaver> BuildDelegateHolder(this Type weaverType)
    {
        if (weaverType.InheritsFromBaseWeaver())
        {
            return weaverType.BuildConstructorDelegate();
        }

        var message = $"Cannot load/use weaver {weaverType.FullName}. Weavers must inherit from {nameof(BaseModuleWeaver)} which exists in the FodyHelpers nuget package.";
        throw new WeavingException(message);
    }

    public static Func<BaseModuleWeaver> BuildConstructorDelegate(this Type weaverType)
    {
        if (weaverType.IsNested)
        {
            throw new WeavingException($"'{weaverType.FullName}' is a nested class which is not supported.");
        }

        if (weaverType.IsAbstract || !weaverType.IsClass || !weaverType.IsPublic)
        {
            throw new WeavingException($"'{weaverType.FullName}' is not a public instance class.");
        }

        var constructorInfo = weaverType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] { }, null);
        if (constructorInfo == null)
        {
            var message = $"'{weaverType.FullName}' does not have a public instance constructor with no parameters.";
            throw new WeavingException(message);
        }

        return (Func<BaseModuleWeaver>) Expression.Lambda(Expression.TypeAs(Expression.New(constructorInfo), typeof(BaseModuleWeaver)))
            .Compile();
    }
}