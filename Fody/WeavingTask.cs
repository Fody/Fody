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
        public string IntermediateDir { get; set; }

        public string KeyFilePath { get; set; }

        public bool SignAssembly { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        public string DocumentationFilePath { get; set; }

        [Required]
        public string References { get; set; }

        [Required]
        public ITaskItem[] ReferenceCopyLocalPaths { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public string DefineConstants { get; set; }

        public string Configuration { get; set; }

        [Output]
        public string ExecutedWeavers { get; private set; }

        public string NuGetPackageRoot { get; set; }

        public string[] PackageDefinitions { get; set; }

        //TODO move back to DebugSymbols when it resolves to true in release mode
        public bool DebugSymbols { get; set; }
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
                IntermediateDirectory = IntermediateDir,
                KeyFilePath = KeyFilePath,
                SignAssembly = SignAssembly,
                ProjectDirectory = ProjectDirectory,
                DocumentationFilePath = DocumentationFilePath,
                References = References,
                SolutionDirectory = SolutionDir,
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                NuGetPackageRoot = NuGetPackageRoot,
                PackageDefinitions = PackageDefinitions?.ToList(),
                DebugSymbols = DebugSymbolsProduced()
            };
            var success = processor.Execute();
            if (success)
            {
                var weavers = processor.Weavers.Select(x => x.AssemblyName);
                ExecutedWeavers = string.Join(";", weavers) + ";";
            }

            return success;
        }

        bool DebugSymbolsProduced()
        {
            return
                !string.Equals(DebugType, "none", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(DebugType, "embedded", StringComparison.OrdinalIgnoreCase);
        }

        public void Cancel()
        {
            processor.Cancel();
        }
    }
}

