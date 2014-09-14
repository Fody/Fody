using System;
using Microsoft.Build.Framework;

public class BuildLogger : MarshalByRefObject, ILogger
{
    public IBuildEngine BuildEngine { get; set; }
    public bool ErrorOccurred;
    string currentWeaverName;

    private const MessageImportance DebugMessageImportant = MessageImportance.Normal;
    private const MessageImportance InfoMessageImportant = MessageImportance.High;

    public virtual void SetCurrentWeaverName(string weaverName)
    {
        currentWeaverName = weaverName;
    }

    public virtual void ClearWeaverName()
    {
        currentWeaverName = null;
    }

    public virtual void LogMessage(string message, MessageImportance level = InfoMessageImportant)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("  " + message, "", "Fody", level));
    }

    public virtual void LogDebug(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("  " + message, "", "Fody", DebugMessageImportant));
    }

    public virtual void LogInfo(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs("  " + message, "", "Fody", InfoMessageImportant));
    }

    public virtual void LogWarning(string message)
    {
        LogWarning(message, null, 0, 0, 0, 0);
    }

    public virtual void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, PrependMessage(message), "", "Fody"));
    }

    public virtual void LogError(string message)
    {
        LogError(message, null, 0, 0, 0, 0);
    }

    public virtual void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        ErrorOccurred = true;
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, PrependMessage(message), "", "Fody"));
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