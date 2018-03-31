using System;
using System.Collections.Generic;
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
        if (!weaverType.InheritsFromBaseWeaver())
        {
            var message = $"Cannot load/use weaver {weaverType.FullName}. Weavers must inherit from {nameof(BaseModuleWeaver)} which exists in the FodyHelpers nuget package.";
            throw new WeavingException(message);
        }

        return weaverType.BuildConstructorDelegate();
    }
}