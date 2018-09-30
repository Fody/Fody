using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    using System;

    public class WeavingTask : Task, ICancelableTask
    {
        Processor processor;
        [Required]
        public string AssemblyPath { set; get; }

        [Required]
        public string IntermediateDirectory { get; set; }
        public string KeyOriginatorFile { get; set; }
        public string AssemblyOriginatorKeyFile { get; set; }

        public bool SignAssembly { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        public string DocumentationFilePath { get; set; }

        [Required]
        public string References { get; set; }

        [Required]
        public ITaskItem[] ReferenceCopyLocalPaths { get; set; }
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

        public override bool Execute()
        {
            var referenceCopyLocalPaths = ReferenceCopyLocalPaths.Select(x => x.ItemSpec).ToList();
            var defineConstants = DefineConstants.GetConstants();
            processor = new Processor
            {
                Logger = new BuildLogger
                {
                    BuildEngine = BuildEngine,
                },
                AssemblyFilePath = AssemblyPath,
                IntermediateDirectory = IntermediateDirectory,
                KeyFilePath = KeyOriginatorFile ?? AssemblyOriginatorKeyFile,
                SignAssembly = SignAssembly,
                ProjectDirectory = ProjectDirectory,
                DocumentationFilePath = DocumentationFilePath,
                References = References,
                SolutionDirectory = SolutionDirectoryFinder.Find(SolutionDirectory, NCrunchOriginalSolutionDirectory, ProjectDirectory),
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                NuGetPackageRoot = NuGetPackageRoot,
                MSBuildDirectory = MSBuildThisFileDirectory,
                WeaverFilesFromProps = WeaverFiles.Select(x => x.ItemSpec).ToList(),
                DebugSymbols = GetDebugSymbolsType()
            };
            var success = processor.Execute();
            if (success)
            {
                var weavers = processor.Weavers.Select(x => x.AssemblyName);
                ExecutedWeavers = string.Join(";", weavers) + ";";
            }

            return success;
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