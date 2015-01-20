using System.Collections.Generic;
using System.Linq;

namespace Fody
{
    public class WeavingTask : TaskBase
    {
        protected override bool Execute(List<string> referenceCopyLocalPaths, List<string> defineConstants)
        {
            return new Processor
            {
                AssemblyFilePath = AssemblyPath,
                IntermediateDirectoryPath = IntermediateDir,
                KeyFilePath = KeyFilePath,
                SignAssembly = SignAssembly,
                ProjectDirectory = ProjectDirectory,
                References = References,
                SolutionDirectoryPath = SolutionDir,
                BuildEngine = BuildEngine,
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants
            }.Execute();
        }
    }
}