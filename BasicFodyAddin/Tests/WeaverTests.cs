using System;
using Fody;
using Xunit;

#region WeaverTests

public class WeaverTests
{
    static TestResult testResult;

    static WeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    [Fact]
    public void ValidateHelloWorldIsInjected()
    {
        var type = testResult.Assembly.GetType("TheNamespace.Hello");
        var instance = (dynamic)Activator.CreateInstance(type);

        Assert.Equal("Hello World", instance.World());
    }
}

#endregion