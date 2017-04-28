![Icon](https://raw.github.com/Fody/Fody/master/Icons/package_icon.png)


## Extensible tool for weaving .net assemblies 

[![Join the chat at https://gitter.im/Fody/Fody](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Fody/Fody?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


## Introduction

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model.


## The nuget package  [![NuGet Status](https://img.shields.io/nuget/v/Fody.svg?style=flat)](https://www.nuget.org/packages/Fody/)

https://www.nuget.org/packages/Fody/

    PM> Install-Package Fody


## Why? 

This technique of "weaving" in new instructions is fantastically powerful. You can turn simple public properties into full [INotifyPropertyChanged implementations](https://github.com/Fody/PropertyChanged), add [checks for null arguments](https://github.com/Fody/NullGuard), add [Git hashes to your Assemblies](https://github.com/Fody/Stamp), even [make all your string comparisons case insensitive](https://github.com/Fody/Caseless).


## The plumbing tasks Fody handles

  * Injection of the MSBuild task into the build pipeline
  * Resolving the location of the assembly and pdb
  * Abstracts the complexities of logging to MSBuild
  * Reads the assembly and pdb into the Mono.Cecil object model
  * Re-applying the strong name if necessary
  * Saving the assembly and pdb

Fody Uses [Mono.Cecil](http://www.mono-project.com/Cecil/)  and an add-in based approach to modifying the IL of .net assemblies at compile time.

 * No install required to build
 * No attributes required
 * No references required
 * Supports .net 3.5, .net 4, .net 4.5, .net 4.6, Silverlight 4, Silverlight 5, Windows Phone 7, Windows Phone 8, Metro on Windows 8, Mono, MonoTouch, MonoDroid and PCL 


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
  * [AutoDependencyProperty](http://blog.angeloflogic.com/2014/12/no-more-dependencyproperty-with.html) Generates WPF DependencyProperty boilerplate from automatic C# properties.
  * [AutoDI](https://github.com/Keboo/AutoDI) A framework for simplifying the creation of objects that only partially depend on resources in your favorite DI container.
  * [AutoLazy](https://github.com/bcuff/AutoLazy) Automatically implements the double-checked locking pattern on specified properties and methods.
  * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
  * [Caseless](https://github.com/Fody/Caseless) Change string comparisons to be case insensitive.
  * [Catel](https://github.com/Catel/Catel.Fody) For transforming automatic properties into [Catel](https://github.com/Catel/Catel) properties.
  * [Cauldron](https://github.com/Capgemini/Cauldron) Provides method, property and field interception. It also provides weavers for Cauldron.Core and Cauldron.Activator.
  * [ConfigureAwait](https://github.com/distantcam/ConfigureAwait) Allows you to set the async ConfigureAwait at a global level for all your await calls.
  * [Costura](https://github.com/Fody/Costura/) For embedding references as resources.
  * [CryptStr](https://cryptstr.codeplex.com/) Encrypts literal strings in your .NET assemblies.
  * [DependencyInjection](https://github.com/jorgehmv/FodyDependencyInjection) automatic dependency injection for [Ninject](http://www.ninject.org/), [Autofac](http://autofac.org/) and [Spring](http://www.springframework.net/).
  * [EmptyConstructor](https://github.com/Fody/EmptyConstructor) Adds an empty constructor to classes even if a non empty one is defined.
  * [EmptyStringGuard](https://github.com/thirkcircus/EmptyStringGuard) Adds empty string argument checks to an assembly.
  * [EnableFaking](https://github.com/philippdolder/EnableFaking.Fody) Allows faking your types without writing interfaces for testing purposes only.
  * [Equals](https://github.com/Fody/Equals) Generate Equals, GetHashCode and operators methods from properties.
  * [Expose](https://github.com/kedarvaidya/Expose.Fody) Exposes members and optionally implements interface of a field declared in class.
  * [ExtraConstraints](https://github.com/Fody/ExtraConstraints) Facilitates adding constraints for Enum and Delegate to types and methods.
  * [FactoryId](https://github.com/ramoneeza/FactoryId.Fody) Simplifies the implementation of Factory Method Pattern
  * [Fielder](https://github.com/Fody/Fielder) Converts public fields to public properties.
  * [FodyDependencyInjection](https://github.com/jorgehmv/FodyDependencyInjection) Dependency injection with Fody add-ins.
  * [Freezable](https://github.com/Fody/Freezable) Implements the Freezable pattern.
  * [InfoOf](https://github.com/Fody/InfoOf) Provides `methodof`, `propertyof` and `fieldof` equivalents of [`typeof`](https://msdn.microsoft.com/en-us/library/58918ffs.aspx).
  * [Ionad](https://github.com/Fody/Ionad) Replaces static method calls. 
  * [Janitor](https://github.com/Fody/Janitor) Simplifies the implementation of [IDisposable](https://msdn.microsoft.com/en-us/library/system.idisposable.aspx).
  * [LoadAssembliesOnStartup](https://github.com/Fody/LoadAssembliesOnStartup) Loads references on startup by using the types in the module initializer
  * [MethodCache](https://github.com/Dresel/MethodCache) Caches return values of methods decorated with a `CacheAttribute`.
  * [MethodDecorator](https://github.com/Fody/MethodDecorator) Decorate arbitrary methods to run code before and after invocation.
  * [MethodTimer](https://github.com/Fody/MethodTimer) Injects method timing code.
  * [ModuleInit](https://github.com/Fody/ModuleInit) Adds a module initializer to an assembly.
  * [Mutable](https://github.com/ndamjan/Mutable.Fody) Make F# setters for union types and eliminate need for `CLIMutable` attribute for records.
  * [Mvid](https://github.com/hmemcpy/Mvid.Fody) Adds the ability to specify the assembly MVID (Module Version Id).
  * [Nancy.ModelPostprocess](https://bitbucket.org/tpluscode/nancy.modelpostprocess) Modify Nancy models after route execution but before serialization
  * [NullGuard](https://github.com/Fody/NullGuard) Adds null argument checks to an assembly
  * [Obsolete](https://github.com/Fody/Obsolete) Helps keep usages of [ObsoleteAttribute]([https://msdn.microsoft.com/en-us/library/fwz0y5c2 ) consistent.
  * [Padded](https://github.com/Scooletz/Padded) Adds padding to fight the false sharing problem.
  * [PropertyChanged](https://github.com/Fody/PropertyChanged) Injects INotifyPropertyChanged code into properties.
  * [PropertyChanging](https://github.com/Fody/PropertyChanging) Injects INotifyPropertyChanging code into properties.
  * [Publicize](https://github.com/Fody/Publicize) Converts non-public members to public hidden members.
  * [QueryValidator](https://github.com/kamil-mrzyglod/QueryValidator.Fody) Validates your DB queries during a build.
  * [Realm](https://github.com/realm/realm-dotnet) Mobile database: a replacement for SQLite & ORMs.
  * [ReactiveUI](https://github.com/kswoll/ReactiveUI.Fody) Generates [ReactiveUI](http://reactiveui.net/) `RaisePropertyChange` notifications for properties and `ObservableAsPropertyHelper` properties.
  * [Resourcer](https://github.com/Fody/Resourcer) Simplifies reading embedded resources from an Assembly.
  * [RomanticWeb](http://romanticweb.net/) Fody weaver plugin for RomanticWeb instrumentation.
  * [SemVer](https://github.com/dittodhole/dotnet-SemVer.Stamp) versions your assemblies according to [SemVer](http://semver.org/) based on your CVS commit messages.
  * [Spring](https://github.com/jorgehmv/FodySpring) Spring constructor configuration. 
  * [StampSvn](https://github.com/krk/Stamp) Stamps an assembly with SVN data.
  * [StaticProxy](https://github.com/BrunoJuchli/StaticProxy.Fody) Helps proxy dynamic code emitting: Moq, FakeItEasy, Castle Dynamic, LinFu.
  * [SexyProxy](https://github.com/kswoll/sexy-proxy) Proxy generator with support for async patterns.
  * [SwallowExceptions](https://github.com/duaneedwards/SwallowExceptions) Swallow Exceptions in targeted methods.
  * [ToString](https://github.com/Fody/ToString) Generate `ToString` method from public properties.
  * [Tracer](https://github.com/csnemes/tracer) Adds trace-enter and trace-leave log entries for selected methods.
  * [Undisposed](https://github.com/ermshiperete/undisposed-fody) Debugging tool to track down undisposed objects.
  * [Usable](https://github.com/Fody/Usable) Adds using statements for local variables that have been created, and implement [IDisposable](https://msdn.microsoft.com/en-au/library/system.idisposable.aspx).
  * [Validar](https://github.com/Fody/Validar) Injects [IDataErrorInfo](https://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](https://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx ) code into a class at compile time.
  * [Vandelay](https://github.com/jasonwoods-7/Vandelay) Simplifies MEF importing\exporting.
  * [Visualize](https://github.com/Fody/Visualize) Adds debugger attributes to help visualize objects.
  * [Virtuosity](https://github.com/Fody/Virtuosity) Change all members to virtual.
  * [WeakEvents](https://github.com/adbancroft/WeakEvents.Fody) Automatic publishing of weak events via compile time code weaving and a supporting runtime library.
  * [With](https://github.com/mikhailshilkov/With.Fody) Methods to return copies of immutable objects with one property modified.
  * [YALF](https://github.com/sharpmonkey/YALF) Yet Another Logging Framework.
  * [Tail](https://github.com/hazzik/Tail.Fody) Adds a postfixed method call instruction to recursive calls.


### No longer maintained

The below addins are no longer maintained. Raise an issue in the specific project if you would like to take ownership.

  * [ArraySlice](https://github.com/Codealike/arrayslice) ArraySlice allows to build shared memory array views without performance impact. It uses IL manipulation to achieve the fastest implementation.
  * [Cilador](https://github.com/rileywhite/Cilador) Write your own [mixins](https://en.wikipedia.org/wiki/Mixin) in C# for code reuse without inheritance.
  * [Commander](https://github.com/DamianReeves/Commander.Fody) Injects ICommand properties and implementations for use in MVVM applications.
  * [JetBrainsAnnotations](https://github.com/Fody/JetBrainsAnnotations) Leverage JetBrains Annotations without the need for JetBrainsAnnotations.dll.
  * [Mixins](https://bitbucket.org/skwasiborski/mixins.fody/wiki/Home) A mixin is a class that provides a certain functionality to be inherited or just reused by a subclass.
  * [NameOf](https://github.com/NickStrupat/NameOf) Provides strongly typed access to a compile-time string representing the name of a variable, field, property, method, event, enum value, or type.
  * [RemoveReference](https://github.com/icnocop/RemoveReference.Fody) Facilitates removing references in a compiled assembly during a build.
  * [Scalpel](https://github.com/Fody/Scalpel) Strips tests from an assembly.
  * [Seal](https://github.com/kamil-mrzyglod/Seal) mark all non-virtual(abstract, non-sealed) types as sealed by default.
  * [Stiletto](https://github.com/benjamin-bader/stiletto) Compile-time static analysis and optimization for the Stiletto IoC library.
  * [Stamp](https://github.com/Fody/Stamp) Stamps an assembly with git data.


## Projects built using Fody

  * [Catel](http://catelproject.com/)
  * [Enums.NET](https://github.com/TylerBrinkley/Enums.NET)
  * [NServiceBus](https://particular.net/nservicebus)
  * [Orchestra & all Orc.* components (30+)](https://github.com/WildGums)


## Icon

<a href="http://thenounproject.com/noun/bird/#icon-No6726" target="_blank">Bird</a> designed by <a href="http://thenounproject.com/MARCOHS" target="_blank">Marco Hernandez</a> from The Noun Project


## More Info

 * [AddinSearchPaths](https://github.com/Fody/Fody/wiki/AddinSearchPaths)
 * [DeployingAddinsAsNugets](https://github.com/Fody/Fody/wiki/DeployingAddinsAsNugets)
 * [Home](https://github.com/Fody/Fody/wiki/Home)
 * [HowToWriteAnAddin](https://github.com/Fody/Fody/wiki/HowToWriteAnAddin)
 * [InSolutionWeaving](https://github.com/Fody/Fody/wiki/InSolutionWeaving)
 * [AssemblyVerification](https://github.com/Fody/Fody/wiki/AssemblyVerification)
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
 * [Building From A Network Share](https://github.com/Fody/Fody/wiki/BuildingFromANetworkShare)


## With thanks to


### Resharper from Jetbrains

http://www.jetbrains.com/resharper/

![Resharper.png](http://www.jetbrains.com/img/logos/logo_resharper_small.gif)


### TeamCity from Jetbrains

http://www.jetbrains.com/teamcity/

![TeamCity.png](http://www.jetbrains.com/img/logos/logo_teamcity_small.gif)
