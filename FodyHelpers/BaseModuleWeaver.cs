namespace Fody;

/// <summary>
/// Base class for module weavers.
/// </summary>
public abstract class BaseModuleWeaver
{
    static XElement Empty = new("Empty");

    /// <summary>
    /// The full element XML from FodyWeavers.xml.
    /// </summary>
    public XElement Config { get; set; } = Empty;

    /// <summary>
    /// Write a log entry to MSBuild with the <see cref="MessageImportance.Low"/> level
    /// </summary>
    public virtual void WriteDebug(string message)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogDebug(message);
    }

    /// <summary>
    /// Handler for writing a log entry at the <see cref="MessageImportance.Low"/> level.
    /// </summary>
    [Obsolete("Use WriteDebug", false)]
    public Action<string> LogDebug { get; set; } = _ => { };

    /// <summary>
    /// Write a log entry to MSBuild with the <see cref="MessageImportance.Normal"/> level
    /// </summary>
    public virtual void WriteInfo(string message)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogInfo(message);
    }

    /// <summary>
    /// Handler for writing a log entry at the <see cref="MessageImportance.Normal"/> level.
    /// </summary>
    [Obsolete("Use WriteInfo", false)]
    public Action<string> LogInfo { get; set; } = _ => { };

    /// <summary>
    /// Write a log entry to MSBuild with <paramref name="importance"/> level
    /// </summary>
    public virtual void WriteMessage(string message, MessageImportance importance)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogMessage(message, importance);
    }

    /// <summary>
    /// Handler for writing a log entry at a specific <see cref="MessageImportance"/> level.
    /// </summary>
    [Obsolete("Use WriteMessage", false)]
    public Action<string, MessageImportance> LogMessage { get; set; } = (_, _) => { };

    /// <summary>
    /// Write a warning to MSBuild.
    /// </summary>
    public virtual void WriteWarning(string message)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogWarning(message);
    }

    /// <summary>
    /// Write a warning to MSBuild and use <paramref name="sequencePoint"/> for the file and line information.
    /// </summary>
    public virtual void WriteWarning(string message, SequencePoint? sequencePoint)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogWarningPoint(message, sequencePoint);
    }

    /// <summary>
    /// Write a warning to MSBuild and use <paramref name="method"/> for the file and line information.
    /// </summary>
    public virtual void WriteWarning(string message, MethodDefinition method)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        Guard.AgainstNull(nameof(method), method);
        LogWarningPoint(message, method.GetSequencePoint());
    }

    /// <summary>
    /// Handler for writing a warning.
    /// </summary>
    [Obsolete("Use WriteWarning", false)]
    public Action<string> LogWarning { get; set; } = _ => { };

    /// <summary>
    /// Handler for writing a warning at a specific point in the code
    /// </summary>
    [Obsolete("Use WriteWarning", false)]
    public Action<string, SequencePoint?> LogWarningPoint { get; set; } = (_, _) => { };

    /// <summary>
    /// Write an error to MSBuild.
    /// </summary>
    public virtual void WriteError(string message)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogError(message);
    }

    /// <summary>
    /// Write an error to MSBuild and use <paramref name="sequencePoint"/> for the file and line information.
    /// </summary>
    public virtual void WriteError(string message, SequencePoint? sequencePoint)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogErrorPoint(message, sequencePoint);
    }

    /// <summary>
    /// Write a error to MSBuild and use <paramref name="method"/> for the file and line information.
    /// </summary>
    public virtual void WriteError(string message, MethodDefinition method)
    {
        Guard.AgainstNullAndEmpty(nameof(message), message);
        LogErrorPoint(message, method.GetSequencePoint());
    }

    /// <summary>
    /// Handler for writing an error.
    /// </summary>
    [Obsolete("Use WriteError", false)]
    public Action<string> LogError { get; set; } = _ => { };

    /// <summary>
    /// Handler for writing an error at a specific point in the code.
    /// </summary>
    [Obsolete("Use WriteError", false)]
    public Action<string, SequencePoint?> LogErrorPoint { get; set; } = (_, _) => { };

    /// <summary>
    /// Handler for resolving <see cref="AssemblyDefinition"/>s.
    /// </summary>
    public Func<string, AssemblyDefinition?> ResolveAssembly { get; set; } = null!;

    /// <summary>
    /// The current <see cref="IAssemblyResolver"/>s.
    /// </summary>
    public IAssemblyResolver AssemblyResolver { get; set; } = null!;

    /// <summary>
    /// An instance of <see cref="Mono.Cecil.ModuleDefinition"/> for processing.
    /// </summary>
    public ModuleDefinition ModuleDefinition { get; set; } = null!;

    /// <summary>
    /// Commonly used <see cref="TypeReference"/>s.
    /// </summary>
    public TypeSystem TypeSystem { get; set; } = null!;

    /// <summary>
    /// The full path of the target assembly.
    /// </summary>
    public string AssemblyFilePath { get; set; } = null!;

    /// <summary>
    /// The full directory path of the target project.
    /// A copy of $(MSBuildProjectDirectory).
    /// </summary>
    public string ProjectDirectoryPath { get; set; } = null!;

    /// <summary>
    /// The full file path of the target project.
    /// A copy of $(MSBuildProjectFullPath).
    /// </summary>
    public string ProjectFilePath { get; set; } = null!;

    /// <summary>
    /// The full directory path of the XML documentation file,
    /// if generating the documentation file is enabled in the project.
    /// A copy of @(DocFileItem->'%(FullPath)').
    /// </summary>
    public string? DocumentationFilePath { get; set; }

    /// <summary>
    /// The full directory path of the current weaver.
    /// </summary>
    public string AddinDirectoryPath { get; set; } = null!;

    /// <summary>
    /// The full directory path of the current solution.
    /// A copy of `$(SolutionDir)` or, if it does not exist, a copy of `$(MSBuildProjectDirectory)..\..\..\`. OPTIONAL
    /// </summary>
    public string SolutionDirectoryPath { get; set; } = null!;

    /// <summary>
    /// A semicolon delimited string that contains
    /// all the references for the target project.
    /// A copy of the contents of the @(ReferencePath).
    /// </summary>
    public string References { get; set; } = null!;

    /// <summary>
    /// A list of all the references marked as copy-local.
    /// A copy of the contents of the @(ReferenceCopyLocalPaths).
    /// </summary>
    /// <remarks>
    /// This list will be actively synced back to the build system, i.e. adding or removing items from this list will modify the @(ReferenceCopyLocalPaths) list of the current build.
    /// </remarks>
    public List<string> ReferenceCopyLocalPaths { get; set; } = new();

    /// <summary>
    /// A list of all the runtime references marked as copy-local.
    /// A copy of the contents of the @(RuntimeCopyLocalItems).
    /// </summary>
    /// <remarks>
    /// This list will be actively synced back to the build system, i.e. adding or removing items from this list will modify the @(RuntimeCopyLocalItems) list of the current build.
    /// </remarks>
    public List<string> RuntimeCopyLocalPaths { get; set; } = new();

    /// <summary>
    /// A list of all the msbuild constants.
    /// A copy of the contents of the $(DefineConstants).
    /// </summary>
    public List<string> DefineConstants { get; set; } = new();

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
    /// Find a <see cref="TypeDefinition"/>.
    /// Uses all assemblies listed from calling <see cref="GetAssembliesForScanning"/> on all weavers.
    /// </summary>
    public TypeDefinition FindTypeDefinition(string name) =>
        FindType(name);

    /// <summary>
    /// Handler for searching for a type.
    /// Uses all assemblies listed from calling <see cref="GetAssembliesForScanning"/> on all weavers.
    /// </summary>
    [Obsolete("Use FindTypeDefinition", false)]
    public Func<string, TypeDefinition> FindType { get; set; } = _ => throw new WeavingException($"{nameof(FindType)} has not been set.");

    /// <summary>
    /// Find a <see cref="TypeDefinition"/>.
    /// Uses all assemblies listed from calling <see cref="GetAssembliesForScanning"/> on all weavers.
    /// </summary>
    public bool TryFindTypeDefinition(string name, [NotNullWhen(true)] out TypeDefinition? type) =>
        TryFindType(name, out type);

    /// <summary>
    /// Handler for searching for a type.
    /// Uses all assemblies listed from calling <see cref="GetAssembliesForScanning"/> on all weavers.
    /// </summary>
    [Obsolete("Use TryFindTypeDefinition", false)]
    public TryFindTypeFunc TryFindType { get; set; } = (string _, out TypeDefinition? type) => throw new WeavingException($"{nameof(TryFindType)} has not been set.");

    /// <summary>
    /// Called after all weaving has occurred and the module has been saved.
    /// </summary>
    public virtual void AfterWeaving()
    {
    }

    /// <summary>
    /// Set to true if the reference to an equally named library (same name as this fody addin with the ending ".Fody" trimmed)
    /// should be removed by Fody after a successful execution
    /// </summary>
    public virtual bool ShouldCleanReference => false;
}