using System;
using System.Text;

public static class ExceptionExtensions
{
    public static string ExceptionHierarchyToString(this Exception exception)
    {
        var stringBuilder = new StringBuilder();
        var count = 0;

        while (exception != null)
        {
            if (count++ == 0)
            {
                stringBuilder.AppendLine("Top-level Exception");
            }
            else
            {
                stringBuilder.AppendLine("Inner Exception " + (count - 1));
            }

            stringBuilder.AppendLine("Type:        " + exception.GetType())
                .AppendLine("Message:     " + exception.Message)
                .AppendLine("Source:      " + exception.Source);

            if (exception.StackTrace != null)
                stringBuilder.AppendLine("Stack Trace: " + exception.StackTrace.Trim());

            stringBuilder.AppendLine();
            exception = exception.InnerException;
        }
        return stringBuilder.ToString().TrimEnd();
    }
}