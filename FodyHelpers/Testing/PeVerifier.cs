namespace Fody;

/// <summary>
/// Verifies assemblies using peverify.exe.
/// Only for test usage. Only for development purposes when building Fody addins. The API may change in minor releases.
/// </summary>
public static class PeVerifier
{
    public static readonly bool FoundPeVerify;
    static string? peverifyPath;

    static PeVerifier() =>
        FoundPeVerify = SdkToolFinder.TryFindTool("peverify", out peverifyPath);

    public static bool Verify(
        string assemblyPath,
        IEnumerable<string> ignoreCodes,
        out string output,
        string? workingDirectory = null)
    {
        Guard.AgainstNullAndEmpty(nameof(assemblyPath), assemblyPath);
        Guard.AgainstNull(nameof(ignoreCodes), ignoreCodes);
        if (!FoundPeVerify)
        {
            throw new("Could not find peverify.exe.");
        }

        if (ignoreCodes == null)
        {
            throw new ArgumentNullException(nameof(ignoreCodes));
        }

        if (string.IsNullOrWhiteSpace(assemblyPath))
        {
            throw new ArgumentNullException(nameof(assemblyPath));
        }

        if (!File.Exists(assemblyPath))
        {
            throw new ArgumentNullException($"Cannot verify assembly, file '{assemblyPath}' does not exist");
        }

        return InnerVerify(assemblyPath, ignoreCodes.ToList(), out output, workingDirectory);
    }

    public static bool Verify(
        string beforeAssemblyPath,
        string afterAssemblyPath,
        IEnumerable<string> ignoreCodes,
        out string beforeOutput,
        out string afterOutput,
        string? workingDirectory = null)
    {
        Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
        Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
        Guard.AgainstNull(nameof(ignoreCodes), ignoreCodes);
        var codes = ignoreCodes.ToList();
        InnerVerify(beforeAssemblyPath, codes, out beforeOutput, workingDirectory);
        InnerVerify(afterAssemblyPath, codes, out afterOutput, workingDirectory);
        afterOutput = TrimLineNumbers(afterOutput);
        beforeOutput = TrimLineNumbers(beforeOutput);
        return afterOutput == beforeOutput;
    }

    public static void ThrowIfDifferent(
        string beforeAssemblyPath,
        string afterAssemblyPath,
        string? workingDirectory = null)
    {
        Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
        Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
        ThrowIfDifferent(beforeAssemblyPath, afterAssemblyPath, Enumerable.Empty<string>(), workingDirectory);
    }

    public static void ThrowIfDifferent(
        string beforeAssemblyPath,
        string afterAssemblyPath,
        IEnumerable<string> ignoreCodes,
        string? workingDirectory = null)
    {
        Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
        Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
        Verify(beforeAssemblyPath, afterAssemblyPath, ignoreCodes, out var beforeOutput, out var afterOutput, workingDirectory);
        if (beforeOutput == afterOutput)
        {
            return;
        }

        throw new(
            $"""
             The files have difference peverify results.

             AfterOutput:
             {afterOutput}

             BeforeOutput:
             {beforeOutput}
             """);
    }

    public static string TrimLineNumbers(string input) =>
        Regex.Replace(input, @"\[offset .*\]", "");

    static bool InnerVerify(
        string assemblyPath,
        List<string> ignoreCodes,
        out string output,
        string? workingDirectory = null)
    {
        ignoreCodes.Add("0x80070002");
        ignoreCodes.Add("0x80131252");
        workingDirectory ??= Path.GetDirectoryName(assemblyPath);
        var processStartInfo = new ProcessStartInfo(peverifyPath)
        {
            Arguments = $"\"{assemblyPath}\" /hresult /nologo /ignore={string.Join(",", ignoreCodes)}",
            WorkingDirectory = workingDirectory,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using var process = Process.Start(processStartInfo);
        output = process.StandardOutput.ReadToEnd();
        output = Regex.Replace(output, "^All Classes and Methods.*", "");
        output = output.Trim();
        if (!process.WaitForExit(10000))
        {
            throw new("PeVerify failed to exit");
        }

        if (process.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}