# Supported .net runtimes and IDE


## Visual Studio support

Fody requires one of the following at build time:

 * [MSBuild 16](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2019) which ships with Visual Studio 2019.
 * [dotnet core SDK 2.2.105](https://dotnet.microsoft.com/download) (or higher) to work with [dotnet build](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build).


## Project formats

The following are not supported

 * Projects using the [project.json](https://docs.microsoft.com/en-us/nuget/schema/project-json).
 * Projects using the xproj.

To tell the difference between the old and new csproj formats.

The old format starts with:

```
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
```

The new format starts with:

```
<Project Sdk="Microsoft.NET.Sdk">
```

For all these scenarios is it instead recommended to move to the new SDK style projects.

References

 * [Bye-Bye Project.json and .xproj and welcome back .csproj](http://www.talkingdotnet.com/bye-bye-project-json-xproj-welcome-back-csproj/)
 * [Project.json to MSBuild conversion guide](http://www.natemcmaster.com/blog/2017/01/19/project-json-to-csproj/)
 * [Migrate from project.json to new SDK style projects](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-migrate)


## Target framework

 * Classic .NET: See *Support ended* in [NET Framework version history](https://en.wikipedia.org/wiki/.NET_Framework_version_history#Overview). i.e only 4.5.2 and above is supported.
 * .NET core: Follows [.NET Core Support Policy](https://www.microsoft.com/net/core/support).

No explicit code is in place to check for non supported versions, and throw an error. As such earlier versions of .net may work as a side effect. It's all up to the individual weavers that you use and what version they are able to support.

Any bugs found must be reproduced in a supported version.

Downstream weavers are recommended to follow the above guidelines.


### Reasons

While it may seam trivial to "implement support for earlier versions of .net" the long term support implications are too costly. For example to support earlier versions of .net require

 * Custom VMs to verify problems.
 * Added complexity to setting up build environment.
 * Bugs in unsupported versions in .NET may be manifest as bugs in Fody.


## Implications if using using Fody on project

Fody provides the build integration for the weavers. The runtime that the project may target only depends on the weaver(s) being used; see the documentation of the individual weavers to know what frameworks are supported.

The target framework(s) supported by a weaver is up to the designer of the individual weaver. Since extending the range of supported frameworks might introduce extra overhead, not all possible framework versions might be actively supported.
