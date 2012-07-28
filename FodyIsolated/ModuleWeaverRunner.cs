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
        catch (RuntimeBinderException exception)
        {
            if (exception.IsIncorrectParams() || exception.IsNoDefinition())
            {
                throw new WeavingException("ModuleWeaver must contain a method with the signature 'public void Execute()'.");
            }
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
        Logger.LogInfo(string.Format("Finished {0}ms.{1}", stopwatch.ElapsedMilliseconds, Environment.NewLine));
    }
}

