# Weaving Task Options

## AssemblyPath 

The path to the assembly to process. 

*Required*

    <Fody.WeavingTask ... AssemblyPath="@(IntermediateAssembly)" />

## SolutionDir 

The path to the directory containing the solution. 

*Required*

    <Fody.WeavingTask ... SolutionDir="$(SolutionDir)" />

## ProjectPath 

The path to the current project file. 

*Required*

    <Fody.WeavingTask ... ProjectPath="$(ProjectPath)" />

## References 

A comma separated list of paths to assembly references. 

*Required*

    <Fody.WeavingTask ... References="@(ReferencePath)" />

## AddinSearchPaths 

A comma separated list of directories to search for Addins. 

*Optional.* See: [AddinSearchPaths]

    <Fody.WeavingTask ... AddinSearchPaths="$(SolutionDir)FodyAddins" />

## KeyFilePath 

The path to the strong name key of the assembly. 

*Optional.* Uses the 'AssemblyOriginatorKeyFile' and 'SignAssembly' nodes of the target project to attempt to derive the path.

    <Fody.WeavingTask ... KeyFilePath="$(SolutionDir)MyKeyFil.snk" />

## MessageImportance 

Sets the [http://msdn.microsoft.com/en-us/library/microsoft.build.framework.messageimportance.aspx MessageImportance] to use when logging MSBuild messages. This is useful to control the verbosity of diagnostics outputted by MSBuild. 

When building using Visual Studio the default verbosity is `Minimal`. This means that if you set `WeavingTask.MessageImportance` to `Normal` or `Low` nothing will be written the the build output. 

You can also control the MSBuild verbosity from [Visual Studio](http://saraford.net/2008/10/07/did-you-know-you-can-configure-the-msbuild-verbosity-in-the-output-window-329/). 

Note: this setting does not effect the logging of errors or warnings. 

*Optional. Defaults to `Low`*

*Allowed values: `High`, `Normal` or `Low`*

For example

    <Fody.WeavingTask ... MessageImportance="Normal"/>
