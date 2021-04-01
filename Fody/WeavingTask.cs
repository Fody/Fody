using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Fody
{
    public class WeavingTask :
        Task,
        ICancelableTask
    {
        Processor processor = null!;
        [Required]
        public string AssemblyFile { set; get; } = null!;

        [Required]
        public string IntermediateDirectory { get; set; } = null!;
        public string? KeyOriginatorFile { get; set; }
        public string? AssemblyOriginatorKeyFile { get; set; }

        public bool SignAssembly { get; set; }
        public bool DelaySign { get; set; }

        [Required]
        public string ProjectDirectory { get; set; } = null!;

        [Required]
        public string ProjectFile { get; set; } = null!;

        public string? DocumentationFile { get; set; }

        [Required]
        public string References { get; set; } = null!;

        [Required]
        public ITaskItem[] ReferenceCopyLocalFiles { get; set; } = null!;
        [Required]
        public ITaskItem[] WeaverFiles { get; set; } = null!;
        public string? WeaverConfiguration { get; set; }
        public ITaskItem[]? PackageReferences { get; set; }

        public string? NCrunchOriginalSolutionDirectory { get; set; }
        public string? SolutionDirectory { get; set; }

        public string? DefineConstants { get; set; }

        [Output]
        public string ExecutedWeavers { get; private set; } = null!;

        [Required]
        public string IntermediateCopyLocalFilesCache { get; set; } = null!;

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
                DelaySign = DelaySign,
                ProjectDirectory = ProjectDirectory,
                ProjectFilePath = ProjectFile,
                DocumentationFilePath = DocumentationFile,
                References = References,
                SolutionDirectory = SolutionDirectoryFinder.Find(SolutionDirectory, NCrunchOriginalSolutionDirectory, ProjectDirectory),
                ReferenceCopyLocalPaths = referenceCopyLocalPaths,
                DefineConstants = defineConstants,
                Weavers = GetWeaversFromProps(),
                WeaverConfiguration = WeaverConfiguration,
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

        public List<WeaverEntry> GetWeaversFromProps()
        {
            if (WeaverFiles == null)
            {
                return new List<WeaverEntry>();
            }

            return WeaverFiles
                .Select(
                    taskItem => new
                    {
                        taskItem.ItemSpec,
                        ClassNames = GetConfiguredClassNames(taskItem),
                        PackageReference = GetPackageReference(taskItem)
                    })
                .SelectMany(entry => entry.ClassNames.Select(
                    className =>
                        new WeaverEntry
                        {
                            AssemblyPath = entry.ItemSpec,
                            ConfiguredTypeName = className,
                            PrivateAssets = entry.PackageReference?.GetMetadata("PrivateAssets"),
                            IncludeAssets = entry.PackageReference?.GetMetadata("IncludeAssets")
                        }))
                .Distinct(WeaverEntry.NameComparer)
                .ToList();
        }

        static IEnumerable<string> GetConfiguredClassNames(ITaskItem taskItem)
        {
            return taskItem.GetMetadata("WeaverClassNames")
                .Split(';')
                .Select(name => name.Trim())
                .Where(name => !string.IsNullOrEmpty(name))
                .DefaultIfEmpty();
        }

        ITaskItem? GetPackageReference(ITaskItem weaverFileItem)
        {
            var packageName = Path.GetFileNameWithoutExtension(weaverFileItem.ItemSpec);
            return PackageReferences?.FirstOrDefault(p => string.Equals(p.ItemSpec, packageName, StringComparison.OrdinalIgnoreCase));
        }

        public void Cancel()
        {
            processor.Cancel();
        }
    }
}
