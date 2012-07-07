using Microsoft.CSharp.RuntimeBinder;

public static class RuntimeBinderExceptionExtensions
{
    public static bool IsNoDefinition(this RuntimeBinderException exception)
    {
        return exception.Message.Contains("does not contain a definition for");
    }
    public static bool IsWrongType(this RuntimeBinderException exception)
    {
        return exception.Message.Contains("Cannot implicitly convert type");
    }
}