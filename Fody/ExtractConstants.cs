static class ExtractConstants
{
    internal static List<string> GetConstants(this string? input)
    {
        if (input == null)
        {
            return new();
        }
        return input.Split(';').ToList();
    }
}
