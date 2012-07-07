public interface ILogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string currentWeaver, string message);
}

