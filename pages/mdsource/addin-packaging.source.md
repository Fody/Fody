# Addin packaging


## Convention

The convention for Fody addin NuGet packages is as follows:

&#x1F4C1; build<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.props<br>
&#x1F4C1; lib<br>
&nbsp;&nbsp;&nbsp;&#x1F4C1; net452<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.dll<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.xml<br>
&nbsp;&nbsp;&nbsp;&#x1F4C1; netstandard2.0<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.dll<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.xml<br>
&#x1F4C1; weaver<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.dll<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.xcf<br>


### Convention Descriptions


 * `build/AddinName.Fody.props`: Facilitates [addin discovery](addin-discovery.md) via an [props file included by NuGet](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).
 * `lib`: Contains the [Lib/Reference project](addin-development.md#Lib/Reference-project) for all [supported target frameworks](https://docs.microsoft.com/en-us/nuget/create-packages/supporting-multiple-target-frameworks).
 * `weaver`: Contains the [Weaver Project](addin-development.md#Weaver-Project) assembly. Also contains the XCF file to [Supporting intellisense for FodyWeavers.xml](addin-development.md#Supporting-intellisense-for-FodyWeavers.xml).


## FodyPackaging NuGet Package

The [FodyPackaging NuGet Package](https://www.nuget.org/packages/FodyPackaging/) simplifies following the above convention.


### MSBuild props and targets

The below files are include as [MSBuild props and targets in a package](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).


#### FodyPackaging.props

snippet: FodyPackaging.props


#### FodyPackaging.targets

snippet: FodyPackaging.targets


### Weaver.props

Included in the consuming package to facilitate [addin discovery](addin-discovery.md).

snippet: Weaver.props