using System;
using NUnit.Framework;

[TestFixture]
public class ExceptionExtensionsTests
{

    [Test]
    [Ignore]
    public void ExceptionHierarchyToString()
    {
        Exception exception1 = null;
        try
        {
            ThrowException1();
        }
        catch (Exception exception)
        {
            exception1 = exception;
        }
        var exceptionAsString = exception1.ExceptionHierarchyToString();
        //TODO: validate string
    }

    void ThrowException1()
    {
        try
        {
            ThrowException2();
        }
        catch (Exception exception)
        {
            throw new Exception("Exceltion1", exception);
        }
    }

    void ThrowException2()
    {
        throw new InvalidOperationException("Exception2");
    }
}