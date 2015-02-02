using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class VerifyTask : Task
    {
        [Required]
        public string SolutionDir { get; set; }
        [Required]
        public string DefineConstants { get; set; }
        [Required]
        public string ProjectDirectory { get; set; }
        [Required]
        public string TargetPath { get; set; }

        public override bool Execute()
        {
            var defineConstants = DefineConstants.Split(';').ToList();
            var verifier = new Verifier
                           {
                               Logger = new BuildLogger
                                        {
                                            BuildEngine = BuildEngine,
                                        },
                               SolutionDirectory= SolutionDir,
                               ProjectDirectory = ProjectDirectory,
                               DefineConstants = defineConstants,
                               TargetPath = TargetPath,
                           };
            return verifier.Verify();
        }

    }
}