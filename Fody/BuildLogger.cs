using System;
using Fody;
using Microsoft.Build.Framework;

public class BuildLogger :
    MarshalByRefObject,
    ILogger
{
    public IBuildEngine BuildEngine { get; set; } = null!;
    string? currentWeaverName;

    public virtual void SetCurrentWeaverName(string weaverName)
    {
        currentWeaverName = weaverName;
    }

    public virtual void ClearWeaverName()
    {
        currentWeaverName = null;
    }

    public virtual void LogMessage(string message, int level)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)level));
    }

    public virtual void LogDebug(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)MessageImportanceDefaults.Debug));
    }

    public virtual void LogInfo(string message)
    {
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)MessageImportanceDefaults.Info));
    }

    public virtual void LogWarning(string message, string? code)
    {
        LogWarning(message, null, 0, 0, 0, 0, code);
    }

    public virtual void LogWarning(string message, string? file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string? code)
    {
        BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", code ?? "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, PrependMessage(message), "", "Fody"));
    }

    public virtual void LogError(string message)
    {
        LogError(message, null, 0, 0, 0, 0);
    }

    public bool ErrorOccurred { get; private set; }

    public virtual void LogError(string message, string? file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        ErrorOccurred = true;
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, PrependMessage(message), "", "Fody"));
    }

    string GetIndent()
    {
        return currentWeaverName == null ? "" : "  ";
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
