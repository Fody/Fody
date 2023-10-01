public static class ExceptionExtensions
{
    public static void LogException(this ILogger logger, Exception exception)
    {
        var type = exception.GetType();
        if (type.Name == "WeavingException")
        {
            logger.LogError(exception.Message);
        }
        else
        {
            logger.LogError(exception.ToFriendlyString());
        }
    }
}