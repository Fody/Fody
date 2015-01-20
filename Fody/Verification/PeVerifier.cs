using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Fody.Verification
{
    public class PeVerifier : VerifierBase
    {
        private static readonly HashSet<string> PossibleLocations = new HashSet<string>();

        private string _location;

        static PeVerifier()
        {
            var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            // New => old
            PossibleLocations.Add(Path.Combine(programFilesPath, @"Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\PeVerify.exe"));
            PossibleLocations.Add(Path.Combine(programFilesPath, @"Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\PeVerify.exe"));
            PossibleLocations.Add(Path.Combine(programFilesPath, @"Microsoft SDKs\Windows\v7.0A\Bin\PeVerify.exe"));
        }

        public PeVerifier(ILogger logger)
            : base(logger)
        {
        }

        public override string Name { get { return "PeVerify"; } }

        protected override bool VerifyAssembly(string assemblyFileName)
        {
            var verifierPath = FindVerifier();
            if (string.IsNullOrWhiteSpace(verifierPath))
            {
                Logger.LogInfo("Cannot verify assembly, no verifier found. Assuming assembly is correct.");
                return true;
            }

            if (!File.Exists(assemblyFileName))
            {
                Logger.LogInfo(string.Format("Cannot verify assembly, file '{0}' does not exist", assemblyFileName));
                return true;
            }

            Logger.LogDebug("Running verifier using command line " + string.Format("{0} {1}", verifierPath, assemblyFileName));

            var processStartInfo = new ProcessStartInfo(verifierPath);
            processStartInfo.Arguments = string.Format("\"{0}\" /ignore=0x80070002", assemblyFileName);
            processStartInfo.WorkingDirectory = Path.GetDirectoryName(assemblyFileName);

            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;

            var process = Process.Start(processStartInfo);

            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Logger.LogError(string.Format("Verification of the assembly failed using '{0}'.\n{1}", Name, output));
                return false;
            }

            return true;
        }

        private string FindVerifier()
        {
            if (string.IsNullOrWhiteSpace(_location))
            {
                foreach (var possibleLocation in PossibleLocations)
                {
                    if (File.Exists(possibleLocation))
                    {
                        _location = possibleLocation;
                        break;
                    }
                }
            }

            return _location;
        }

    }
}