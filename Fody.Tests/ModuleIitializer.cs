using System.Diagnostics;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        Fody.MethodTimeLogger.LogDebug = s => Trace.WriteLine(s);
        FodyIsolated.MethodTimeLogger.LogDebug = s => Trace.WriteLine(s);
    }
}