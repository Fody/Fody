using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Fody;

public static class AccessorBuilder
{
    static bool InheritsFromBaseWeaver(this Type weaverType)
    {
        return typeof(BaseModuleWeaver).IsAssignableFrom(weaverType);
    }

    public static WeaverAccessor GetAccessorFromCache(this Type weaverType)
    {
        if (!accessors.TryGetValue(weaverType.TypeHandle, out var accessor))
        {
            accessors[weaverType.TypeHandle] = accessor = GetWeaverAccessor(weaverType);
        }

        return accessor;
    }

    static ConcurrentDictionary<RuntimeTypeHandle, WeaverAccessor> accessors = new ConcurrentDictionary<RuntimeTypeHandle, WeaverAccessor>();

    static WeaverAccessor GetWeaverAccessor(this Type weaverType)
    {
        if (!weaverType.InheritsFromBaseWeaver())
        {
            var message = $"Cannot load/use weaver {weaverType.FullName}. Weavers must inherit from {nameof(BaseModuleWeaver)} which exists in the FodyHelpers nuget package.";
            throw new WeavingException(message);
        }

        var constructor = weaverType.BuildConstructorDelegate();
        return new WeaverAccessor
        {
            Constructor = constructor
        };
    }

    public static Func<BaseModuleWeaver> BuildConstructorDelegate(this Type weaverType)
    {
        if (weaverType.IsNested)
        {
            throw new WeavingException($"'{weaverType.FullName}' is a nested class which is not supported.");
        }

        if (weaverType.IsAbstract ||
            !weaverType.IsClass ||
            !weaverType.IsPublic)
        {
            throw new WeavingException($"'{weaverType.FullName}' is not a public instance class.");
        }

        var publicInstance = BindingFlags.Instance | BindingFlags.Public;
        var constructorInfo = weaverType.GetConstructor(publicInstance, null, new Type[] { }, null);
        if (constructorInfo == null)
        {
            var message = $"'{weaverType.FullName}' does not have a public instance constructor with no parameters.";
            throw new WeavingException(message);
        }

        var construct = Expression.New(constructorInfo);
        var cast = Expression.TypeAs(construct, typeof(BaseModuleWeaver));
        return (Func<BaseModuleWeaver>) Expression.Lambda(cast)
            .Compile();
    }
}