using System;
using System.Collections;
using Microsoft.Build.Framework;

namespace Dotnet.Fody
{
    public class ConsoleBuildEngine : IBuildEngine
    {
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            Console.Error.WriteLine(e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties,
            IDictionary targetOutputs)
        {
            throw new NotImplementedException();
        }

        public bool ContinueOnError { get; } = true;
        public int LineNumberOfTaskNode { get; } = 0;
        public int ColumnNumberOfTaskNode { get; } = 0;
        public string ProjectFileOfTaskNode { get; } = "";
    }
}