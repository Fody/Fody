using System;
using System.Text;

static class StringExtensions
{
    public static string ReplaceCaseless(this string str, string oldValue, string newValue)
    {
        var builder = new StringBuilder();

        var previousIndex = 0;
        var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            builder.Append(str.Substring(previousIndex, index - previousIndex));
            builder.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
        }

        builder.Append(str.Substring(previousIndex));

        return builder.ToString();
    }
}