using System.Text;

public static class ExceptionExtensions
{
    public static string GetLoaderMessages(this ReflectionTypeLoadException exception)
    {
        var builder = new StringBuilder();

        foreach (var loaderException in exception.LoaderExceptions)
        {
            builder.AppendLine(loaderException.ToString());
        }

        return builder.ToString();
    }

    internal static void LogException(this ILogger logger, Exception exception)
    {
        var exceptionType = exception.GetType();
        if (exceptionType.Name == "WeavingException")
        {
            var point = GetSequencePoint(exception);
            if (point == null)
            {
                logger.LogError(exception.Message);
                return;
            }

            logger.LogError(exception.Message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn);

            return;
        }

        logger.LogError(exception.ToFriendlyString());
    }

    static SequencePoint? GetSequencePoint(Exception exception)
    {
        var exceptionType = exception.GetType();
        var sequencePointProperty = exceptionType.GetProperty("SequencePoint", BindingFlags.Public | BindingFlags.Instance);
        var sequencePoint = (SequencePoint?) sequencePointProperty?.GetValue(exception, null);

        if (sequencePoint != null)
        {
            return sequencePoint;
        }

        var sequencePointField = exceptionType.GetField("SequencePoint", BindingFlags.Public | BindingFlags.Instance);
        return (SequencePoint?) sequencePointField?.GetValue(exception);
    }
}