using System;
using System.Text;

public static class ExceptionExtensions
{
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

            stringBuilder.Append("Type:");
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append(exception.GetType());
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