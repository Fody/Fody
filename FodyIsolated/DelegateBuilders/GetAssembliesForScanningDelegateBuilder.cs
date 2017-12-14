using System;
using System.Collections.Generic;
using System.Linq;
using Fody;

public static class GetAssembliesForScanningDelegateBuilder
{
    public static Func<object, IEnumerable<string>> BuildGetAssembliesForScanningDelegate(this Type weaverType)
    {
        if (weaverType.InheritsFromBaseWeaver())
        {
            return weaver =>
            {
                var baseModuleWeaver = (BaseModuleWeaver) weaver;
                return baseModuleWeaver.GetAssembliesForScanning();
            };
        }

        weaverType.ThrowIfPropertyExists(nameof(BaseModuleWeaver.GetAssembliesForScanning));
        return o => Enumerable.Empty<string>();
    }
}