using System.Reflection;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

public static class ExceptionExtensions
{
    public static bool IsNoDefinition(this RuntimeBinderException exception)
    {
        return exception.Message.Contains("does not contain a definition for");
    }

    public static bool IsWrongType(this RuntimeBinderException exception)
    {
        return exception.Message.Contains("Cannot implicitly convert type");
    }

    public static bool IsStatic (this RuntimeBinderException exception)
    {
        return exception.Message.Contains("cannot be accessed with an instance reference; qualify it with a type name instead");
    }

    public static bool IsIncorrectParams(this RuntimeBinderException exception)
    {
        var message = exception.Message;
        if (message.Contains("No overload for method"))
        {
            return true;
        }
        return message.Contains("The best overloaded method match for") && message.Contains("has some invalid arguments");
    }

    public static string GetLoaderMessages(this ReflectionTypeLoadException exception)
    {
        var stringBuilder = new StringBuilder();
        foreach (var loaderException in exception.LoaderExceptions)
        {
            stringBuilder.AppendLine(loaderException.ToString());
        }
        return stringBuilder.ToString();
    }
}