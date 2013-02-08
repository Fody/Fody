using System;
using System.Linq;

public partial class InnerWeaver
{
    public void DisposeWeavers()
    {
        foreach (var weaverInstance in WeaverInstances.OfType<IDisposable>())
        {
            weaverInstance.Dispose();
        }
    }

}