namespace Fody
{
    /// <summary>
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public class LogMessage
    {
        public LogMessage(string text, object messageImportance)
        {
            Text = text;
            MessageImportance = messageImportance;
        }

        public string Text { get; }
        public object MessageImportance { get; }
    }
}