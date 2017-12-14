using System;
using System.Collections.Generic;
using System.Linq;

static class ExtractConstants
{
    internal static List<string> GetConstants(this string input)
    {
        if (input == null)
        {
            return new List<string>();
        }
        return input.Split(';').ToList();
    }
}

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