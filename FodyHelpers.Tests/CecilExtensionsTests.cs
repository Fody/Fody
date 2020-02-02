using System;
using System.IO;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class CecilExtensionsTests :
    VerifyBase
{
    [Fact]
    public void GetSequencePointWithNoSymbols()
    {
        var sequencePoint = ReadSequencePoint(false);
        Assert.Null(sequencePoint);
    }

    [Fact]
    public void GetSequencePointWithSymbols()
    {
        var sequencePoint = ReadSequencePoint(true);
        Assert.NotNull(sequencePoint);
    }

    static SequencePoint? ReadSequencePoint(bool readSymbols)
    {
        var assemblyPath = Path.Combine(Environment.CurrentDirectory, "DummyAssembly.dll");
        var parameters = new ReaderParameters
        {
            ReadSymbols = readSymbols
        };
        var module = ModuleDefinition.ReadModule(assemblyPath, parameters);

        return module.GetType("DummyAssembly.Class1").Methods
            .Single(x => x.Name == "Method")
            .GetSequencePoint();
    }

    public CecilExtensionsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}