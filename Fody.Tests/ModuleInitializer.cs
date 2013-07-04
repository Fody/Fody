public static class ModuleInitializer
{
    public static void Initialize()
    {
        AppDomainAssemblyFinder.Attach();
    }
}