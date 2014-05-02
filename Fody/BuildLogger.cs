using System;
using Microsoft.Build.Framework;

public class BuildLogger : MarshalByRefObject, ILogger
{
    public IBuildEngine BuildEngine { get; set; }
    public bool ErrorOccurred;
    string currentWeaverName;

    public void LogWarning(string message)
    {
        LogWarning(message, null, 0, 0, 0, 0);
    }

    public void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, PrependMessage(message), "", "Fody"));
    }

    public void SetCurrentWeaverName(string weaverName)
    {
        currentWeaverName = weaverName;
    }

    public void ClearWeaverName()
    {
        currentWeaverName = null;
    }

    public void LogInfo(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("  " + message, "", "Fody", MessageImportance.Low));
    }
    public void LogDebug(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("  " + message, "", "Fody", MessageImportance.Low));
    }

    public void LogError(string message)
    {
        LogError(message, null, 0, 0, 0, 0);
    }

    public void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        ErrorOccurred = true;
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber,PrependMessage( message), "", "Fody"));
    }

    string PrependMessage(string message)
    {
        if (currentWeaverName == null)
        {
            return "Fody: " + message;
        }

        return string.Format("Fody/{0}: {1}", currentWeaverName, message);
    }
}