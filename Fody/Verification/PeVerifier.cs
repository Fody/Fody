using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Fody.Verification
{
    public class PeVerifier : IVerifier
    {
        private readonly ILogger _logger;
        private readonly string _references;
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

        public PeVerifier(ILogger logger, string references)
        {
            _logger = logger;
            _references = references;
        }

        public string Name { get { return "PeVerify"; } }

        public bool Verify(string assemblyFileName)
        {
            var verifierPath = FindVerifier();
            if (string.IsNullOrWhiteSpace(verifierPath))
            {
                _logger.LogInfo("Cannot verify assembly, no verifier found. Assuming assembly is correct.");
                return true;
            }

            if (!File.Exists(assemblyFileName))
            {
                _logger.LogInfo(string.Format("Cannot verify assembly, file '{0}' does not exist", assemblyFileName));
                return true;
            }

            var targetDirectory = Path.GetDirectoryName(assemblyFileName);

            _logger.LogDebug(string.Format("Copying references to '{0}' to ensure them for the verifier", targetDirectory));

            foreach (var reference in _references.Split(';'))
            {
                var referenceFileName = Path.GetFileName(reference);
                var destinationFile = Path.Combine(targetDirectory, referenceFileName);

                if (!File.Exists(destinationFile))
                {
                    File.Copy(reference, destinationFile);
                }
            }

            _logger.LogDebug("Running verifier using command line " + string.Format("{0} {1}", verifierPath, assemblyFileName));

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
                _logger.LogError(string.Format("Verification of the assembly failed using '{0}'.\n{1}", Name, output));
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