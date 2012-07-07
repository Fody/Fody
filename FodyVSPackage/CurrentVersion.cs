using System;

public static class CurrentVersion
{
    public static Version Version
    {
        get { return typeof(ContentsFinder).Assembly.GetName().Version; }
    }
}