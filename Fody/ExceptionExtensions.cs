using System;

public static class ExceptionExtensions
{
    public static void LogException(this ILogger logger, Exception exception)
    {
        var exceptionType = exception.GetType();
        if (exceptionType.Name == "WeavingException")
        {
            logger.LogError(exception.Message);
        }
        else
        {
            logger.LogError(exception.ToFriendlyString());
        }
    }
}