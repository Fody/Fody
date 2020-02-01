using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class VerifyTask :
        Task
    {
        public string? NCrunchOriginalSolutionDirectory { get; set; } = null!;
        public string? SolutionDirectory { get; set; }
        public string DefineConstants { get; set; } = null!;
        [Required]
        public string ProjectDirectory { get; set; } = null!;
        [Required]
        public string TargetPath { get; set; } = null!;

        public override bool Execute()
        {
            var defineConstants = DefineConstants.GetConstants();
            var verifier = new Verifier
            {
                Logger = new BuildLogger
                {
                    BuildEngine = BuildEngine,
                },
                SolutionDirectory = SolutionDirectoryFinder.Find(SolutionDirectory, NCrunchOriginalSolutionDirectory, ProjectDirectory),
                ProjectDirectory = ProjectDirectory,
                DefineConstants = defineConstants,
                TargetPath = TargetPath,
            };
            return verifier.Verify();
        }
    }
}