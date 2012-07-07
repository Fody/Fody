## NuGet Package 

Add-ins can be deployed through [NuGet](http://nuget.org/) packages. 

 * The package should contain one assembly in the root of the package. 
 * The name of the package and the name of the assembly should be the same and be suffixed with ".Fody". For example
 * The package should not have any nuget dependencies. (nuget dependencies are not supported yet)

For example

[VirtuosityNuget.png]


See [AddinSearchPaths]