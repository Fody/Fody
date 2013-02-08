using System;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder;

public partial class InnerWeaver
{

    public void RunWeaver(dynamic weaverInstance)
    {
        var weaverName = ObjectTypeName.GetTypeName(weaverInstance);
        Logger.LogInfo(string.Format("Executing Weaver '{0}'.", weaverName));

        var stopwatch = Stopwatch.StartNew();
        try
        {
            weaverInstance.Execute();
        }
        catch (RuntimeBinderException)
        {
            var message = String.Format("ModuleWeaver '{0}' must contain a method with the signature 'public void Execute()'.", weaverName);
            throw new WeavingException(message);
        }
        finally
        {
            stopwatch.Stop();
        }
        Logger.LogInfo(string.Format("Finished {0}ms.{1}", stopwatch.ElapsedMilliseconds, Environment.NewLine));
    }
}

