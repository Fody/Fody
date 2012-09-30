using System;
using NUnit.Framework;

[TestFixture]
public class ExceptionWindowTests
{

    [Test]
    [Ignore]
    [STAThread]
    public void Launch()
    {
        var exception = GetException();
        var model = new ExceptionWindowModel
            {
                ExceptionText = exception.ExceptionHierarchyToString(),
            };
        var window = new ExceptionWindow(model);
        window.ShowDialog();
    }

    Exception GetException()
    {
        try
        {
            ThrowException();
        }
        catch (Exception exception)
        {
            return exception;
        }
        return null;
    }

    void ThrowException()
    {
        throw new NotImplementedException("Hello1", new NullReferenceException("Hello2"));
    }
}