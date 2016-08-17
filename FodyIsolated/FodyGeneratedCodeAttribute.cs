using System;

namespace FodyIsolated
{
    internal class FodyGeneratedCodeAttribute : Attribute
    {
        public FodyGeneratedCodeAttribute(string version)
        {
            Version = version;
        }

        public string Version { get; private set; }
    }
}