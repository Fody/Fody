using System;
using System.Collections.Generic;
using System.Linq;

public partial class InnerWeaver
{
    public List<string> SplitReferences;

    public virtual void SplitUpReferences()
    {
        SplitReferences = References
            .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        Logger.LogDebug("Reference count=" + SplitReferences.Count);
    }
}