using System;
using System.Collections.Generic;

class WeaverNameComparer : IEqualityComparer<string>
{
    public bool Equals(string x, string y)
    {
        return string.Equals(AddinFinder.GetAddinNameFromWeaverFile(x), AddinFinder.GetAddinNameFromWeaverFile(y), StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string obj)
    {
        return AddinFinder.GetAddinNameFromWeaverFile(obj)?.GetHashCode() ?? 0;
    }
}