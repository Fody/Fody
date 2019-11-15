# Assembly Strong Naming

Fody looks for the two settings that indicate a project file has produced a [strong named](https://docs.microsoft.com/en-us/dotnet/framework/app-domains/strong-named-assemblies) assembly.


## SignAssembly

```xml
<PropertyGroup>
  <SignAssembly>true</SignAssembly>
</PropertyGroup>
```


## AssemblyOriginatorKeyFile 

```xml
<PropertyGroup>
  <AssemblyOriginatorKeyFile>Key.snk</AssemblyOriginatorKeyFile>
</PropertyGroup>
```

So if `SignAssembly` is true and `AssemblyOriginatorKeyFile` contains a valid path to a key file then the resultant processed assembly will be strong named.