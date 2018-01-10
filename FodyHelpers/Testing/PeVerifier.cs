using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fody
{
    /// <summary>
    /// Verifies assemblies using peverify.exe.
    /// </summary>
    [Obsolete(OnlyForTesting.Message)]
    public static class PeVerifier
    {
        public static readonly bool FoundPeVerify;
        static string peverifyPath;
        static string windowsSdkDirectory;

        static PeVerifier()
        {
            var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            windowsSdkDirectory = Path.Combine(programFilesPath, @"Microsoft SDKs\Windows");
            if (!Directory.Exists(windowsSdkDirectory))
            {
                FoundPeVerify = false;
                return;
            }

            peverifyPath = Directory.EnumerateFiles(windowsSdkDirectory, "peverify.exe", SearchOption.AllDirectories)
                .Where(x => !x.ToLowerInvariant().Contains("x64"))
                .OrderByDescending(x =>
                {
                    var info = FileVersionInfo.GetVersionInfo(x);
                    return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart);
                })
                .FirstOrDefault();
            if (peverifyPath == null)
            {
                FoundPeVerify = false;
                return;
            }

            FoundPeVerify = true;
        }

        public static bool Verify(string assemblyPath, IEnumerable<string> ignoreCodes, out string output, string workingDirectory = null)
        {
            Guard.AgainstNullAndEmpty(nameof(assemblyPath), assemblyPath);
            Guard.AgainstNull(nameof(ignoreCodes), ignoreCodes);
            if (!FoundPeVerify)
            {
                throw new Exception($"Could not find find peverify.exe in '{windowsSdkDirectory}'.");
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

            return InnerVerify(assemblyPath, ignoreCodes, out output, workingDirectory);
        }

        public static bool Verify(string beforeAssemblyPath, string afterAssemblyPath, IEnumerable<string> ignoreCodes, out string beforeOutput, out string afterOutput, string workingDirectory = null)
        {
            Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
            Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
            Guard.AgainstNull(nameof(ignoreCodes), ignoreCodes);
            var codes = ignoreCodes.ToList();
            InnerVerify(beforeAssemblyPath, codes, out beforeOutput, workingDirectory);
            InnerVerify(afterAssemblyPath, codes, out afterOutput, workingDirectory);
            afterOutput = afterOutput.TrimLineNumbers();
            beforeOutput = beforeOutput.TrimLineNumbers();
            return afterOutput == beforeOutput;
        }

        public static void ThrowIfDifferent(string beforeAssemblyPath, string afterAssemblyPath, string workingDirectory = null)
        {
            Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
            Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
            ThrowIfDifferent(beforeAssemblyPath, afterAssemblyPath, Enumerable.Empty<string>(), workingDirectory);
        }

        public static void ThrowIfDifferent(string beforeAssemblyPath, string afterAssemblyPath, IEnumerable<string> ignoreCodes, string workingDirectory = null)
        {
            Guard.AgainstNullAndEmpty(nameof(beforeAssemblyPath), beforeAssemblyPath);
            Guard.AgainstNullAndEmpty(nameof(afterAssemblyPath), afterAssemblyPath);
            Verify(beforeAssemblyPath, afterAssemblyPath, ignoreCodes, out var beforeOutput, out var afterOutput, workingDirectory);
            if (beforeOutput == afterOutput)
            {
                return;
            }

            throw new Exception($@"The files have difference peverify results.

AfterOutput:
{afterOutput}

BeforeOutput:
{beforeOutput}");
        }

        static string TrimLineNumbers(this string input)
        {
            return Regex.Replace(input, "0x.*]", "");
        }

        static bool InnerVerify(string assemblyPath, IEnumerable<string> ignoreCodes, out string output, string workingDirectory = null)
        {
            if (workingDirectory == null)
            {
                workingDirectory = Path.GetDirectoryName(assemblyPath);
            }
            var processStartInfo = new ProcessStartInfo(peverifyPath)
            {
                Arguments = $"\"{assemblyPath}\" /hresult /nologo /ignore={string.Join(",", ignoreCodes)}",
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(processStartInfo))
            {
                output = process.StandardOutput.ReadToEnd();
                output = Regex.Replace(output, @"^All Classes and Methods.*", "");
                if (!process.WaitForExit(10000))
                {
                    throw new Exception("PeVerify failed to exit");
                }

                if (process.ExitCode != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}