namespace Fody
{
    /// <summary>
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public class LogMessage
    {
        public string Text { get; internal set; }
        public object MessageImportance { get; internal set; }
    }
}