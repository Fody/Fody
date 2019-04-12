namespace Fody
{
    /// <summary>
    /// Defaults for <see cref="MessageImportance"/> when writing to <see cref="BaseModuleWeaver.LogDebug"/> and <see cref="BaseModuleWeaver.LogInfo"/>.
    /// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
    /// </summary>
    public static class MessageImportanceDefaults
    {
        public static readonly MessageImportance Debug = MessageImportance.Low;
        public static readonly MessageImportance Info = MessageImportance.Normal;
    }
}