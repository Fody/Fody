using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{

    public class WeavingTask : Task
    {
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
                           AssemblyFilePath = AssemblyPath,
                           IntermediateDirectoryPath = IntermediateDir,
                           KeyFilePath = KeyFilePath,
                           MessageImportance = MessageImportance,
                           ProjectFilePath = ProjectPath,
                           References = References,
                           SolutionDirectoryPath = SolutionDir,
                           BuildEngine = BuildEngine
                       }.Execute();
        }

    }
}

