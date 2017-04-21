using System;
using System.Reflection;
using System.Text;
using Mono.Cecil.Cil;

public static class ExceptionExtensions
{
    public static void LogException(this ILogger logger, Exception exception)
    {
        var exceptionType = exception.GetType();
        if (exceptionType.Name == "WeavingException")
        {
            var point = GetSequencePoint(exception);
            if (point == null)
            {
                logger.LogError(exception.Message);
            }
            else
            {
                logger.LogError(exception.Message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn);
            }
        }
        else
        {
            logger.LogError(exception.ToFriendlyString());
        }
    }

    static SequencePoint GetSequencePoint(Exception exception)
    {
        var exceptionType = exception.GetType();
        var sequencePointProperty = exceptionType.GetProperty("SequencePoint", BindingFlags.Public | BindingFlags.Instance);
        var sequencePoint = (SequencePoint) sequencePointProperty?.GetValue(exception, null);
        if (sequencePoint != null)
        {
            return sequencePoint;
        }
        var sequencePointField = exceptionType.GetField("SequencePoint", BindingFlags.Public | BindingFlags.Instance);
        return (SequencePoint) sequencePointField?.GetValue(exception);
    }

    public static string ToFriendlyString(this Exception exception)
    {

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("An unhandled exception occurred:");
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append("Exception:");
        stringBuilder.Append(Environment.NewLine);
        while (exception != null)
        {
            stringBuilder.Append(exception.Message);
            stringBuilder.Append(Environment.NewLine);

            foreach (var i in exception.Data)
            {
                stringBuilder.Append("Data :");
                stringBuilder.Append(i);
                stringBuilder.Append(Environment.NewLine);
            }

            if (exception.StackTrace != null)
            {
                stringBuilder.Append("StackTrace:");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(exception.StackTrace);
                stringBuilder.Append(Environment.NewLine);
            }

            if (exception.Source != null)
            {
                stringBuilder.Append("Source:");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(exception.Source);
                stringBuilder.Append(Environment.NewLine);
            }

            if (exception.TargetSite != null)
            {
                stringBuilder.Append("TargetSite:");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(exception.TargetSite);
                stringBuilder.Append(Environment.NewLine);
            }

            exception = exception.InnerException;
        }

        return stringBuilder.ToString();
    }
}