using System;
using System.Text;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public static class StringExtensions
    {
        public static string ReplaceCaseless(this string str, string oldValue, string newValue)
        {
            var sb = new StringBuilder();

            var previousIndex = 0;
            var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}