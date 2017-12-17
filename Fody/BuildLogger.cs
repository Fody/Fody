using System;
using Fody;
using Microsoft.Build.Framework;

public class BuildLogger : MarshalByRefObject, ILogger
{
    public IBuildEngine BuildEngine { get; set; }
    public bool ErrorOccurred;
    string currentWeaverName;

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
#pragma warning disable 618
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)MessageImportanceDefaults.Debug));
#pragma warning restore 618
    }

    public virtual void LogInfo(string message)
    {
#pragma warning disable 618
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(GetIndent() + PrependMessage(message), "", "Fody", (Microsoft.Build.Framework.MessageImportance)MessageImportanceDefaults.Info));
#pragma warning restore 618
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