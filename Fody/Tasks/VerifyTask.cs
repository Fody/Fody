using System.Collections.Generic;
using System.Linq;
using Fody.Verification;

namespace Fody
{
    public class VerifyTask : TaskBase
    {
        protected override bool Execute(List<string> referenceCopyLocalPaths, List<string> defineConstants)
        {
            // TODO: read configuration

            if (!VerifyAssembly)
            {
                return true;
            }

            var verifier = new PeVerifier( new BuildLogger());
            return verifier.Verify(AssemblyPath);
        }
    }
}