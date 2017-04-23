using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{

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
                References = References,
                SolutionDirectory = SolutionDir,
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                NuGetPackageRoot = NuGetPackageRoot,
                PackageDefinitions = PackageDefinitions?.ToList()
            };
            var success = processor.Execute();
            if (success)
            {
                var weavers = processor.Weavers.Select(x => x.AssemblyName);
                ExecutedWeavers = string.Join(";", weavers) + ";";
            }

            return success;
        }

        public void Cancel()
        {
            processor.Cancel();
        }
    }
}

