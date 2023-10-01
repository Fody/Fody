namespace Fody;

/// <summary>
/// Decompile assemblies using ildasm.exe.
/// </summary>
public static class Ildasm
{
    static string? ildasmPath;

    static Ildasm() =>
        FoundIldasm = SdkToolFinder.TryFindTool("ildasm", out ildasmPath);

    public static readonly bool FoundIldasm;

    [Obsolete("Prefer using https://www.nuget.org/packages/Verify.ICSharpCode.Decompiler or https://www.nuget.org/packages/ICSharpCode.Decompiler")]
    public static string Decompile(string assemblyPath, string? item = "")
    {
        Guard.AgainstNullAndEmpty(nameof(assemblyPath), assemblyPath);
        if (!FoundIldasm)
        {
            throw new("Could not find find ildasm.exe.");
        }

        if (!string.IsNullOrEmpty(item))
        {
            item = $"/item:{item}";
        }

        var startInfo = new ProcessStartInfo(
            fileName: ildasmPath,
            arguments: $"\"{assemblyPath}\" /text /linenum /source {item}")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(startInfo);
        var stringBuilder = new StringBuilder();
        while (process.StandardOutput.ReadLine() is { } line)
        {
            line=line.Trim();

            if (line.Length == 0)
            {
                continue;
            }

            if (line.Contains(".line "))
            {
                continue;
            }

            if (line.Contains(".custom instance void ["))
            {
                continue;
            }

            if (line.StartsWith("// "))
            {
                continue;
            }

            if (line.StartsWith("//0"))
            {
                continue;
            }

            if (line.StartsWith("  } // end of "))
            {
                stringBuilder.AppendLine("  } ");
                continue;
            }

            if (line.StartsWith("} // end of "))
            {
                stringBuilder.AppendLine("}");
                continue;
            }

            if (line.StartsWith("    .get instance "))
            {
                continue;
            }

            if (line.StartsWith("    .set instance "))
            {
                continue;
            }

            if (line.Contains(".language '"))
            {
                continue;
            }

            stringBuilder.AppendLine(line);
        }

        return stringBuilder.ToString();
    }
}