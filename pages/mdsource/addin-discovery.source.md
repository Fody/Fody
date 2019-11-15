# Addin Discovery

Every Weaver must publish the location of it's binary ('WaverName.Fody.dll') at compile time as an MSBuild item, so Fody is able to locate it. This is achieved by providing a `.props` file with the NuGet package with the following default content:

snippet: Weaver.props

If the [FodyPackaging NuGet](addin-packaging.md#FodyPackaging-NuGet-Package) is used to create the addin package, this file is automatically added.

However depending on requirements a custom file may be required. The important part is to provide an item named `WeaverFiles` that points to the location of the weaver assembly somewhere in the build chain.

For example to replace the legacy `SolutionDir/Tool` or [in solution weaving](in-solution-weaving.md) conventions, add the `WeaverFiles` item to any project that needs to consume it:

```xml
<ItemGroup>
  <WeaverFiles
    Include="$(SolutionDir)SampleWeaver.Fody\bin\$(Configuration)\netstandard2.0\SampleWeaver.Fody.dll" />
</ItemGroup>
```


## Legacy strategies

**Legacy strategies will no longer be supported in Fody version 4.0 and above**

Legacy Weavers that are listed in the `FodyWeavers.xml` file, but don't expose the `WeaverFiles` MSBuild item, are located using a simple directory search.

The following directories are searched for legacy Weavers

 * NuGet Package directories
 * SolutionDir/Tools
 * A project in the solution named 'Weavers'. See [in solution weaving](in-solution-weaving.md)

Only the newest assembly of every found Weaver (as defined by Assembly.Version) is used.

Since this can result in random results, depending on the actual content of the folders, avoid to use such legacy weavers, but ask the owner to update the weaver.