[![AppVeyor](https://img.shields.io/appveyor/ci/SimonCropp/fody/master.svg?style=flat&max-age=86400&label=appveyor)](https://ci.appveyor.com/project/SimonCropp/fody/branch/master)
[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/Fody.svg)](https://www.nuget.org/packages/Fody/)
[![Patrons on Open Collective](https://opencollective.com/fody/tiers/patron/badge.svg)](#patrons)
[![Build Status](https://dev.azure.com/tom-englert/Fody/_apis/build/status%2FFody.Integration?repoName=Fody%2FFody&branchName=master)](https://dev.azure.com/tom-englert/Fody/_build/latest?definitionId=42&repoName=Fody%2FFody&branchName=master)

### <img src="https://raw.githubusercontent.com/Fody/Fody/master/package_icon.png" height="28px"> Extensible tool for weaving .net assemblies

Manipulating the IL of an assembly as part of a build requires a significant amount of plumbing code. This plumbing code involves knowledge of both the MSBuild and Visual Studio APIs. Fody attempts to eliminate that plumbing code through an extensible add-in model.

**This is the codebase of core Fody engine. For more information on the larger Fody project see https://github.com/Fody/Home.**

**See [Milestones](https://github.com/Fody/Fody/milestones?state=closed) for release notes.**


<!--- StartOpenCollectiveBackers -->

[Already a Patron? skip past this section](#endofbacking)


## Community backed

Fody requires significant effort to maintain. As such it relies on financial support to ensure its long term viability.

**It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059).**

[See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.


### Gold Sponsors

Support this project by [becoming a Gold Sponsor](https://opencollective.com/fody/contribute/gold-7088). A large company logo will be added here with a link to your website.

<a href="https://www.postsharp.net?utm_source=fody&utm_medium=referral"><img alt="PostSharp" src="https://raw.githubusercontent.com/Fody/Home/master/images/postsharp.png"></a>


### Silver Sponsors

Support this project by [becoming a Silver Sponsor](https://opencollective.com/fody/contribute/silver-7086). A medium company logo will be added here with a link to your website.

<a href="https://www.gresearch.co.uk/"><img alt="G-Research" width="120px" src="https://raw.githubusercontent.com/Fody/Home/master/images/g-research.svg?sanitize=true"></a> <a href="https://particular.net/"><img alt="Particular Software" width="200px" src="https://raw.githubusercontent.com/Fody/Home/master/images/particular.svg?sanitize=true"></a>


### Bronze Sponsors

Support this project by [becoming a Bronze Sponsor](https://opencollective.com/fody/contribute/bronze-7085). The company avatar will show up here with a link to your OpenCollective Profile.

<a href="https://opencollective.com/fody/tiers/bronze/0/website"><img src="https://opencollective.com/fody/tiers/bronze/0/avatar.svg?avatarHeight=100"></a> 


### Patrons and sponsors

Thanks to all the backers and sponsors! Support this project by [becoming a patron](https://opencollective.com/fody/contribute/patron-3059).

<a href="https://opencollective.com/fody#contributors"><img src="https://opencollective.com/fody/sponsor.svg?width=890&avatarHeight=50&button=false"><img src="https://opencollective.com/fody/backer.svg?width=890&avatarHeight=50&button=false"></a>


<!--- EndOpenCollectiveBackers -->


## Hot Reload

Fody modifies the assembly after the compiler has run. This is fundamentally incompatible with in-process [.NET Hot Reload](https://learn.microsoft.com/visualstudio/debugger/hot-reload) / Edit and Continue: Hot Reload computes its changes against the assembly the compiler emitted, but the assembly running on disk is the *woven* one. The two diverge, so applying a Hot Reload edit to a Fody project results in errors rather than woven code. See [dotnet/roslyn#56678](https://github.com/dotnet/roslyn/issues/56678).

To avoid this, Fody opts woven assemblies out of in-process Hot Reload:

  * It removes the `EnableEditAndContinue` flag from the assembly. This flag is only emitted for debug builds, so this is a no-op for release builds.
  * It sets `SupportsHotReload` to `false` so Visual Studio does not offer the Hot Reload button for Fody projects.

As a result, editing a Fody project during a `dotnet watch`, Visual Studio, or Rider session falls back to a full rebuild and restart instead of an in-process update. The rebuild re-runs Fody, so the running application always matches the woven output. The trade-off is losing in-process Hot Reload speed in exchange for correctness.

To opt out, disable Fody for the build via `<DisableFody>true</DisableFody>`.


## Documentation and Further Learning

  * [Licensing and patron FAQ](https://github.com/Fody/Home/tree/master/pages/licensing-patron-faq.md)<br>
    **It is expected that all developers using Fody [become a Patron on OpenCollective](https://opencollective.com/fody/order/3059).** See [Licensing/Patron FAQ](https://github.com/Fody/Home/tree/master/pages/licensing-patron-faq.md) for more information.
  * [Usage](https://github.com/Fody/Home/tree/master/pages/usage.md)<br>
    Introduction to using Fody.
  * [Configuration](https://github.com/Fody/Home/tree/master/pages/configuration.md)<br>
    All configuration options for Fody.
  * [Addin discovery](https://github.com/Fody/Home/tree/master/pages/addin-discovery.md)<br>
    How addins are resolved.
  * [List of Fody weavers/addins](https://github.com/Fody/Home/tree/master/pages/addins.md)<br>
  * [Changelog](https://github.com/Fody/Fody/milestones?state=closed)
  * [FodyAddinSamples](https://github.com/Fody/FodyAddinSamples)<br>
    A GitHub repo that contains a working sample of every Fody addin.
  * [Common errors](https://github.com/Fody/Home/tree/master/pages/common-errors.md)
  * [In solution weaving](https://github.com/Fody/Home/tree/master/pages/in-solution-weaving.md)<br>
    Writing an addin that manipulates IL within the same solution.
  * [ProcessedByFody class](https://github.com/Fody/Home/tree/master/pages/processedbyfody-class.md)<br>
    Marker class added to target assemblies for diagnostic purposes.
  * [Strong naming](https://github.com/Fody/Home/tree/master/pages/strong-naming.md)
  * [Supported runtimes and IDE](https://github.com/Fody/Home/tree/master/pages/supported-runtimes-and-ide.md)
  * [Addin development](https://github.com/Fody/Home/tree/master/pages/addin-development.md)<br>
    Building a new Fody addin.
  * [Addin packaging](https://github.com/Fody/Home/tree/master/pages/addin-packaging.md)<br>
    Packaging and deployment of Fody weavers.
  * [BasicFodyAddin](https://github.com/Fody/Home/tree/master/BasicFodyAddin)<br>
    A simple project meant to illustrate how to build an addin.
  * [Fody Project Configuration Manager](https://github.com/tom-englert/ProjectConfigurationManager/wiki/6.-Fody)<br>
    Provides an interactive tool that can support configuring weavers, which is especially helpful in solutions with many projects.
  * [Backers tracking/info](https://github.com/Fody/Home/tree/master/pages/backers.md)
  * [Donations](https://github.com/Fody/Home/tree/master/pages/donations.md)<br>
    Every month the Fody project will donate a portion of funding raised to a charity or other cause.


## Contributors

This project exists thanks to all the people who contribute.
<a href="https://github.com/Fody/Fody/graphs/contributors"><img src="https://opencollective.com/fody/contributors.svg?width=890&button=false" /></a>
