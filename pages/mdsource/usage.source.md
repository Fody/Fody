# Usage

Install using [NuGet](https://docs.microsoft.com/en-au/nuget/). See [Using the package manager console](https://docs.microsoft.com/en-au/nuget/tools/package-manager-console) for more info.

Fody ships in two parts:

 1. The core "engine" shipped in the [Fody NuGet package](https://www.nuget.org/packages/Fody/)
 1. Any number of ["addins" or "weavers"](#addins-list) which are all shipped in their own NuGet packages.



## Install Fody

Since NuGet always defaults to the oldest, and most buggy, version of any dependency it is important to do a [NuGet install](https://docs.microsoft.com/en-us/nuget/tools/ps-ref-install-package) of Fody after installing any weaver.

```
Install-Package Fody
```

[Subscribe to Fody](https://libraries.io/nuget/Fody) on [Libraries.io](https://libraries.io) to get notified of releases of Fody.


## Add the NuGet Package

[Install](https://docs.microsoft.com/en-us/nuget/tools/ps-ref-install-package) the package in the project:

```
Install-Package WeaverName.Fody
```

e.g.

```
Install-Package Virtuosity.Fody
```


## Adding Fody to a project that generates a NuGet package

When Fody is added to a project that generates a NuGet package, the produced package will have a dependency on Fody. If this dependency needs to be removed from the generated .nupkg file, then, in the consuming .cproj project file, replace 

```xml
<PackageReference Include="Fody" Version="xxx" />
```

with

```xml
<PackageReference Include="Fody" Version="xxx" >
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```


## Add FodyWeavers.xml

To indicate what weavers run and in what order a `FodyWeavers.xml` file is used at the project level. It needs to be added manually. It takes the following form:


```xml
<Weavers>
  <WeaverName/>
</Weavers>
```

e.g.

```xml
<Weavers>
  <Virtuosity/>
</Weavers>
```

See [Configuration](configuration.md) for additional options.