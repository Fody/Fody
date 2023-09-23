namespace Fody;

/// <summary>
/// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
/// </summary>
public class LogMessage(string text, object messageImportance)
{
    public string Text { get; } = text;
    public object MessageImportance { get; } = messageImportance;
}