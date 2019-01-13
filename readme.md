[![AppVeyor](https://img.shields.io/appveyor/ci/SimonCropp/fody/master.svg?style=flat&max-age=86400&label=appveyor)](https://ci.appveyor.com/project/SimonCropp/fody/branch/master)
[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat&max-age=86400)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/Fody/)
[![Patrons on Open Collective](https://opencollective.com/fody/tiers/patron/badge.svg)](#patrons)

### <img src="https://raw.githubusercontent.com/Fody/Fody/master/package_icon.png" height="28px"> Extensible tool for weaving .net assemblies

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model.

**This is the codebase of core Fody engine. For more information on the larger Fody project see https://github.com/Fody/Home.**


<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

Fody requires significant effort to maintain. As such it relies on financial contributions from the community and sponsorship to ensure its long term viability. **It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059).**

[Go to licensing FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.


### Platinum Sponsors

Support this project by [becoming a Platinum Sponsor](https://opencollective.com/fody/order/7089). A banner with your company logo will be added here with a link to your website. A "Sponsored by" text and link will be added to the description of the NuGet Package for the life of your sponsorship. You also get 1 hour of remote support per month.

<!--
<a href="https://opencollective.com/fody/tiers/platinum/0/website"><img src="https://opencollective.com/fody/tiers/platinum/0/avatar.svg" height="100px"></a>
-->


### Gold Sponsors

Support this project by [becoming a Gold Sponsor](https://opencollective.com/fody/order/7088). A large company logo will be added here with a link to your website.

<a href="https://www.postsharp.net?utm_source=fody&utm_medium=referral"><img alt="PostSharp" src="https://raw.githubusercontent.com/Fody/Home/master/images/postsharp.png"></a>


### Silver Sponsors

Support this project by [becoming a Silver Sponsor](https://opencollective.com/fody/order/7086). A medium company logo will be added here with a link to your website.

<a href="https://particular.net/"><img alt="Particular Software" width="200px" src="https://raw.githubusercontent.com/Fody/Home/master/images/particular.svg?sanitize=true"></a>


### Bronze Sponsors

Support this project by [becoming a Bronze Sponsor](https://opencollective.com/fody/order/7085). The company avatar will show up here with a link to your OpenCollective Profile.


<a href="https://opencollective.com/fody/tiers/bronze/0/website"><img src="https://opencollective.com/fody/tiers/bronze/0/avatar.svg?avatarHeight=100"></a>


### Patrons and sponsors

Thanks to all the backers and sponsors! Support this project by [becoming a patron](https://opencollective.com/fody/order/3059).

<a href="https://opencollective.com/fody#contributors"><img src="https://opencollective.com/fody/sponsor.svg?width=890&avatarHeight=50&button=false"><img src="https://opencollective.com/fody/backer.svg?width=890&avatarHeight=50&button=false"></a>



<!--- EndOpenCollectiveBackers -->


## Licensing/Patron FAQ

[Moved to Fody/Home: Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md)


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


## The plumbing tasks Fody handles

  * Injection of the MSBuild task into the build pipeline
  * Resolving the location of the assembly and pdb
  * Abstracts the complexities of logging to MSBuild
  * Reads the assembly and pdb into the Mono.Cecil object model
  * Re-applying the strong name if necessary
  * Saving the assembly and pdb

Fody Uses [Mono.Cecil](http://www.mono-project.com/Cecil/) and an add-in based approach to modifying the IL of .net assemblies at compile time.

 * No install required to build
 * No attributes required
 * No references required


## Addins List

[Moved to Fody/Home: Addins](https://github.com/Fody/Home/blob/master/pages/addins.md)


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

