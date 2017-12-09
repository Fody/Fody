using System.Reflection;
using System.Text;

public static class ExceptionExtensions
{
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