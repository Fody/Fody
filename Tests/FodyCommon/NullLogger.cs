public class NullLogger : ILogger
{
    public void SetCurrentWeaverName(string weaverName)
    {
    }

    public void ClearWeaverName()
    {
    }

    public void LogDebug(string message)
    {
    }

    public void LogInfo(string message)
    {
    }

    public void LogMessage(string message, int level)
    {
    }

    public void LogWarning(string message)
    {
    }

    public void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
    }

    public void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
    }

    public void LogError(string message)
    {
    }
}