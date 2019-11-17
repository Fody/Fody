using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class VerifyTask : Task
    {
        public string NCrunchOriginalSolutionDirectory { get; set; }
        public string SolutionDirectory { get; set; }
        public string DefineConstants { get; set; }
        [Required]
        public string ProjectDirectory { get; set; }
        [Required]
        public string TargetPath { get; set; }
        
        public string BuildConfiguration { get; set; }

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
                BuildConfiguration = BuildConfiguration,
                DefineConstants = defineConstants,
                TargetPath = TargetPath,
            };
            return verifier.Verify();
        }
    }
}