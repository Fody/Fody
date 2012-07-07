using System;
using System.Linq;
using NUnit.Framework;
using TargetClassLibrary;

[TestFixture]
public class EnsureAssemblyIsWovenTest
{
    [Test]
    public void Foo()
    {
        Type[] types = typeof (Class1).Assembly.GetTypes();
        var firstOrDefault = types.FirstOrDefault(type => type.Name == "MyType");
        Assert.IsNotNull(firstOrDefault);
    }
}

