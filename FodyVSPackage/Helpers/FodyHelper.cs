using System.IO;
using Catel;

namespace FodyVSPackage
{
    public static class FodyHelper
    {
        public static string GetFodyConfigForProject(string projectDirectory)
        {
            Argument.IsNotNull("projectDirectory", projectDirectory);

            return Path.Combine(projectDirectory, "FodyWeavers.xml");
        }
    }
}