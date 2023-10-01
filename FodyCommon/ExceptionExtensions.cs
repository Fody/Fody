public static class ExceptionExtensions
{
    public static string ToFriendlyString(this Exception exception)
    {
        var builder = new StringBuilder();
        builder.Append("An unhandled exception occurred:");
        builder.Append(Environment.NewLine);
        builder.Append("Exception:");
        builder.Append(Environment.NewLine);
        while (exception != null)
        {
            builder.Append(exception.Message);
            builder.Append(Environment.NewLine);

            builder.Append("Type:");
            builder.Append(Environment.NewLine);
            builder.Append(exception.GetType());
            builder.Append(Environment.NewLine);

            foreach (var i in exception.Data)
            {
                builder.Append("Data :");
                builder.Append(i);
                builder.Append(Environment.NewLine);
            }

            if (exception.StackTrace != null)
            {
                builder.Append("StackTrace:");
                builder.Append(Environment.NewLine);
                builder.Append(exception.StackTrace);
                builder.Append(Environment.NewLine);
            }

            if (exception.Source != null)
            {
                builder.Append("Source:");
                builder.Append(Environment.NewLine);
                builder.Append(exception.Source);
                builder.Append(Environment.NewLine);
            }

            if (exception.TargetSite != null)
            {
                builder.Append("TargetSite:");
                builder.Append(Environment.NewLine);
                builder.Append(exception.TargetSite);
                builder.Append(Environment.NewLine);
            }

            exception = exception.InnerException;
        }

        return builder.ToString();
    }
}