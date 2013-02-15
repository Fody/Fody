using System;
using System.Reflection;

namespace FodyIsolated
{
    public static class MethodTimeLogger
    {
        [ThreadStatic]
        public static Action<string> LogDebug;

        public static void Log(MethodBase methodBase, long milliseconds)
        {
            if (LogDebug != null)
            {
                LogDebug(string.Format("Finished '{0}' {1}ms", methodBase.Name, milliseconds));
            }
        }
    }

}