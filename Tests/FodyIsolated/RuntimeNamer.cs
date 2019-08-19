using System;
using ApprovalTests.Namers;

public static class RuntimeNamer
{
    public static IDisposable Build()
    {
        #if(NETCOREAPP)
        var description = "netcore";
        #else
        var description = "netclassic";
#endif
        return NamerFactory.AsEnvironmentSpecificTest(() => description);
    }
}