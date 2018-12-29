[![AppVeyor](https://img.shields.io/appveyor/ci/SimonCropp/fody/master.svg?style=flat&max-age=86400&label=appveyor)](https://ci.appveyor.com/project/SimonCropp/fody/branch/master)
[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat&max-age=86400)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/Fody/)
[![Patrons on Open Collective](https://opencollective.com/fody/tiers/patron/badge.svg)](#patrons)

### <img src="https://raw.githubusercontent.com/Fody/Fody/master/package_icon.png" height="28px"> Extensible tool for weaving .net assemblies

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model.


<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

**It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059). [Go to licensing FAQ](https://github.com/Fody/Fody/blob/master/readme.md#licensingpatron-faq)**


### Platinum Sponsors

Support this project by [becoming a Platinum Sponsor](https://opencollective.com/fody/order/7089). A banner with your company logo will be added here with a link to your website. A "Sponsored by" text and link will be added to the description of the NuGet Package for the life of your sponsorship. You also get 1 hour of remote support per month.

<!--
<a href="https://opencollective.com/fody/tiers/platinum/0/website"><img src="https://opencollective.com/fody/tiers/platinum/0/avatar.svg" height="100px"></a>
-->


### Gold Sponsors

Support this project by [becoming a Gold Sponsor](https://opencollective.com/fody/order/7088). A large company logo will be added here with a link to your website.

<a href="https://www.postsharp.net?utm_source=fody&utm_medium=referral"><img src="https://raw.githubusercontent.com/Fody/Home/master/logos/PostSharp.png"></a>


### Silver Sponsors

Support this project by [becoming a Silver Sponsor](https://opencollective.com/fody/order/7086). A medium company logo will be added here with a link to your website.


### Bronze Sponsors

Support this project by [becoming a Bronze Sponsor](https://opencollective.com/fody/order/7085). The company avatar will show up here with a link to your OpenCollective Profile.


<a href="https://opencollective.com/fody/tiers/bronze/0/website"><img src="https://opencollective.com/fody/tiers/bronze/0/avatar.svg?avatarHeight=100"></a>


### Patrons and sponsors

Thanks to all the backers and sponsors! Support this project by [becoming a patron](https://opencollective.com/fody/order/3059).

<a href="https://opencollective.com/fody#contributors">
	<img src="https://opencollective.com/fody/sponsor.svg?width=890&avatarHeight=50&button=false">
	<img src="https://opencollective.com/fody/backer.svg?width=890&avatarHeight=50&button=false">
</a>



<!--- EndOpenCollectiveBackers -->


### Thanks to [JetBrains](https://www.jetbrains.com)

For the generous donation of [ReSharper](https://www.jetbrains.com/resharper/) licenses.

<a href="https://www.jetbrains.com/resharper"><img src="http://www.jetbrains.com/img/logos/logo_resharper_small.gif"/></a>


## Licensing/Patron FAQ

**It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059).**


### Is this all versions?

No, it will include version 3.3.3 and higher.


### Honesty System / Enforcement

It is an honesty system with no code or legal enforcement. When raising an issue or a pull request, the GitHub Id may be checked to ensure they are a patron, and that issue/PR may be closed without further examination. This process will depend on the issue quality, your circumstances, and the impact on the larger user base. If a individual or organization has no interest in the long term sustainability of the project, then they are legally free to ignore the honesty system.


### Why charge for open source?

 * [Open-Source Maintainers are Jerks! | Nick Randolph & Geoffrey Huntley](https://vimeo.com/296579853)
 * [FOSS is free as in toilet | Geoffroy Couprie](http://unhandledexpression.com/general/2018/11/27/foss-is-free-as-in-toilet.html)
 * [How to Charge for your Open Source | Mike Perham](https://www.mikeperham.com/2015/11/23/how-to-charge-for-your-open-source/)
 * [Sustain OSS: The Report](https://sustainoss.org/assets/pdf/SustainOSS-west-2017-report.pdf)
 * [Open Source Maintainers Owe You Nothing | Mike McQuaid](https://mikemcquaid.com/2018/03/19/open-source-maintainers-owe-you-nothing/)
 * [Who should fund open source projects? | Jane Elizabeth](https://jaxenter.com/who-funds-open-source-projects-133222.html)
 * [Apply at OSS Inc today| Ryan Chenkie](https://twitter.com/ryanchenkie/status/1067801413974032385)
 * [The Ethics of Unpaid Labor and the OSS Community | Ashe Dryden](https://www.ashedryden.com/blog/the-ethics-of-unpaid-labor-and-the-oss-community)


### So what OSS license are projects using?

Fody (all code, NuGets and binaries) are under the [MIT License](https://opensource.org/licenses/MIT).


### Why not use a modified MIT license?

Using any OSS license in a modified form causes significant problems with adoption of tools. There is no simplified guidance on using modified licenses. For example they are not included in [choose a license](https://choosealicense.com/) or [tldr legal](https://tldrlegal.com/). It often forces an organization to obtain approval from a legal department. It means any consuming tools need to ensure that the modified license does not propagate in an undesirable way.


### But shouldn't OSS be completely free and supported by the community through their contributions?

Yes in theory this is true, however the long term reality has shown this not to be the case. The vast majority of consumers of open source projects do not contribute enough to ensure those project survive. This results in a small core team spending large amounts of their own free time maintaining projects.


### Is support now included as part of the cost?

No. As per the MIT license:

> the software is provided "as is", without warranty of any kind

For a supported product, you should instead consider [PostSharp](https://www.postsharp.net/?utm_source=fody&utm_medium=referral).
PostSharp is a mature and supported product that is used by Fortune 500 companies as well as startups, and it doesn't require knowledge of IL.

They offer a free edition and their feature list includes: Aspect-Oriented Programming, Logging, INotifyPropertyChanged, Caching, Transactions, Security, Multithreading, Code Contracts, XAML plumbing, and Architecture Validation.


### But it is MIT, can't I use it for free?

Yes all projects are under [MIT](https://opensource.org/licenses/MIT) and you can ignore the community backing honesty system and use Fody for free.


### Do I need to be a Patron to contribute a Pull Request?

Yes. You must be a Patron to be a user of Fody. Contributing Pull Requests does not cancel this out. It may seem unfair to expect people both contribute PRs and also financially back this project. However it is important to remember the effort in reviewing and merging a PR is often similar to that of creating the PR. Also the project maintainers are committing to support that added code (feature or bug fix) for the life of the project.


### Organization licensing


#### How to handle multiple developers

There are two options for an organization.

  1. Apply a multiplier to the Patron cost.
    The [Patron tier](https://opencollective.com/fody/order/3059) has no upper bound on the monthly amount. This allows an organization with multiple developers to pay a single monthly price. For example: An organization with 5 developers would pay $15 per month, i.e. 5 x $3 per patron. An organization with 10 developers would pay $30 per month, i.e. 10 x $3 per patron and so on.
  2. Create an Open Collective organization
    A organization can [Create an Open Collective organization](https://opencollective.com/become-a-sponsor) and then allow their developers to draw on the funds from that organization. See [FAQ for backers](https://opencollective.com/faq/backers). This is the recommended option as it also opens up the opportunity for developers to select other projects they feel need support.


#### Do all developers in a organization need to become Patrons?

No. Only those coding against projects that directly, or indirectly, consume the [Fody NuGet package](https://www.nuget.org/packages/Fody/).


#### Can only one developer of an organization become a patron?

Yes, since the only point of (optional) enforcement is when an issue or PR is raised, then legally an organization can ignore the honesty system and route all issues and PRs though a single GitHub user account. However if a single GitHub user account is drawing on significant time to support, they may be requested to purchase some [hourly support](https://opencollective.com/fody/order/7087).


### What about open source projects consume or producing Fody weavers?

It is be expected that the core team of maintainers of any open source projects that consume or produce Fody weavers would become Patrons. Non core contributors do not need to become Patrons. For example, in the context of GitHub, this would be any people with push access to the the repository. The reasoning is that if a project is utilizing Fody, that should not be a blocker for the larger community from contributing to the project.


### Can I fork, re-use code, or start competing (possibly commercial) projects?

Yes.


### What happens if I produce a library using Fody?

Consumers of that produced library do not need to become Patrons.



<a href="#" id="endofbacking"></a>



## Usage

Install using [NuGet](https://docs.microsoft.com/en-au/nuget/). See [Using the package manager console](https://docs.microsoft.com/en-au/nuget/tools/package-manager-console) for more info.

Fody ships in two parts:

 1. The core "engine" shipped in the [Fody NuGet package](https://www.nuget.org/packages/Fody/)
 1. Any number of ["addins" or "weavers"](#addins-list) which are all shipped in their own NuGet packages.

The below examples will use [Virtuosity](https://github.com/Fody/Virtuosity).


### Install Fody

Since NuGet always defaults to the oldest, and most buggy, version of any dependency it is important to do a [NuGet install](https://docs.microsoft.com/en-us/nuget/tools/ps-ref-install-package) of Fody after installing any weaver.

```
Install-Package Fody
```

[Subscribe to Fody](https://libraries.io/nuget/Fody) on [Libraries.io](https://libraries.io) to get notified of releases of Fody.


### Add the nuget

[Install](https://docs.microsoft.com/en-us/nuget/tools/ps-ref-install-package) the package in the project:

```
Install-Package WeaverName.Fody
```

e.g.

```
Install-Package Virtuosity.Fody
```


### Add FodyWeavers.xml

To indicate what weavers run and in what order a `FodyWeavers.xml` file is used at the project level. It needs to be added manually. It takes the following form:


```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <WeaverName/>
</Weavers>
```

e.g.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <Virtuosity/>
</Weavers>
```

See [Configuration](https://github.com/Fody/Fody/wiki/Configuration) in the wiki for additional options.

The [Project Configuration Manager](https://github.com/tom-englert/ProjectConfigurationManager/wiki/6.-Fody) provides an interactive tool that can support you configuring your weavers, which is especially helpful in solutions with many projects.


## Supported Visual Studio Versions

To build a project using Fody you will need:

 * Visual Studio 2017 or later
 * .Net >= 4.6

Older versions of Visual Studio may still work, but are not actively supported. We do our best to not break backward compatibility, but can't guarantee this forever.


## Supported Runtimes

 * Classic .NET: See *Support ended* in [NET Framework version history](https://en.wikipedia.org/wiki/.NET_Framework_version_history#Overview). i.e only 4.5.2 and above is supported.
 * .NET core: Follows [.NET Core Support Policy](https://www.microsoft.com/net/core/support).

No explicit code is in place to check for non supported versions, and throw an error. As such earlier versions of .net may work as a side effect.
It's all up to the individual weavers that you use and what version they are able to support.

Any bugs found must be reproduced in a supported version.

Downstream plugins are recommended to follow the above guidelines.


### Reasons

While it may seam trivial to "implement support for earlier versions of .net" the long term support implications are too costly. For example to support earlier versions of .net require

 * Custom VMs to verify problems.
 * Added complexity to setting up build environment.
 * Bugs in unsupported versions in .NET may be manifest as bugs in Fody.


## Edit and continue

Edit and continue is not supported. There is no extension point to re-weave an assembly after the edit part.


## Project formats

The following are not supported

 * Projects using the [project.json](https://docs.microsoft.com/en-us/nuget/schema/project-json).
 * Projects using the xproj.
 * Projects mixing the old `.csproj` format with new [`<PackageReference>` nodes](https://docs.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files#adding-a-packagereference).

To tell the difference between the old and new csproj formats.

The old format starts with

```
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
```

The new format starts with

```
<Project Sdk="Microsoft.NET.Sdk">
```

For all these scenarios is it instead recommended to move to the new VS 2017 SDK style projects.

References

 * [Bye-Bye Project.json and .xproj and welcome back .csproj](http://www.talkingdotnet.com/bye-bye-project-json-xproj-welcome-back-csproj/)
 * [Project.json to MSBuild conversion guide](http://www.natemcmaster.com/blog/2017/01/19/project-json-to-csproj/)
 * [Migrate from project.json to new VS 2017 SDK style projects](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-migrate)


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


## Naming

The name "Fody" comes from the small birds that belong to the weaver family [Ploceidae](http://en.wikipedia.org/wiki/Fody).


## Samples

 * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
 * [FodyAddinSamples](https://github.com/Fody/FodyAddinSamples) is a single solution that contains a working copy of every fody addin.


## Addins List

  * [Anotar](https://github.com/Fody/Anotar) Simplifies logging through a static class and some IL manipulation.
  * [AssertMessage](https://github.com/Fody/AssertMessage) Generates 'message' from sourcecode and adds it to assertion.
  * [AsyncErrorHandler](https://github.com/Fody/AsyncErrorHandler) Integrates error handling into async and TPL code.
  * [AutoDependencyProperty](http://blog.angeloflogic.com/2014/12/no-more-dependencyproperty-with.html) Generates WPF DependencyProperty boilerplate from automatic C# properties.
  * [AutoProperties](https://github.com/tom-englert/AutoProperties.Fody) Gives you extended control over auto-properties, like directly accessing the backing field or intercepting getters and setters.
  * [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin) A simple project meant to illustrate how to build an addin.
  * [Bindables](https://github.com/yusuf-gunaydin/Bindables) Converts your auto properties into Wpf dependency or attached properties. Allows specifying options, defining readonly properties, and calling property changed methods.
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
  * [Equatable](https://github.com/tom-englert/Equatable.Fody) Generate Equals, GetHashCode and operators methods from explicit annotated fields and properties.
  * [Expose](https://github.com/kedarvaidya/Expose.Fody) Exposes members and optionally implements interface of a field declared in class.
  * [ExtraConstraints](https://github.com/Fody/ExtraConstraints) Facilitates adding constraints for Enum and Delegate to types and methods.
  * [FactoryId](https://github.com/ramoneeza/FactoryId.Fody) Simplifies the implementation of Factory Method Pattern
  * [Fielder](https://github.com/Fody/Fielder) Converts public fields to public properties.
  * [FodyDependencyInjection](https://github.com/jorgehmv/FodyDependencyInjection) Dependency injection with Fody add-ins.
  * [Freezable](https://github.com/Fody/Freezable) Implements the Freezable pattern.
  * [InfoOf](https://github.com/Fody/InfoOf) Provides `methodof`, `propertyof` and `fieldof` equivalents of [`typeof`](https://msdn.microsoft.com/en-us/library/58918ffs.aspx).
  * [InlineIL](https://github.com/ltrzesniewski/InlineIL.Fody) Provides a way to embed arbitrary IL instructions in existing code.
  * [Ionad](https://github.com/Fody/Ionad) Replaces static method calls.
  * [Janitor](https://github.com/Fody/Janitor) Simplifies the implementation of [IDisposable](https://msdn.microsoft.com/en-us/library/system.idisposable.aspx).
  * [JetBrainsAnnotations](https://github.com/tom-englert/JetBrainsAnnotations.Fody) Converts all JetBrains ReSharper code annotation attributes to External Annotations.
  * [Lazy](https://github.com/tom-englert/Lazy.Fody) Automates the plumbing around System.Lazy.
  * [LoadAssembliesOnStartup](https://github.com/Fody/LoadAssembliesOnStartup) Loads references on startup by using the types in the module initializer
  * [LoggerIsEnabled](https://github.com/wazowsk1/LoggerIsEnabled.Fody) Adds `ILogger.IsEnabled` check around the logging statement for the [Microsoft.Extensions.Logging](https://github.com/aspnet/Logging)
  * [MethodBoundaryAspect](https://github.com/vescon/MethodBoundaryAspect.Fody) Allows to decorate methods and hook into method start, method end and method exceptions (like PostSharp)
  * [MethodCache](https://github.com/Dresel/MethodCache) Caches return values of methods decorated with a `CacheAttribute`.
  * [MethodDecorator](https://github.com/Fody/MethodDecorator) Decorate arbitrary methods to run code before and after invocation.
  * [MethodTimer](https://github.com/Fody/MethodTimer) Injects method timing code.
  * [ModuleInit](https://github.com/Fody/ModuleInit) Adds a module initializer to an assembly.
  * [Mutable](https://github.com/ndamjan/Mutable.Fody) Make F# setters for union types and eliminate need for `CLIMutable` attribute for records.
  * [Mvid](https://github.com/hmemcpy/Mvid.Fody) Adds the ability to specify the assembly MVID (Module Version Id).
  * [Nancy.ModelPostprocess](https://bitbucket.org/tpluscode/nancy.modelpostprocess) Modify Nancy models after route execution but before serialization
  * [NObservable](https://github.com/kekekeks/NObservable) MobX-like observable state management library with Blazor support.
  * [NullGuard](https://github.com/Fody/NullGuard) Adds null argument checks to an assembly
  * [Obsolete](https://github.com/Fody/Obsolete) Helps keep usages of [ObsoleteAttribute]([https://msdn.microsoft.com/en-us/library/fwz0y5c2 ) consistent.
  * [Padded](https://github.com/Scooletz/Padded) Adds padding to fight the false sharing problem.
  * [PropertyChanged](https://github.com/Fody/PropertyChanged) Injects INotifyPropertyChanged code into properties.
  * [PropertyChanging](https://github.com/Fody/PropertyChanging) Injects INotifyPropertyChanging code into properties.
  * [Publicize](https://github.com/Fody/Publicize) Converts non-public members to public hidden members.
  * [QueryValidator](https://github.com/kamil-mrzyglod/QueryValidator.Fody) Validates your DB queries during a build.
  * [Realm](https://github.com/realm/realm-dotnet/tree/master/Weaver/RealmWeaver.Fody) Mobile database: a replacement for SQLite & ORMs.
  * [ReactiveUI.Fody](https://github.com/reactiveui/ReactiveUI) Generates [ReactiveUI](http://reactiveui.net/) `RaisePropertyChanged` notifications for properties and `OAPH`s.
  * [Resourcer](https://github.com/Fody/Resourcer) Simplifies reading embedded resources from an Assembly.
  * [RomanticWeb](http://romanticweb.net/) Fody weaver plugin for RomanticWeb instrumentation.
  * [Spring](https://github.com/jorgehmv/FodySpring) Spring constructor configuration.
  * [Stamp](https://github.com/304NotModified/Fody.Stamp) Stamps an assembly with git data.
  * [StampSvn](https://github.com/krk/Stamp) Stamps an assembly with SVN data.
  * [StaticProxy](https://github.com/BrunoJuchli/StaticProxy.Fody) Proxy Generator, also for .net standard / .net core (.net standard 1.0+).
  * [Substitute](https://github.com/tom-englert/Substitute.Fody) Substitute types with other types to e.g. intercept generated code
  * [SexyProxy](https://github.com/kswoll/sexy-proxy) Proxy generator with support for async patterns.
  * [SwallowExceptions](https://github.com/duaneedwards/SwallowExceptions) Swallow Exceptions in targeted methods.
  * [TestFlask](https://github.com/FatihSahin/test-flask) Records your method args and responses to replay, assert and test.
  * [Throttle](https://github.com/tom-englert/Throttle.Fody) Easily use throttles with minimal coding
  * [ToString](https://github.com/Fody/ToString) Generate `ToString` method from public properties.
  * [Tracer](https://github.com/csnemes/tracer) Adds trace-enter and trace-leave log entries for selected methods.
  * [Undisposed](https://github.com/ermshiperete/undisposed-fody) Debugging tool to track down undisposed objects.
  * [Usable](https://github.com/Fody/Usable) Adds using statements for local variables that have been created, and implement [IDisposable](https://msdn.microsoft.com/en-au/library/system.idisposable.aspx).
  * [Validar](https://github.com/Fody/Validar) Injects [IDataErrorInfo](https://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](https://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx ) code into a class at compile time.
  * [Vandelay](https://github.com/jasonwoods-7/Vandelay) Simplifies MEF importing\exporting.
  * [Visualize](https://github.com/Fody/Visualize) Adds debugger attributes to help visualize objects.
  * [Virtuosity](https://github.com/Fody/Virtuosity) Change all members to virtual.
  * [WeakEventHandler](https://github.com/tom-englert/WeakEventHandler.Fody) Changes regular event handlers into weak event handlers by weaving a weak event adapter between source and subscriber.
  * [WeakEvents](https://github.com/adbancroft/WeakEvents.Fody) Automatic publishing of weak events via compile time code weaving and a supporting runtime library.
  * [With](https://github.com/mikhailshilkov/With.Fody) Methods to return copies of immutable objects with one property modified.
  * [YALF](https://github.com/sharpmonkey/YALF) Yet Another Logging Framework.
  * [Tail](https://github.com/hazzik/Tail.Fody) Adds a postfixed method call instruction to recursive calls.


### No longer maintained

The below addins are no longer maintained. Raise an issue in the specific project if you would like to take ownership.

  * [ArraySlice](https://github.com/Codealike/arrayslice) ArraySlice allows to build shared memory array views without performance impact. It uses IL manipulation to achieve the fastest implementation.
  * [AutoLazy](https://github.com/bcuff/AutoLazy) Automatically implements the double-checked locking pattern on specified properties and methods.
  * [Cilador](https://github.com/rileywhite/Cilador) Write your own [mixins](https://en.wikipedia.org/wiki/Mixin) in C# for code reuse without inheritance.
  * [Commander](https://github.com/DamianReeves/Commander.Fody) Injects ICommand properties and implementations for use in MVVM applications.
  * [Mixins](https://bitbucket.org/skwasiborski/mixins.fody/wiki/Home) A mixin is a class that provides a certain functionality to be inherited or just reused by a subclass.
  * [NameOf](https://github.com/NickStrupat/NameOf) Provides strongly typed access to a compile-time string representing the name of a variable, field, property, method, event, enum value, or type.
  * [RemoveReference](https://github.com/icnocop/RemoveReference.Fody) Facilitates removing references in a compiled assembly during a build.
  * [Scalpel](https://github.com/Fody/Scalpel) Strips tests from an assembly.
  * [Seal](https://github.com/kamil-mrzyglod/Seal) mark all non-virtual(abstract, non-sealed) types as sealed by default.
  * [Stiletto](https://github.com/benjamin-bader/stiletto) Compile-time static analysis and optimization for the Stiletto IoC library.


## Icon

<a href="http://thenounproject.com/noun/bird/#icon-No6726">Bird</a> designed by <a href="http://thenounproject.com/MARCOHS">Marco Hernandez</a> from The Noun Project


## More Info

 * [Wiki Home](https://github.com/Fody/Fody/wiki/Home)
 * [Configuration](https://github.com/Fody/Fody/wiki/Configuration)
 * [AddinSearchPaths](https://github.com/Fody/Fody/wiki/AddinSearchPaths)
 * [DeployingAddinsAsNugets](https://github.com/Fody/Fody/wiki/DeployingAddinsAsNugets)
 * [HowToWriteAnAddin](https://github.com/Fody/Fody/wiki/HowToWriteAnAddin)
 * [InSolutionWeaving](https://github.com/Fody/Fody/wiki/InSolutionWeaving)
 * [AssemblyVerification](https://github.com/Fody/Fody/wiki/AssemblyVerification)
 * [ModuleWeaver](https://github.com/Fody/Fody/wiki/ModuleWeaver)
 * [PdbReWritingAndDebugging](https://github.com/Fody/Fody/wiki/PdbReWritingAndDebugging)
 * [SignedAssemblies](https://github.com/Fody/Fody/wiki/SignedAssemblies)
 * [SupportedRuntimesAndIde](https://github.com/Fody/Fody/wiki/SupportedRuntimesAndIde)
 * [TaskAddsAFlagInterface](https://github.com/Fody/Fody/wiki/TaskAddsAFlagInterface)
 * [TaskCouldNotBeLoaded](https://github.com/Fody/Fody/wiki/TaskCouldNotBeLoaded)
 * [WeavingTaskOptions](https://github.com/Fody/Fody/wiki/WeavingTaskOptions)
 * [ThrowingAnExceptionFromAnAddin](https://github.com/Fody/Fody/wiki/ThrowingAnExceptionFromAnAddin)
 * [Mono Support](https://github.com/Fody/Fody/wiki/Mono)
 * [Building From A Network Share](https://github.com/Fody/Fody/wiki/BuildingFromANetworkShare)


## Contributors

This project exists thanks to all the people who contribute. [[Contribute](CONTRIBUTING.md)].
<a href="https://github.com/Fody/Fody/graphs/contributors"><img src="https://opencollective.com/fody/contributors.svg?width=890&button=false" /></a>

