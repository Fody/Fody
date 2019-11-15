# Configuration


## FodyWeavers.xml

Fody requires a `FodyWeavers.xml` file to be present in the project directory. If it is missing, a default file will be created the first time the project is built.

This file specifies the list of weavers that should be applied to the project, and in what order. Weavers can also be configured here.

The file format is:

```xml
<Weavers>
  <WeaverA />
  <WeaverB ConfigForWeaverB="Value" />
</Weavers>
```

The `<Weavers>` element supports the following attributes:

 * `VerifyAssembly`: Set to `true` to run PEVerify on the build result. See [Assembly verification](#assembly-verification).
 * `VerifyIgnoreCodes`: A comma-separated list of error codes which should be ignored during assembly verification. See [Assembly verification](#assembly-verification).
 * `GenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) support for `FodyWeavers.xml`. This overrides the `FodyGenerateXsd` MSBuild property.


## MSBuild properties

The following options can be set through MSBuild properties:

 * `DisableFody`: Set to `false` to disable Fody entirely.
 * `FodyGenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) support for `FodyWeavers.xml`.
 * `FodyVerifyAssembly`: Enables [assembly verification](#assembly-verification) with PEVerify.


## Defining dependencies in the build order

This can be done by defining a property group project named `FodyDependsOnTargets`. The content of this property will then be passed to the `DependsOnTargets` of the Fody weaving task.

> DependsOnTargets: The targets that must be executed before this target can be executed or top-level dependency analysis can occur. Multiple targets are separated by semicolons.

An example use case of this is to force Fody to run after [CodeContracts](https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/code-contracts).

```xml
<PropertyGroup>
  <FodyDependsOnTargets>
    CodeContractRewrite
  </FodyDependsOnTargets>
</PropertyGroup>
```


## Assembly Verification

Post build verification via [PeVerify](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool) is supported.

To enable do one of the following

Add `VerifyAssembly="true"` to FodyWeavers.xml:

```xml
<Weavers VerifyAssembly="true">
  <Anotar.Custom />
</Weavers>
```

Add a build constant with the value of `FodyVerifyAssembly`

To send ignore codes to PeVerify use `VerifyIgnoreCodes`.

```xml
<Weavers VerifyAssembly="true"
         VerifyIgnoreCodes="0x80131869">
  <Anotar.Custom />
</Weavers>
```