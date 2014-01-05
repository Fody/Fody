![Icon](https://raw.github.com/Fody/Fody/master/Icons/package_icon.png)

## Extensible tool for weaving .net assemblies

## Introduction 

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model. 

## The nuget package

https://nuget.org/packages/Fody/

## Why? 

This technique of "weaving" in new instructions is fantastically powerful. You can turn simple public properties into full [INotifyPropertyChanged implementations](https://github.com/Fody/PropertyChanged), add [checks for null arguments](https://github.com/Fody/NullGuard), add [Git hashes to your Assemblies](https://github.com/Fody/Stamp), even [make all your string comparisons case insensitive](https://github.com/Fody/Caseless). 

### Note: NotifyPropertyWeaver

Users of the [NotifyPropertyWeaver](https://github.com/SimonCropp/NotifyPropertyWeaver) extension who are migrating to [Fody](https://github.com/Fody/fody) will want to use NuGet to Install the PropertyChanged.Fody package along with Fody itself to get the same functionality as before. This is because Fody is a general purpose weaver with plugins while NotifyPropertyWeaver was specific to one scenario. That scenario now lives in the [PropertyChanged addin](https://github.com/Fody/PropertyChanged). See [Converting from NotifyPropertyWeaver](https://github.com/Fody/PropertyChanged/wiki/ConvertingFromNotifyPropertyWeaver) for more information. 

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
  * [AssertMessage](https://github.com/Fody/AssertMessage) Generates 'message' from sourcecode and adds it to assertion.
  * [AsyncErrorHandler](https://github.com/Fody/AsyncErrorHandler) Integrates error handling into async and TPL code.
  * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
  * [Caseless](https://github.com/Fody/Caseless) Change string comparisons to be case insensitive.
  * [Catel](https://github.com/Catel/Catel.Fody) For transforming automatic properties into [Catel](https://github.com/Catel/Catel) properties.
  * [Commander](https://github.com/DamianReeves/Commander.Fody) Injects ICommand properties and implementations for use in MVVM applications.
  * [Costura](https://github.com/Fody/Costura/) For embedding references as resources.
  * [DependencyInjection](https://github.com/jorgehmv/FodyDependencyInjection) automatic dependency injection for [Ninject](http://www.ninject.org/), [Autofac](http://autofac.org/) and [Spring](http://www.springframework.net/).
  * [EmptyConstructor](https://github.com/Fody/EmptyConstructor) Adds an empty constructor to classes even if a non empty one is defined.
  * [EmptyStringGuard](https://github.com/thirkcircus/EmptyStringGuard) Adds empty string argument checks to an assembly.
  * [EnableFaking](https://github.com/philippdolder/EnableFaking.Fody) Allows faking your types without writing interfaces for testing purposes only.
  * [Equals] (https://github.com/Fody/Equals) Generate Equals, GetHashCode and operators methods from properties.
  * [ExtraConstraints](https://github.com/Fody/ExtraConstraints) Facilitates adding constraints for Enum and Delegate to types and methods.
  * [Fielder](https://github.com/Fody/Fielder) Converts public fields to public properties.
  * [Freezable](https://github.com/Fody/Freezable) Implements the Freezable pattern.
  * [InfoOf](https://github.com/Fody/InfoOf) Provides `methodof`, `propertyof` and `fieldof` equivalents of [`typeof`](http://msdn.microsoft.com/en-us/library/58918ffs.aspx).
  * [Ionad](https://github.com/Fody/Ionad) Replaces static method calls. 
  * [Janitor](https://github.com/Fody/Janitor) Simplifies the implementation of [IDisposable](http://msdn.microsoft.com/en-us/library/system.idisposable.aspx).
  * [JetBrainsAnnotations](https://github.com/Fody/JetBrainsAnnotations) Modifies an assembly so you can leverage JetBrains Annotations but don't need to deploy JetBrainsAnnotations.dll. 
  * [MethodCache](https://github.com/Dresel/MethodCache) Caches return values of methods decorated with a `CacheAttribute`.
  * [MethodDecorator](http://github.com/Fody/MethodDecorator) Decorate arbitrary methods to run code before and after invocation.
  * [MethodTimer](https://github.com/Fody/MethodTimer) Injects method timing code.
  * [Mixins](https://bitbucket.org/skwasiborski/mixins.fody/wiki/Home) A mixin is a class that provides a certain functionality to be inherited or just reused by a subclass.
  * [ModuleInit](https://github.com/Fody/ModuleInit) Adds a module initializer to an assembly.
  * [Mutable](https://github.com/ndamjan/Mutable.Fody) Use this addin for F# to make setters for union types and eliminate need for `CLIMutable` attribute for records.
  * [NullGuard](https://github.com/Fody/NullGuard) Adds null argument checks to an assembly
  * [Obsolete](https://github.com/Fody/Obsolete) Helps keep usages of [ObsoleteAttribute]([http://msdn.microsoft.com/en-us/library/fwz0y5c2 ) consistent.
  * [PropertyChanged](https://github.com/Fody/PropertyChanged) Injects INotifyPropertyChanged code into properties.
  * [PropertyChanging](https://github.com/Fody/PropertyChanging) Injects INotifyPropertyChanging code into properties.
  * [Publicize](https://github.com/Fody/Publicize) Converts non-public members to public hidden members.
  * [RemoveReference](https://github.com/icnocop/RemoveReference.Fody) Facilitates removing references in a compiled assembly during a build.
  * [Resourcer](https://github.com/Fody/Resourcer) Simplifies reading embedded resources from an Assembly.
  * [Scalpel](https://github.com/Fody/Scalpel) Strips tests from an assembly. 
  * [Spring](https://github.com/jorgehmv/FodySpring) Spring constructor configuration. 
  * [Stamp](https://github.com/Fody/Stamp) Stamps an assembly with git data.
  * [StampSvn](https://github.com/krk/Stamp) Stamps an assembly with svn data.
  * [Stiletto](https://github.com/benjamin-bader/stiletto) Compile-time static analysis and optimization for the Stiletto IoC library.  
  * [ToString](https://github.com/Fody/ToString) Generate ToString method from public properties.
  * [Usable](https://github.com/Fody/Usable) Adds using statements for local variables that have been created, and implement [IDisposable](http://msdn.microsoft.com/en-au/library/system.idisposable.aspx).
  * [Validar](https://github.com/Fody/Validar) Injects [IDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx ) code into a class at compile time.
  * [Visualize](https://github.com/Fody/Visualize) Adds debugger attributes to help visualize objects.
  * [Virtuosity](https://github.com/Fody/Virtuosity) Change all members to virtual.
    
## Icon

<a href="http://thenounproject.com/noun/bird/#icon-No6726" target="_blank">Bird</a> designed by <a href="http://thenounproject.com/MARCOHS" target="_blank">Marco Hernandez</a> from The Noun Project

## More Info

 * [AddinSearchPaths](https://github.com/Fody/Fody/wiki/AddinSearchPaths)
 * [DeployingAddinsAsNugets](https://github.com/Fody/Fody/wiki/DeployingAddinsAsNugets)
 * [Home](https://github.com/Fody/Fody/wiki/Home)
 * [HowToWriteAnAddin](https://github.com/Fody/Fody/wiki/HowToWriteAnAddin)
 * [InSolutionWeaving](https://github.com/Fody/Fody/wiki/InSolutionWeaving)
 * [ModuleWeaver](https://github.com/Fody/Fody/wiki/ModuleWeaver)
 * [PdbReWritingAndDebugging](https://github.com/Fody/Fody/wiki/PdbReWritingAndDebugging)
 * [SampleUsage](https://github.com/Fody/Fody/wiki/SampleUsage)
 * [Setup](https://github.com/Fody/Fody/wiki/Setup)
 * [SignedAssemblies](https://github.com/Fody/Fody/wiki/SignedAssemblies)
 * [SupportedRuntimesAndIde](https://github.com/Fody/Fody/wiki/SupportedRuntimesAndIde)
 * [TaskAddsAFlagInterface](https://github.com/Fody/Fody/wiki/TaskAddsAFlagInterface)
 * [TaskCouldNotBeLoaded](https://github.com/Fody/Fody/wiki/TaskCouldNotBeLoaded)
 * [WeavingTaskOptions](https://github.com/Fody/Fody/wiki/WeavingTaskOptions)
 * [ThrowingAnExceptionFromAnAddin](https://github.com/Fody/Fody/wiki/ThrowingAnExceptionFromAnAddin)
 * [Mono Support](https://github.com/Fody/Fody/wiki/Mono)

## With thanks to

### Resharper from Jetbrains

http://www.jetbrains.com/resharper/

![Resharper.png](https://raw.github.com/wiki/Fody/Fody/Resharper.png)


### TeamCity from Jetbrains

http://www.jetbrains.com/TeamCity/

![TeamCity.png](https://raw.github.com/wiki/Fody/Fody/TeamCity.png)
