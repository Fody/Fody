using System.Diagnostics;
using System.Linq;
using Mono.Cecil;

public class EditAndContinueTests
{
    [Fact]
    public void RemovesEnableEditAndContinueFlag()
    {
        const DebuggableAttribute.DebuggingModes modes =
            DebuggableAttribute.DebuggingModes.Default |
            DebuggableAttribute.DebuggingModes.DisableOptimizations |
            DebuggableAttribute.DebuggingModes.EnableEditAndContinue;
        var module = BuildModuleWithDebuggable(modes);

        var changed = EditAndContinue.Disable(module);

        Assert.True(changed);
        var result = (DebuggableAttribute.DebuggingModes)ReadDebuggingModes(module);
        Assert.False(result.HasFlag(DebuggableAttribute.DebuggingModes.EnableEditAndContinue));
        // Unrelated flags are preserved so normal debugging is unaffected.
        Assert.True(result.HasFlag(DebuggableAttribute.DebuggingModes.DisableOptimizations));
    }

    [Fact]
    public void LeavesAttributeWhenFlagAbsent()
    {
        const DebuggableAttribute.DebuggingModes modes =
            DebuggableAttribute.DebuggingModes.Default |
            DebuggableAttribute.DebuggingModes.DisableOptimizations;
        var module = BuildModuleWithDebuggable(modes);

        var changed = EditAndContinue.Disable(module);

        Assert.False(changed);
        Assert.Equal((int)modes, ReadDebuggingModes(module));
    }

    [Fact]
    public void NoOpWhenNoDebuggableAttribute()
    {
        var module = BuildModule();

        Assert.False(EditAndContinue.Disable(module));
    }

    static int ReadDebuggingModes(ModuleDefinition module) =>
        (int) module.Assembly.CustomAttributes
            .Single(_ => _.Constructor.DeclaringType.FullName == "System.Diagnostics.DebuggableAttribute")
            .ConstructorArguments[0]
            .Value;

    static ModuleDefinition BuildModuleWithDebuggable(DebuggableAttribute.DebuggingModes modes)
    {
        var module = BuildModule();
        var constructor = typeof(DebuggableAttribute).GetConstructor([typeof(DebuggableAttribute.DebuggingModes)])!;
        var attribute = new CustomAttribute(module.ImportReference(constructor));
        attribute.ConstructorArguments.Add(
            new(module.ImportReference(typeof(DebuggableAttribute.DebuggingModes)), (int) modes));
        module.Assembly.CustomAttributes.Add(attribute);
        return module;
    }

    static ModuleDefinition BuildModule() =>
        AssemblyDefinition
            .CreateAssembly(new("test", new(1, 0)), "test", ModuleKind.Dll)
            .MainModule;
}
