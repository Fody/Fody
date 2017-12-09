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