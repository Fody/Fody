using System;
using System.IO;
using System.Reflection;

namespace Fody
{
    /// <summary>
    /// Uses <see cref="Assembly.CodeBase"/> to derive the current directory.
    /// </summary>
    [Obsolete(OnlyForTesting.Message)]
    public static class CodeBaseLocation
    {
        static CodeBaseLocation()
        {
            var assembly = typeof(CodeBaseLocation).Assembly;

            var currentAssemblyPath = assembly.GetAssemblyLocation();
            var currentDirectory = Path.GetDirectoryName(currentAssemblyPath);
            CurrentDirectory = currentDirectory;
        }

        public static string GetAssemblyLocation(this Assembly assembly)
        {
            Guard.AgainstNull(nameof(assembly), assembly);
            var uri = new UriBuilder(assembly.CodeBase);
            return Uri.UnescapeDataString(uri.Path);
        }

        public static readonly string CurrentDirectory;
    }
}