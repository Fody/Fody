public class WeaverHolder
{
    public WeaverHolder(
        BaseModuleWeaver instance,
        WeaverEntry config)
    {
        Instance = instance;
        Config = config;
    }

    public BaseModuleWeaver Instance { get; }
    public WeaverEntry Config { get; }
    public bool IsUsingOldFodyVersion;
    public int FodyVersion;
}