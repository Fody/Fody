public static class ProjectKind
{
    const string CSharpProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
    const string VBProjectKind = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
    const string FSharpProjectKind = "{F2A71F9B-5D33-465A-A702-920D77279786}";
    const string SolutionFolderKind = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

    public static bool IsSolutionFolderKind(string kind)
    {
        return kind.ToUpperInvariant() == SolutionFolderKind;
    }

    public static bool IsSupportedProjectKind(string kind)
    {
        var upperInvariant = kind.ToUpperInvariant();
        if (CSharpProjectKind == upperInvariant)
        {
            return true;
        }
        if (VBProjectKind == upperInvariant)
        {
            return true;
        }
        if (FSharpProjectKind == upperInvariant)
        {
            return true;
        }
        return false;
    }
}