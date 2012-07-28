using System;
using System.Text;
using Microsoft.Build.Framework;

public class BuildLogger : MarshalByRefObject, ILogger
{
    public IBuildEngine BuildEngine { get; set; }
    public MessageImportance MessageImportance { get; set; }

    StringBuilder stringBuilder;
 
    public virtual void Initialise(string messageImportance)
    {
        stringBuilder = new StringBuilder();
        MessageImportance messageImportanceEnum;
        if (!Enum.TryParse(messageImportance, out messageImportanceEnum))
        {
            throw new WeavingException(string.Format("Invalid MessageImportance in config. Should be 'Low', 'Normal' or 'High' but was '{0}'.", messageImportance));
        }
        MessageImportance = messageImportanceEnum;
    }

    public virtual void LogWarning(string message)
    {
        LogWarning(message, null, 0, 0, 0, 0);
    }

    public virtual void LogWarning(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        stringBuilder.AppendLine("  Warning: " + message);
        BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, "", "Fody"));
    }

    public virtual void LogInfo(string message)
    {
        stringBuilder.AppendLine("  " + message);
    }

    public virtual void LogError(string message)
    {
        LogError(message, null, 0, 0, 0, 0);
    }

    public virtual void LogError(string message, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber)
    {
        ErrorOccurred = true;
        stringBuilder.AppendLine("  Error: " + message);
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs("", "", file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, "", "Fody"));
    }

    public bool ErrorOccurred;

    public virtual void Flush()
    {
        var message = stringBuilder.ToString();
        //message = message.Substring(0, message.Length - 2);
        BuildEngine.LogMessageEvent(new BuildMessageEventArgs(message, "", "Fody", MessageImportance));
    }


}