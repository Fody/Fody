using System;
using Microsoft.Build.Framework;

public class BuildLogger : MarshalByRefObject, ILogger
{
    public IBuildEngine BuildEngine { get; set; }
    public bool ErrorOccurred;
    string currentWeaverName;

    MessageImportance DebugMessageImportant = MessageImportance.Normal;
    MessageImportance InfoMessageImportant = MessageImportance.High;

    public virtual void SetCurrentWeaverName(string weaverName)
    {
        currentWeaverName = weaverName;
    }

    public virtual void ClearWeaverName()
    {
        currentWeaverName = null;
    }

    public virtual void LogMessage(string message, MessageImportance level)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)level));
    }

    public virtual void LogDebug(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance) DebugMessageImportant));
    }

    public virtual void LogInfo(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)InfoMessageImportant));
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

    string GetIndent()
    {
        return currentWeaverName == null ? "  " : "    ";
    }

    string PrependMessage(string message)
    {
        if (currentWeaverName == null)
        {
            return $"Fody: {message}";
        }

        return $"Fody/{currentWeaverName}: {message}";
    }
}