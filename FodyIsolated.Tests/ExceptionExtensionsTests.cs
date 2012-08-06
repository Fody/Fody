using System;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

[TestFixture]
public class ExceptionExtensionsTests
{
    [Test]
    public void IsWrongType()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.StringProperty = 6;
                                         });
        Assert.IsTrue(exception.IsWrongType());
    }

    [Test]
    public void IsIncorrectParamsCount()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.Method("string",8);
                                         });
        Assert.IsTrue(exception.IsIncorrectParams());
    }
    [Test]
    public void IsIncorrectParamsType()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.Method("string");
                                         });
        Assert.IsTrue(exception.IsIncorrectParams());
    }

    [Test]
    public void IsNoDefinition()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.NoProperty = 6;
                                         });
        Assert.IsTrue(exception.IsNoDefinition());
    }

 

    [Test]
    public void IsStaticProperty()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.StaticProperty = 6;
                                         });
        Assert.IsTrue(exception.IsStatic());
    }
    [Test]
    public void IsStaticMethod()
    {
        var exception = GetException(() =>
                                         {
                                             var instance = (dynamic) new ExceptionExtensionsTests();
                                             instance.StaticMethod();
                                         });
        Assert.IsTrue(exception.IsStatic());
    }

    static RuntimeBinderException GetException(Action action)
    {
        try
        {
            action();
        }
        catch (RuntimeBinderException exception) 
        {
            return exception;
        }
        throw new Exception();
    }

    // ReSharper disable UnusedParameter.Local
    void Method(int param)
    {
        
    }
    // ReSharper restore UnusedParameter.Local
    string StringProperty;
    public static string StaticProperty { get; set; }
    public static void StaticMethod (){}
}