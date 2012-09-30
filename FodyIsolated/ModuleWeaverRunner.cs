using System;
using System.Diagnostics;
using Microsoft.CSharp.RuntimeBinder;

public class ModuleWeaverRunner
{
    public ILogger Logger;

    public void Execute(dynamic weaverInstance)
    {
        Logger.LogInfo(string.Format("Executing Weaver '{0}'.", ObjectTypeName.GetTypeName(weaverInstance)));

        var stopwatch = Stopwatch.StartNew();
        try
        {
            weaverInstance.Execute();
        }
        catch (RuntimeBinderException)
        {
                throw new WeavingException("ModuleWeaver must contain a method with the signature 'public void Execute()'.");
        }
        finally
        {
            stopwatch.Stop();
        }
        Logger.LogInfo(string.Format("Finished {0}ms.{1}", stopwatch.ElapsedMilliseconds, Environment.NewLine));
    }
}

