# Addin development

This page uses the a sample addin called BasicFodyAddin to describe building an addin.

 * [NuGet Package](https://www.nuget.org/packages/BasicFodyAddin.Fody/)
 * [Source](/BasicFodyAddin/)


## Lib/Reference project

[BasicFodyAddin.csproj](/BasicFodyAddin/BasicFodyAddin/)

 * Contain all classes to control the addin behavior at compile time or provide intellisense to consumers. Often this is in the form of [Attributes](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/).
 * Generally any usage and reference to this project is removed at compile time so it is not needed as part of application deployment.
 * The target frameworks depends on what targets the weaver can support (see [Supported Runtimes And Ide](supported-runtimes-and-ide.md)).

This project is also used to produce the NuGet package. To achieve this the project consumes two NuGets:

 * [Fody](https://www.nuget.org/packages/Fody/) with `PrivateAssets="None"`. This results in producing NuGet package having a dependency on Fody with all `include="All"` in the nuspec. Note that while this project consumes the Fody NuGet, weaving is not performed on this project. This is due to the FodyPackaging NuGet (see below) including `<DisableFody>true</DisableFody>` in the MSBuild pipeline.
 * [FodyPackaging](addin-packaging.md#fodypackaging-nuget-package) with `PrivateAssets="All"`. This results in a NuGet package being produced by this project, but no dependency on FodyPackaging in the resulting NuGet package.

The produced NuGet package will:

 * Be named with `.Fody` suffix. This project should also contain all appropriate [NuGet metadata properties](https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#nuget-metadata-properties). Many of these properties have defaults in [FodyPackaging](https://github.com/Fody/Fody/blob/master/FodyPackaging/build/FodyPackaging.props), but can be overridden.
 * Target, and hence support from a consumer perspective, the same frameworks that this project targets. 
 * Be created in a directory named `nugets` at the root of the solution.

snippet: BasicFodyAddin.csproj


### Build Order

The Lib/Reference project must contain a [Project Dependency](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-create-and-remove-project-dependencies?view=vs-2019) on the [Weaver-Project](Weaver-Project) to ensure it is built after the Weaver Project produces its output.

![project dependencies](project-dependencies.png)

If a weaver file cannot be found, the build will fail with one of the following:

> FodyPackaging: No weaver found at [PATH]. BasicFodyAddin should have a Project Dependency on BasicFodyAddin.Fody.


## Weaver Project

[BasicFodyAddin.Fody.csproj](/BasicFodyAddin/BasicFodyAddin.Fody/)

This project contains the weaving code.

 * Has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/).
 * Should not have any runtime dependencies (excluding Mono Cecil); runtime dependencies should be combined using e.g. [ILMerge](https://github.com/dotnet/ILMerge) and the `/Internalize` flag.
 * The assembly must contain a public class named 'ModuleWeaver'. The namespace does not matter.

snippet: BasicFodyAddin.Fody.csproj


### Target Frameworks

This project must target `netstandard2.0`.


### Output of the project

It outputs a file named `BasicFodyAddin.Fody`. The '.Fody' suffix is necessary to be picked up by Fody at compile time.


### ModuleWeaver

ModuleWeaver.cs is where the target assembly is modified. Fody will pick up this type during its processing. Note that the class must be named as `ModuleWeaver`.

`ModuleWeaver` must use the base class of `BaseModuleWeaver` which exists in the [FodyHelpers NuGet](https://www.nuget.org/packages/FodyHelpers/).

 * Inherit from `BaseModuleWeaver`.
 * The class must be public, non static, and not abstract.
 * Have an empty constructor.

snippet: ModuleWeaver


#### BaseModuleWeaver.Execute

Called to perform the manipulation of the module. The current module can be accessed and manipulated via `BaseModuleWeaver.ModuleDefinition`.

snippet: Execute


#### BaseModuleWeaver.GetAssembliesForScanning

Called by Fody when it is building up a type cache for lookups. This method should return all possible assemblies that the weaver may require while resolving types. In this case BasicFodyAddin requires `System.Object`, so `GetAssembliesForScanning` returns `netstandard` and `mscorlib`. It is safe to return assembly names that are not used by the current target assembly as these will be ignored.

To use this type cache, a `ModuleWeaver` can call `BaseModuleWeaver.FindType` within `Execute` method.

snippet: GetAssembliesForScanning


#### BaseModuleWeaver.ShouldCleanReference

When `BasicFodyAddin.dll` is referenced by a consuming project, it is only for the purposes configuring the weaving via attributes. As such, it is not required at runtime. With this in mind `BaseModuleWeaver` has an opt in feature to remove the reference, meaning the target weaved application does not need `BasicFodyAddin.dll` at runtime. This feature can be opted in to via the following code in `ModuleWeaver`:

snippet: ShouldCleanReference


#### Other BaseModuleWeaver Members

`BaseModuleWeaver` has a number of other members for logging and extensibility:
https://github.com/Fody/Fody/blob/master/FodyHelpers/BaseModuleWeaver.cs


### Resultant injected code

In this case a new type is being injected into the target assembly that looks like this.

```csharp
public class Hello
{
    public string World()
    {
        return "Hello World";
    }
}
```


### Throwing exceptions

When writing an addin there are a points to note when throwing an Exception.

 * Exceptions thrown from an addin will be caught and interpreted as a build error. So this will stop the build.
 * The exception information will be logged to the MSBuild `BuildEngine.LogErrorEvent` method.
 * If the exception type is `WeavingException` then it will be interpreted as an "error". So the addin is explicitly throwing an exception with the intent of stopping processing and logging a simple message to the build log. In this case the message logged will be the contents of `WeavingException.Message` property. If the `WeavingException` has a property `SequencePoint` then that information will be passed to the build engine so a user can navigate to the error.
 * If the exception type is *not* a `WeavingException` then it will be interpreted as an "unhandled exception". So something has gone seriously wrong with the addin. It most likely has a bug. In this case message logged be much bore verbose and will contain the full contents of the Exception. The code for getting the message can be found here in [ExceptionExtensions](https://github.com/Fody/Fody/blob/master/FodyCommon/ExceptionExtensions.cs).


## Passing config via to FodyWeavers.xml

This file exists at a project level in the users target project and is used to pass configuration to the 'ModuleWeaver'.

So if the FodyWeavers.xml file contains the following:

```xml
<Weavers>
  <BasicFodyAddin Namespace="MyNamespace"/>
</Weavers>
```

The property of the `ModuleWeaver.Config` will be an [XElement](https://docs.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) containing:

```xml
<BasicFodyAddin Namespace="MyNamespace"/>
```


### Supporting intellisense for FodyWeavers.xml

Fody will create or update a schema file (FodyWeavers.xsd) for every FodyWeavers.xml during compilation, adding all detected weavers. Every weaver now can provide a schema fragment describing it's individual properties and content that can be set. This file must be part of the weaver project and named `<project name>.xcf`. It contains the element describing the type of the configuration node. The file must be published side by side with the weaver file; however [FodyPackaging](fodypackaging.md) will configure this correctly based on the convention `WeaverName.Fody.xcf`.

Sample content of the `BasicFodyAddin.Fody.xcf`:

snippet: BasicFodyAddin.Fody.Xcf

Fody will then combine all `.xcf` fragments with the weavers information to the final `.xsd`:

snippet: FodyWeavers.xsd


## AssemblyToProcess Project

[AssemblyToProcess.csproj](/BasicFodyAddin/AssemblyToProcess/)

A target assembly to process and then validate with unit tests.


## Tests Project

[Tests.csproj](/BasicFodyAddin/Tests/)

Contains all tests for the weaver.

The project has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/).

It has a reference to the `AssemblyToProcess` project, so that `AssemblyToProcess.dll` is copied to the bin directory of the test project.

FodyHelpers contains a utility [WeaverTestHelper](https://github.com/Fody/Fody/blob/master/FodyHelpers/Testing/WeaverTestHelper.cs) for executing test runs on a target assembly using a ModuleWeaver.

A test can then be run as follows:

snippet: WeaverTests

By default `ExecuteTestRun` will perform a [PeVerify](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool) on the resultant assembly.

snippet: Tests.csproj


## Build Server


### AppVeyor

To configure an adding to build using [AppVeyor](https://www.appveyor.com/) use the following `appveyor.yml`:

snippet: appveyor.yml


## Usage


### NuGet installation

Install the [BasicFodyAddin.Fody NuGet package](https://www.nuget.org/packages/BasicFodyAddin.Fody/) and update the [Fody NuGet package](https://www.nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package BasicFodyAddin.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

```xml
<Weavers>
  <BasicFodyAddin />
</Weavers>
```


## Deployment

Addins are deployed through [NuGet](https://www.nuget.org/) packages. The package must:

 * Contain two weaver assemblies, one in each of the folders `netclassicweaver` and `netstandardweaver`, to support both .Net Classic and .Net Core.
 * Contain a runtime library, compiled for every supported framework, under the `lib` folder.
 * Contain an MsBuild .props file in the `build` folder that registers the weaver at compile time. The name of the file must be  the package id with the `.props` extension. See [Addin Discover](addin-discovery.md) for details.
 * Have an id with the same name of the weaver assembly should be the same and be suffixed with ".Fody". So in this case the [BasicFodyAddin.Fody NuGet](https://www.nuget.org/packages/BasicFodyAddin.Fody/) contains the weaver assembly `BasicFodyAddin.Fody.dll` and the reference assembly `BasicFodyAddin.dll`.
 * Have a single dependency on **only** the [Fody NuGet package](https://www.nuget.org/packages/Fody/). **Do not add any other NuGet dependencies as Fody does not support loading these files at compile time.**

Note that the addins used via [in-solution-weaving](in-solution-weaving.md) are handled differently.