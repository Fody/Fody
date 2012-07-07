using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

public class CrossThreadRunner
{
    Exception lastException;

    public void RunInSta(Action userDelegate)
    {
        lastException = null;

        var thread = new Thread(() => MultiThreadedWorker(() => userDelegate()));
        thread.SetApartmentState(ApartmentState.STA);

        thread.Start();
        thread.Join();

        if (lastException != null)
        {
            ThrowExceptionPreservingStack(lastException);
        }
    }


    void MultiThreadedWorker(ThreadStart userDelegate)
    {
        try
        {
            userDelegate.Invoke();
        }
        catch (Exception e)
        {
            lastException = e;
        }
    }

    [ReflectionPermission(SecurityAction.Demand)]
    static void ThrowExceptionPreservingStack(Exception exception)
    {
        var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
        remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);
        throw exception;
    }
}