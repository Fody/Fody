## Extensible tool for weaving .net assemblies

## Introduction 

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to elimination that plumbing code through an extensible add-in model. 

## Why? 

This technique of "weaving" in new instructions is fantastically powerful. You can turn simple public properties into full [INotifyPropertyChanged implementations](https://github.com/SimonCropp/PropertyChanged), add [checks for null arguments](https://github.com/SimonCropp/NullGuard.Fody), add [Git hashs to your Assemblies](https://github.com/SimonCropp/Stamp), even [make all your string comparisons case insensitive](https://github.com/SimonCropp/Caseless). 

### Note: NotifyPropertyWeaver

Users of the [NotifyPropertyWeaver](https://github.com/SimonCropp/NotifyPropertyWeaver) extension who are migrating to [Fody](https://github.com/SimonCropp/fody) will want to use NuGet to Install the PropertyChanged.Fody package along with Fody itself to get the same functionality as before. This is because Fody is a general purpose weaver with plugins while NotifyPropertyWeaver was specific to one scenario. That scenario now lives in the [PropertyChanged addin](https://github.com/SimonCropp/PropertyChanged). See [Converting from NotifyPropertyWeaver](https://github.com/SimonCropp/PropertyChanged/wiki/ConvertingFromNotifyPropertyWeaver) for more information 

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

See [SampleUsage](https://github.com/SimonCropp/Fody/wiki/SampleUsage) for an introduction on using Fody.

## Naming

The name "Fody" comes from the small birds that belong to the weaver family [Ploceidae](http://en.wikipedia.org/wiki/Fody).

## Tools and Products Used 

 * [JetBrains dotTrace](http://www.jetbrains.com/profiler/)
 * [JetBrains Resharper](http://www.jetbrains.com/resharper/)
 * [Mono Cecil](http://www.mono-project.com/Cecil)

## Samples

 * [BasicFodyAddin](https://github.com/SimonCropp/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
 * [FodyAddinSamples](https://github.com/SimonCropp/FodyAddinSamples) is a single solution that contains a working copy of every fody addin.

## More Info

 * [AddinSearchPaths](https://github.com/SimonCropp/Fody/wiki/AddinSearchPaths)
 * [AddinsList](https://github.com/SimonCropp/Fody/wiki/AddinsList)
 * [DeployingAddinsAsNugets](https://github.com/SimonCropp/Fody/wiki/DeployingAddinsAsNugets)
 * [Home](https://github.com/SimonCropp/Fody/wiki/Home)
 * [HowToWriteAnAddin](https://github.com/SimonCropp/Fody/wiki/HowToWriteAnAddin)
 * [InSolutionWeaving](https://github.com/SimonCropp/Fody/wiki/InSolutionWeaving)
 * [ModuleWeaver](https://github.com/SimonCropp/Fody/wiki/ModuleWeaver)
 * [PdbReWritingAndDebugging](https://github.com/SimonCropp/Fody/wiki/PdbReWritingAndDebugging)
 * [ReleaseNotes](https://github.com/SimonCropp/Fody/wiki/ReleaseNotes)
 * [SampleUsage](https://github.com/SimonCropp/Fody/wiki/SampleUsage)
 * [Setup](https://github.com/SimonCropp/Fody/wiki/Setup)
 * [SignedAssemblies](https://github.com/SimonCropp/Fody/wiki/SignedAssemblies)
 * [SupportedRuntimesAndIde](https://github.com/SimonCropp/Fody/wiki/SupportedRuntimesAndIde)
 * [TaskAddsAFlagInterface](https://github.com/SimonCropp/Fody/wiki/TaskAddsAFlagInterface)
 * [TaskCouldNotBeLoaded](https://github.com/SimonCropp/Fody/wiki/TaskCouldNotBeLoaded)
 * [VSIXPackage](https://github.com/SimonCropp/Fody/wiki/VSIXPackage)
 * [WeavingTaskOptions](https://github.com/SimonCropp/Fody/wiki/WeavingTaskOptions)
 * [Mono Support](https://github.com/SimonCropp/Fody/wiki/Mono)

## With thanks to

### Resharper from Jetbrains

http://www.jetbrains.com/resharper/

![Resharper.png](https://raw.github.com/wiki/SimonCropp/Fody/Resharper.png)


### TeamCity from Jetbrains

http://www.jetbrains.com/TeamCity/

![TeamCity.png](https://raw.github.com/wiki/SimonCropp/Fody/TeamCity.png)

### Codebetter

For supplying a build server

http://codebetter.com/

![CodeBetter.png](https://raw.github.com/wiki/SimonCropp/Fody/CodeBetter.png)
