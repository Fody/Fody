public class WeaverConfigEntry
{
    public WeaverConfigEntry(
        WeaverConfigFile configFile,
        string elementName,
        string content,
        int executionOrder)
    {
        ConfigFile = configFile;
        ElementName = elementName;
        Content = content;
        ExecutionOrder = executionOrder;
    }

    public WeaverConfigFile ConfigFile { get; }
    public string ElementName { get; }
    public string Content { get; }
    public int ExecutionOrder { get; }
}