//using Fody;

public interface ILogger
{
    void SetCurrentWeaverName(string weaverName);
    void ClearWeaverName();
    void LogDebug(string message);
    void LogInfo(string message);
    void LogMessage(string message, int level);
    void LogWarning(string message);
    void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber);
    void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber);
    void LogError(string message);
}