using System;
using System.IO;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class CecilExtensionsTests
{
    [Test]
    public async Task GetSequencePointWithNoSymbols()
    {
        var sequencePoint = ReadSequencePoint(false);
        await Assert.That(sequencePoint).IsNull();
    }

    [Test]
    public async Task GetSequencePointWithSymbols()
    {
        var sequencePoint = ReadSequencePoint(true);
        await Assert.That(sequencePoint).IsNotNull();
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
            .Single(_ => _.Name == "Method")
            .GetSequencePoint();
    }
}