using System;
using ApprovalTests.Namers;

public static class RuntimeNamer
{
    public static IDisposable BuildForRuntime()
    {
        #if(NETCOREAPP)
        var description = "netcore";
        #else
        var description = "netclassic";
        #endif
        return NamerFactory.AsEnvironmentSpecificTest(() => description);
    }
    public static IDisposable BuildForRuntimeAndConfig()
    {
        #if(NETCOREAPP)
        var description = "netcore";
        #else
        var description = "netclassic";
        #endif
        #if(RELEASE)
        description += "_release";
        #else
        description += "_debug";
        #endif
        return NamerFactory.AsEnvironmentSpecificTest(() => description);
    }
}