using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Fody
{
    /// <summary>
    /// Base class for module weavers.
    /// </summary>
    public abstract class BaseModuleWeaver
    {
        /// <summary>
        /// The full element XML from FodyWeavers.xml.
        /// </summary>
        public XElement Config { get; set; }

        /// <summary>
        /// Handler for writing a log entry at the <see cref="MessageImportance.Low"/> level.
        /// </summary>
        public Action<string> LogDebug { get; set; } = m => { };

        /// <summary>
        /// Handler for writing a log entry at the <see cref="MessageImportance.Normal"/> level.
        /// </summary>
        public Action<string> LogInfo { get; set; } = m => { };

        /// <summary>
        /// Handler for writing a log entry at a specific <see cref="MessageImportance"/> level.
        /// </summary>
        public Action<string, MessageImportance> LogMessage { get; set; } = (m, p) => { };

        /// <summary>
        /// Handler for writing a warning.
        /// </summary>
        public Action<string> LogWarning { get; set; } = m => { };

        /// <summary>
        /// Handler for writing a warning at a specific point in the code
        /// </summary>
        public Action<string, SequencePoint> LogWarningPoint { get; set; } = (m, p) => { };

        /// <summary>
        /// Handler for writing an error.
        /// </summary>
        public Action<string> LogError { get; set; } = m => { };

        /// <summary>
        /// Handler for writing an error at a specific point in the code.
        /// </summary>
        public Action<string, SequencePoint> LogErrorPoint { get; set; } = (m, p) => { };

        /// <summary>
        /// Handler for resolving <see cref="AssemblyDefinition"/>s.
        /// </summary>
        public Func<string, AssemblyDefinition> ResolveAssembly { get; set; }

        /// <summary>
        /// An instance of <see cref="ModuleDefinition"/> for processing.
        /// </summary>
        public ModuleDefinition ModuleDefinition { get; set; }

        /// <summary>
        /// The full path of the target assembly.
        /// </summary>
        public string AssemblyFilePath { get; set; }

        /// <summary>
        /// The full directory path of the target project.
        /// A copy of $(ProjectDir).
        /// </summary>
        public string ProjectDirectoryPath { get; set; }

        /// <summary>
        /// The full directory path of the XML documentation file,
        /// if generating the documentation file is enabled in the project.
        /// A copy of @(DocFileItem->'%(FullPath)').
        /// </summary>
        public string DocumentationFilePath { get; set; }

        /// <summary>
        /// The full directory path of the current weaver.
        /// </summary>
        public string AddinDirectoryPath { get; set; }

        /// <summary>
        /// The full directory path of the current solution.
        /// A copy of `$(SolutionDir)` or, if it does not exist, a copy of `$(MSBuildProjectDirectory)..\..\..\`. OPTIONAL
        /// </summary>
        public string SolutionDirectoryPath { get; set; }

        /// <summary>
        /// A semicolon delimited string that contains
        /// all the references for the target project.
        /// A copy of the contents of the @(ReferencePath).
        /// </summary>
        public string References { get; set; }

        /// <summary>
        /// A list of all the references marked as copy-local.
        /// A copy of the contents of the @(ReferenceCopyLocalPaths).
        /// </summary>
        public List<string> ReferenceCopyLocalPaths { get; set; }

        /// <summary>
        /// A list of all the msbuild constants.
        /// A copy of the contents of the $(DefineConstants).
        /// </summary>
        public List<string> DefineConstants { get; set; }

        /// <summary>
        /// Called when the weaver is executed.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Called when a request to cancel the build occurs.
        /// </summary>
        public virtual void Cancel()
        {
        }

        /// <summary>
        /// Return a list of assembly names for scanning.
        /// Used as a list for <see cref="FindType"/>.
        /// </summary>
        public abstract IEnumerable<string> GetAssembliesForScanning();

        /// <summary>
        /// Handler for searching for a type.
        /// Uses all assemblies listed from calling <see cref="GetAssembliesForScanning"/> on all weavers.
        /// </summary>
        public Func<string, TypeDefinition> FindType { get; set; } = _ => throw new WeavingException($"{nameof(FindType)} has not been set.");

        /// <summary>
        /// Called after all weaving has occurred and the module has been saved.
        /// </summary>
        public virtual void AfterWeaving()
        {
        }

        /// <summary>
        /// Called after all weaving has occurred and the module has been saved.
        /// </summary>
        public virtual bool ShouldCleanReference => false;
    }
}