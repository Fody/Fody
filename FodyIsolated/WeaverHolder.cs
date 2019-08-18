using Fody;

public class WeaverHolder
{
    public BaseModuleWeaver Instance;
    public WeaverEntry Config;
    public bool IsUsingOldFodyVersion;
    public int FodyVersion;
}