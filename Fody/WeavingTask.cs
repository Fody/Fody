using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class WeavingTask : Task, ICancelableTask
    {
        Processor processor;
        [Required]
        public string AssemblyFile { set; get; }

        [Required]
        public string IntermediateDirectory { get; set; }
        public string KeyOriginatorFile { get; set; }
        public string AssemblyOriginatorKeyFile { get; set; }

        public bool SignAssembly { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        public string DocumentationFile { get; set; }

        [Required]
        public string References { get; set; }

        [Required]
        public ITaskItem[] ReferenceCopyLocalFiles { get; set; }
        public ITaskItem[] WeaverFiles { get; set; }

        public string NCrunchOriginalSolutionDirectory { get; set; }
        public string SolutionDirectory { get; set; }

        [Required]
        public string MSBuildThisFileDirectory { get; set; }

        public string DefineConstants { get; set; }

        public string Configuration { get; set; }

        [Output]
        public string ExecutedWeavers { get; private set; }

        public string NuGetPackageRoot { get; set; }

        public string DebugType { get; set; }

        [Required]
        public string IntermediateCopyLocalFilesCache { get; set; }

        public override bool Execute()
        {
            // System.Diagnostics.Debugger.Launch();

            var referenceCopyLocalPaths = ReferenceCopyLocalFiles.Select(x => x.ItemSpec).ToList();

            var defineConstants = DefineConstants.GetConstants();
            processor = new Processor
            {
                Logger = new BuildLogger
                {
                    BuildEngine = BuildEngine,
                },
                AssemblyFilePath = AssemblyFile,
                IntermediateDirectory = IntermediateDirectory,
                KeyFilePath = KeyOriginatorFile ?? AssemblyOriginatorKeyFile,
                SignAssembly = SignAssembly,
                ProjectDirectory = ProjectDirectory,
                DocumentationFilePath = DocumentationFile,
                References = References,
                SolutionDirectory = SolutionDirectoryFinder.Find(SolutionDirectory, NCrunchOriginalSolutionDirectory, ProjectDirectory),
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                NuGetPackageRoot = NuGetPackageRoot,
                MSBuildDirectory = MSBuildThisFileDirectory,
                WeaverFilesFromProps = GetWeaverFilesFromProps(),
                DebugSymbols = GetDebugSymbolsType()
            };
            var success = processor.Execute();

            if (success)
            {
                var weavers = processor.Weavers.Select(x => x.AssemblyName);
                ExecutedWeavers = string.Join(";", weavers) + ";";

                File.WriteAllLines(IntermediateCopyLocalFilesCache, processor.ReferenceCopyLocalPaths);
            }
            else
            {
                if (File.Exists(IntermediateCopyLocalFilesCache))
                {
                    File.Delete(IntermediateCopyLocalFilesCache);
                }
            }

            return success;
        }

        List<string> GetWeaverFilesFromProps()
        {
            if (WeaverFiles == null)
            {
                return new List<string>();
            }
            return WeaverFiles.Select(x => x.ItemSpec)
                .ToList();
        }

        DebugSymbolsType GetDebugSymbolsType()
        {
            if (string.Equals(DebugType, "none", StringComparison.OrdinalIgnoreCase))
            {
                return DebugSymbolsType.None;
            }

            if (string.Equals(DebugType, "embedded", StringComparison.OrdinalIgnoreCase))
            {
                return DebugSymbolsType.Embedded;
            }

            return DebugSymbolsType.External;
        }

        public void Cancel()
        {
            processor.Cancel();
        }
    }
}