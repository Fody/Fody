using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public abstract class TaskBase : Task
    {
        [Required]
        public string AssemblyPath { set; get; }

        public string IntermediateDir { get; set; }
        public string KeyFilePath { get; set; }
        public bool SignAssembly { get; set; }
        public bool VerifyAssembly { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string References { get; set; }

        //TODO: make this required on the next release
        //[Required]
        public ITaskItem[] ReferenceCopyLocalPaths { get; set; }

        [Required]
        public string SolutionDir { get; set; }

        public string DefineConstants { get; set; }

        public override bool Execute()
        {
            var referenceCopyLocalPaths = new List<string>();
            if (ReferenceCopyLocalPaths != null)
            {
                referenceCopyLocalPaths = ReferenceCopyLocalPaths.Select(x => x.ItemSpec).ToList();
            }

            var defineConstants = new List<string>();
            if (DefineConstants != null)
            {
                defineConstants = DefineConstants.Split(';').ToList();
            }

            return Execute(referenceCopyLocalPaths, defineConstants);
        }

        protected abstract bool Execute(List<string> referenceCopyLocalPaths, List<string> defineConstants);
    }
}
