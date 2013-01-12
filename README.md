## Extensible tool for weaving .net assemblies

## Introduction 

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to elimination that plumbing code through an extensible add-in model. 

## Why? 

This technique of "weaving" in new instructions is fantastically powerful. You can turn simple public properties into full [INotifyPropertyChanged implementations](https://github.com/Fody/PropertyChanged), add [checks for null arguments](https://github.com/Fody/NullGuard.Fody), add [Git hashs to your Assemblies](https://github.com/Fody/Stamp), even [make all your string comparisons case insensitive](https://github.com/Fody/Caseless). 

### Note: NotifyPropertyWeaver

Users of the [NotifyPropertyWeaver](https://github.com/SimonCropp/NotifyPropertyWeaver) extension who are migrating to [Fody](https://github.com/Fody/fody) will want to use NuGet to Install the PropertyChanged.Fody package along with Fody itself to get the same functionality as before. This is because Fody is a general purpose weaver with plugins while NotifyPropertyWeaver was specific to one scenario. That scenario now lives in the [PropertyChanged addin](https://github.com/Fody/PropertyChanged). See [Converting from NotifyPropertyWeaver](https://github.com/SimonCropp/PropertyChanged/wiki/ConvertingFromNotifyPropertyWeaver) for more information 

## The plumbing tasks Fody handles 

  * Injection of the MSBuild task into the build pipeline
  * Resolving the location of the assembly and pdb
  * Abstracts the complexities of logging to MSBuild
  * Reads the assembly and pdb into the Mono.Cecil object model
  * Re-applying the strong name if necessary
  * Saving the assembly and pdb

Fody Uses [Mono.Cecil](http://www.mono-project.com/Cecil)  and an add-in based approach to modifying the IL of .net assemblies at compile time.

 * No install required to build
 * No attributes required
 * No references required
 * Supports .net 3.5, .net 4, .net 4.5, Silverlight 4, Silverlight 5, Windows Phone 7, Windows Phone 8, Metro on Windows 8, Mono, MonoTouch, MonoDroid and PCL

## Visual Studio Addin

There is a  [Visual Studio addin](http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a) 

## Usage

See [SampleUsage](https://github.com/Fody/Fody/wiki/SampleUsage) for an introduction on using Fody.

## Naming

The name "Fody" comes from the small birds that belong to the weaver family [Ploceidae](http://en.wikipedia.org/wiki/Fody).

## Tools and Products Used 

 * [JetBrains dotTrace](http://www.jetbrains.com/profiler/)
 * [JetBrains Resharper](http://www.jetbrains.com/resharper/)
 * [Mono Cecil](http://www.mono-project.com/Cecil)

## Samples

 * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
 * [FodyAddinSamples](https://github.com/Fody/FodyAddinSamples) is a single solution that contains a working copy of every fody addin.

## Addins List

  * [Anotar](https://github.com/Fody/Anotar) Simplifies logging through a static class and some IL manipulation.
  * [AsyncErrorHandling](https://github.com/shiftkey/Fody.AsyncErrorHandling) Integrates error handling into async and TPL code.
  * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
  * [Caseless](https://github.com/Fody/Caseless) Change string comparisons to be case insensitive.
  * [Catel](http://catelfody.codeplex.com/) For transforming automatic properties into [Catel](http://catel.codeplex.com/) properties.
  * [EmptyConstructor](https://github.com/Fody/EmptyConstructor) Adds an empty constructor to classes even if a non empty one is defined.
  * [ExtraConstraints](https://github.com/Fody/ExtraConstraints) Facilitates adding constraints for Enum and Delegate to types and methods.
  * [Fielder](https://github.com/Fody/Fielder) Converts public fields to public properties.
  * [Freezable](https://github.com/Fody/Freezable) Implements the Freezable pattern.
  * [InfoOf](https://github.com/Fody/InfoOf) Provides `methodof`, `propertyof` and `fieldof` equivalents of [`typeof`](http://msdn.microsoft.com/en-us/library/58918ffs.aspx) .
  * [JetBrainsAnnotations](https://github.com/Fody/JetBrainsAnnotations.Fody) Modifies an assembly so you can leverage JetBrains Annotations but don't need to deploy JetBrainsAnnotations.dll. 
  * [MethodDecorator](http://github.com/citizenmatt/MethodDecorator.Fody) Decorate arbitrary methods to run code before and after invocation.
  * [MethodTimer](https://github.com/Fody/MethodTimer) Injects method timing code.
  * [Mixins](https://bitbucket.org/skwasiborski/mixins.fody/wiki/Home) A mixin is a class that provides a certain functionality to be inherited or just reused by a subclass.
  * [ModuleInit](https://github.com/Fody/ModuleInit) Adds a module initializer to an assembly.
  * [NullGuard](https://github.com/Fody/NullGuard.Fody) Adds null argument checks to an assembly
  * [Obsolete](https://github.com/Fody/Obsolete) Helps keep usages of [ObsoleteAttribute]([http://msdn.microsoft.com/en-us/library/fwz0y5c2 ) consistent.
  * [PropertyChanged](https://github.com/Fody/PropertyChanged) Injects INotifyPropertyChanged code into properties.
  * [PropertyChanging](https://github.com/Fody/PropertyChanging) Injects INotifyPropertyChanging code into properties.
  * [Publicize](https://github.com/Fody/Publicize) Converts non-public members to public hidden members.
  * [Stamp](https://github.com/Fody/Stamp) Stamps an assembly with git data.
  * [Validar](https://github.com/Fody/Validar) Injects [IDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx ) code into a class at compile time.
  * [Virtuosity](https://github.com/Fody/Virtuosity) Change all members to virtual.

## More Info

 * [AddinSearchPaths](https://github.com/Fody/Fody/wiki/AddinSearchPaths)
 * [DeployingAddinsAsNugets](https://github.com/Fody/Fody/wiki/DeployingAddinsAsNugets)
 * [Home](https://github.com/Fody/Fody/wiki/Home)
 * [HowToWriteAnAddin](https://github.com/Fody/Fody/wiki/HowToWriteAnAddin)
 * [InSolutionWeaving](https://github.com/Fody/Fody/wiki/InSolutionWeaving)
 * [ModuleWeaver](https://github.com/Fody/Fody/wiki/ModuleWeaver)
 * [PdbReWritingAndDebugging](https://github.com/Fody/Fody/wiki/PdbReWritingAndDebugging)
 * [ReleaseNotes](https://github.com/Fody/Fody/wiki/ReleaseNotes)
 * [SampleUsage](https://github.com/Fody/Fody/wiki/SampleUsage)
 * [Setup](https://github.com/Fody/Fody/wiki/Setup)
 * [SignedAssemblies](https://github.com/Fody/Fody/wiki/SignedAssemblies)
 * [SupportedRuntimesAndIde](https://github.com/Fody/Fody/wiki/SupportedRuntimesAndIde)
 * [TaskAddsAFlagInterface](https://github.com/Fody/Fody/wiki/TaskAddsAFlagInterface)
 * [TaskCouldNotBeLoaded](https://github.com/Fody/Fody/wiki/TaskCouldNotBeLoaded)
 * [VSIXPackage](https://github.com/Fody/Fody/wiki/VSIXPackage)
 * [WeavingTaskOptions](https://github.com/Fody/Fody/wiki/WeavingTaskOptions)
 * [Mono Support](https://github.com/Fody/Fody/wiki/Mono)

## With thanks to

### Resharper from Jetbrains

http://www.jetbrains.com/resharper/

![Resharper.png](https://raw.github.com/wiki/Fody/Fody/Resharper.png)


### TeamCity from Jetbrains

http://www.jetbrains.com/TeamCity/

![TeamCity.png](https://raw.github.com/wiki/Fody/Fody/TeamCity.png)

### Codebetter

For supplying a build server

http://codebetter.com/

![CodeBetter.png](https://raw.github.com/wiki/Fody/Fody/CodeBetter.png)
