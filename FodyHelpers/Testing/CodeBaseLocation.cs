using System;
using System.Reflection;

namespace Fody
{
    /// <summary>
    /// Uses <see cref="Assembly.CodeBase"/> to derive the current directory.
    /// </summary>
    [Obsolete(OnlyForTesting.Message)]
    public static class CodeBaseLocation
    {
        public static string GetAssemblyLocation(this Assembly assembly)
        {
            Guard.AgainstNull(nameof(assembly), assembly);
            var uri = new UriBuilder(assembly.CodeBase);
            return Uri.UnescapeDataString(uri.Path);
        }
    }
}