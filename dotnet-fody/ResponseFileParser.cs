using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dotnet.Fody
{
    public class ResponseFileParser
    {
        public static BuildParameters ParseFile(string responseFilePath)
        {
            if (!File.Exists(responseFilePath))
                throw new Exception($"The supplied response file {responseFilePath} does not exist");

            var contents = File.ReadAllText(responseFilePath);
            return Parse(contents);
        }

        public static BuildParameters Parse(string contents)
        {
            var buildParameters = new BuildParameters();
            var matches = Regex.Matches(contents, @"^--([^:]+):(.+)$", RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value.Trim();
                switch (key)
                {
                    case "temp-output":
                        buildParameters.IntermediateDirectory = value;
                        break;
                    case "out":
                        buildParameters.AssemblyFilePath = value;
                        break;
                    case "define":
                        buildParameters.DefineConstants.Add(value);
                        break;
                    case "key-file":
                        buildParameters.KeyFilePath = value;
                        break;
                    case "public-sign":
                        buildParameters.SignAssembly = bool.Parse(value);
                        break;
                    case "reference":
                        buildParameters.References.Add(value);
                        break;
                }
            }
            Validate(buildParameters);
            return buildParameters;
        }

        private static void Validate(BuildParameters parameters)
        {
            if (string.IsNullOrEmpty(parameters.AssemblyFilePath))
                throw new Exception("The `out` parameter was not present in the response file");
            if (string.IsNullOrEmpty(parameters.IntermediateDirectory))
                throw new Exception("The `temp-output` parameter was not present in the response file");
        }
    }
}