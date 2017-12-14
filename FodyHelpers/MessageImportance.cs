using System;

namespace Fody
{
    /// <summary>
    /// Abstraction for the MSBuild MessageImportance.
    /// </summary>
    [Serializable]
    public enum MessageImportance
    {
        High = 0,
        Normal = 1,
        Low = 2,
    }
}