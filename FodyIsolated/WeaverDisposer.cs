using System;
using System.Linq;

public partial class InnerWeaver
{
    public void DisposeWeavers()
    {
        foreach (var weaverInstance in WeaverInstances.OfType<IDisposable>())
        {
            try
            {
                weaverInstance.Dispose();
            }
            catch (Exception exception)
            {
                var weaverName = weaverInstance.GetTypeName();
                var message = String.Format("An error occurred calling Dispose on ModuleWeaver '{0}'.", weaverName);
                throw new Exception(message, exception);
            }
        }
    }

}