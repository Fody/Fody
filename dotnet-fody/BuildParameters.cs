using System.Collections.Generic;

namespace Dotnet.Fody
{
    public class BuildParameters
    {
        public BuildParameters()
        {
            References = new List<string>();
            DefineConstants = new List<string>();
        }

        public string AssemblyFilePath { get; set; }
        public string IntermediateDirectory { get; set; }
        public string KeyFilePath { get; set; }
        public bool SignAssembly { get; set; }
        public List<string> References { get; }
        public List<string> DefineConstants { get; }
    }
}