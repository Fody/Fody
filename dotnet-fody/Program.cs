using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dotnet.Fody
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine($"dotnet-fody {typeof(Program).Assembly.GetName().Version}");

            int exitCode;
            if (args.Length != 2 || !int.TryParse(args[0], out exitCode) || !args[1].EndsWith(".rsp"))
            {
                PrintUsage();
                return 2;
            }

            if (exitCode != 0)
            {
                Console.WriteLine("Compiler exited with an error, exiting without making changes");
                return 0;
            }

            var responseFilePath = args[1];
            var parameters = ResponseFileParser.ParseFile(responseFilePath);
            var processor = new Processor
            {
                Logger = new BuildLogger
                {
                    BuildEngine = new ConsoleBuildEngine(),
                },
                AssemblyFilePath = parameters.AssemblyFilePath,
                IntermediateDirectory = parameters.IntermediateDirectory,
                KeyFilePath = parameters.KeyFilePath,
                SignAssembly = parameters.SignAssembly,
                ProjectDirectory = Environment.CurrentDirectory,
                References = string.Join(";", parameters.References),
                SolutionDirectory = Path.GetDirectoryName(FindSolution(Environment.CurrentDirectory)),
                ReferenceCopyLocalPaths = new List<string>(), // Not a thing in .NET Core
                DefineConstants = parameters.DefineConstants,
                NuGetPackageRoot = GetNuGetPackageRoot()
            };
            var success = processor.Execute();
            if (!success)
            {
                Console.WriteLine("Weaving failed");
                return 1;
            }

            var weavers = processor.Weavers.Select(x => x.AssemblyName);
            Console.WriteLine("Executed Weavers: " + string.Join(";", weavers));
            return 0;
        }


        private static void PrintUsage()
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("dotnet fody %compile:CompilerExitCode% %compile:ResponseFile%");
        }

        private static string GetNuGetPackageRoot()
        {
            var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            if (string.IsNullOrEmpty(userProfile))
                throw new Exception("The environment variable USERPROFILE is not set");

            var nuGetPackageRoot = Path.Combine(userProfile, ".nuget", "packages");
            if (!Directory.Exists(nuGetPackageRoot))
                throw new Exception($"Could not find the nuget package directory %USERPROFILE%/.nuget/packages ({nuGetPackageRoot})");

            return nuGetPackageRoot;
        }

        private static string FindSolution(string dir)
        {
            if (dir == Path.GetPathRoot(dir))
                throw new Exception("Could not find solution");

            return Directory.EnumerateFiles(dir, "*.sln").FirstOrDefault()
                   ?? FindSolution(Path.GetDirectoryName(dir));
        }

    }
}
