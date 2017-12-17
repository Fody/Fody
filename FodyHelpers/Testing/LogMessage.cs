using System;

namespace Fody
{
    [Obsolete(OnlyForTesting.Message)]
    public class LogMessage
    {
        public string Text { get; internal set; }
        public object MessageImportance { get; internal set; }
    }
}