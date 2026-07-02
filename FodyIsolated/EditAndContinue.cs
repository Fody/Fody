public static class EditAndContinue
{
    // System.Diagnostics.DebuggableAttribute.DebuggingModes.EnableEditAndContinue
    const int enableEditAndContinue = 4;

    // Removes the EnableEditAndContinue flag from the DebuggableAttribute so that .NET Hot Reload / Edit and Continue
    // declines to apply in-process metadata deltas to the woven assembly. The host (dotnet watch, Visual Studio, Rider)
    // then falls back to a full rebuild + restart on each edit, which re-runs the build and re-weaves the assembly.
    // This keeps the running assembly consistent with what Fody produces, instead of silently running un-woven code.
    // See https://github.com/dotnet/roslyn/issues/56678.
    // The flag is only emitted by the compiler for debug builds, so this is a no-op for release builds.
    public static bool Disable(ModuleDefinition moduleDefinition)
    {
        var assembly = moduleDefinition.Assembly;
        if (assembly == null)
        {
            return false;
        }

        var changed = false;
        foreach (var attribute in assembly.CustomAttributes)
        {
            if (attribute.Constructor.DeclaringType.FullName != "System.Diagnostics.DebuggableAttribute")
            {
                continue;
            }

            var arguments = attribute.ConstructorArguments;
            // The DebuggingModes constructor takes a single enum (Int32) argument.
            // The legacy DebuggableAttribute(bool, bool) constructor carries no Edit and Continue flag.
            if (arguments.Count != 1 ||
                arguments[0].Value is not int modes ||
                (modes & enableEditAndContinue) == 0)
            {
                continue;
            }

            arguments[0] = new(arguments[0].Type, modes & ~enableEditAndContinue);
            changed = true;
        }

        return changed;
    }
}
