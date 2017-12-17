using System;

namespace Fody
{
    /// <summary>
    /// Defaults for <see cref="MessageImportance"/> when writing to <see cref="BaseModuleWeaver.LogDebug"/> and <see cref="BaseModuleWeaver.LogInfo"/>.
    /// </summary>
    [Obsolete(OnlyForTesting.Message)]
    public static class MessageImportanceDefaults
    {
        public static readonly MessageImportance Debug = MessageImportance.Normal;
        public static readonly MessageImportance Info = MessageImportance.High;
    }
}