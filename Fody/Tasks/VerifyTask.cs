using System.Collections.Generic;
using Fody.Verification;

namespace Fody
{
    public class VerifyTask : TaskBase
    {
        protected override bool Execute(List<string> referenceCopyLocalPaths, List<string> defineConstants)
        {
            var logger = new BuildLogger
            {
                BuildEngine = BuildEngine
            };

            var configuration = new Configuration(logger, SolutionDir, ProjectDirectory, defineConstants);
            if (!configuration.VerifyAssembly)
            {
                return true;
            }

            var verifier = new PeVerifier(logger);
            return verifier.Verify(FinalAssemblyPath);
        }
    }
}