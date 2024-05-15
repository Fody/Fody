using System.Runtime.InteropServices;

namespace Fody;

public sealed class AssemblyPathSet : IEquatable<AssemblyPathSet>
{
    readonly HashSet<string> assemblyPaths;
    readonly int hashCode;

    public IReadOnlyCollection<string> AssemblyPaths => assemblyPaths;

    public AssemblyPathSet(IEnumerable<string> paths)
    {
        var stringComparer = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        assemblyPaths = new(paths, stringComparer);
        hashCode = 0;

        foreach (var path in assemblyPaths.OrderBy(i => i, stringComparer))
        {
            hashCode = (hashCode * 397) ^ path.GetHashCode();
        }
    }

    public bool Equals(AssemblyPathSet? other) =>
        other is not null && assemblyPaths.SetEquals(other.assemblyPaths);

    public override bool Equals(object? obj) =>
        obj is AssemblyPathSet other && Equals(other);

    public override int GetHashCode() =>
        hashCode;
}
