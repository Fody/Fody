using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class WeavingTask : Task, ICancelableTask
    {
        Processor processor;
        [Required]
        public string AssemblyFile { set; get; }

        [Required]
        public string IntermediateDirectory { get; set; }
        public string KeyOriginatorFile { get; set; }
        public string AssemblyOriginatorKeyFile { get; set; }

        public bool SignAssembly { get; set; }

        [Required]
        public string ProjectDirectory { get; set; }

        [Required]
        public string ProjectFile { get; set; }

        public string DocumentationFile { get; set; }

        [Required]
        public string References { get; set; }

        [Required]
        public ITaskItem[] ReferenceCopyLocalFiles { get; set; }
        public ITaskItem[] WeaverFiles { get; set; }

        public string NCrunchOriginalSolutionDirectory { get; set; }
        public string SolutionDirectory { get; set; }

        public string DefineConstants { get; set; }

        [Output]
        public string ExecutedWeavers { get; private set; }

        public string DebugType { get; set; }

        [Required]
        public string IntermediateCopyLocalFilesCache { get; set; }

        public bool GenerateXsd { get; set; }

        public override bool Execute()
        {
            var referenceCopyLocalPaths = ReferenceCopyLocalFiles
                .Select(x => x.ItemSpec)
                .ToList();
            var defineConstants = DefineConstants.GetConstants();
            var buildLogger = new BuildLogger
            {
                BuildEngine = BuildEngine,
            };

            processor = new Processor
            {
                Logger = buildLogger,
                AssemblyFilePath = AssemblyFile,
                IntermediateDirectory = IntermediateDirectory,
                KeyFilePath = KeyOriginatorFile ?? AssemblyOriginatorKeyFile,
                SignAssembly = SignAssembly,
                ProjectDirectory = ProjectDirectory,
                ProjectFilePath = ProjectFile,
                DocumentationFilePath = DocumentationFile,
                References = References,
                SolutionDirectory = SolutionDirectoryFinder.Find(SolutionDirectory, NCrunchOriginalSolutionDirectory, ProjectDirectory),
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                Weavers = GetWeaversFromProps().Distinct(WeaverEntry.NameComparer).ToList(),
                DebugSymbols = GetDebugSymbolsType(),
                GenerateXsd = GenerateXsd
            };

            var success = processor.Execute();

            if (success)
            {
                var weavers = processor.Weavers.Select(x => x.ElementName);
                ExecutedWeavers = string.Join(";", weavers) + ";";

                try
                {
                    File.WriteAllLines(IntermediateCopyLocalFilesCache, processor.ReferenceCopyLocalPaths);
                }
                catch (Exception exception)
                {
                    buildLogger.LogInfo("ProjectDirectory: " + ProjectDirectory);
                    buildLogger.LogInfo("IntermediateDirectory: " + IntermediateDirectory);
                    buildLogger.LogInfo("CurrentDirectory: " + Directory.GetCurrentDirectory());
                    buildLogger.LogInfo("AssemblyFile: " + AssemblyFile);
                    buildLogger.LogInfo("IntermediateCopyLocalFilesCache: " + IntermediateCopyLocalFilesCache);
                    buildLogger.LogError("Error writing IntermediateCopyLocalFilesCache: " + exception.Message);
                    return false;
                }

                return true;
            }

            if (File.Exists(IntermediateCopyLocalFilesCache))
            {
                File.Delete(IntermediateCopyLocalFilesCache);
            }

            return false;
        }

        IEnumerable<WeaverEntry> GetWeaversFromProps()
        {
            if (WeaverFiles == null)
            {
                return Array.Empty<WeaverEntry>();
            }

            return WeaverFiles
                .Select(
                    taskItem => new
                    {
                        taskItem.ItemSpec,
                        ClassNames = GetConfiguredClassNames(taskItem)
                    })
                .SelectMany(entry => entry.ClassNames.Select(
                    className =>
                        new WeaverEntry
                        {
                            AssemblyPath = entry.ItemSpec,
                            ConfiguredTypeName = className
                        }));
        }

        static IEnumerable<string> GetConfiguredClassNames(ITaskItem taskItem)
        {
            return taskItem.GetMetadata("WeaverClassNames")
                .Split(';')
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .DefaultIfEmpty();
        }

        DebugSymbolsType GetDebugSymbolsType()
        {
            if (string.Equals(DebugType, "none", StringComparison.OrdinalIgnoreCase))
            {
                return DebugSymbolsType.None;
            }

            if (string.Equals(DebugType, "embedded", StringComparison.OrdinalIgnoreCase))
            {
                return DebugSymbolsType.Embedded;
            }

            return DebugSymbolsType.External;
        }

        public void Cancel()
        {
            processor.Cancel();
        }
    }
}