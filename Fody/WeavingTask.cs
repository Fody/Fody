using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{

    public class WeavingTask : Task
    {
        public string AddinSearchPaths { get; set; }

        [Required]
        public string AssemblyPath { set; get; }

        public string IntermediateDir { get; set; }
        public string KeyFilePath { get; set; }
        public string MessageImportance { set; get; }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string References { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public override bool Execute()
        {
            return new Processor
                       {
                           AddinSearchPathsFromMsBuild = AddinSearchPaths,
                           AssemblyPath = AssemblyPath,
                           IntermediateDir = IntermediateDir,
                           KeyFilePath = KeyFilePath,
                           MessageImportance = MessageImportance,
                           ProjectPath = ProjectPath,
                           References = References,
                           SolutionDir = SolutionDir,
                           BuildEngine = BuildEngine
                       }.Execute();
        }

    }
}

