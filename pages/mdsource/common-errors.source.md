# Common Errors


## Building from a network share

Building from a network share will result in the following exception:

```
Could not load file or assembly 'file:///S:\a\b\c\d.dll' or one of its dependencies.
The parameter is incorrect. (Exception from HRESULT: 0x80070057 (E_INVALIDARG))
```

This is due to the security restriction put in place by MSBuild.

Work around this problem by adding `<loadFromRemoteSources enabled="true"/>` to the `MSBuild.exe.config` file of the build machine.

It is theoretically possible for Fody to work around this problem by copying files locally before loading them. However considering the negative effects on build time due to decreased IO building from a network share is not a recommended practice. As such Fody will not be supporting this approach.


## Task could not be loaded

If the below error occurs it is one of two problems:

```
error MSB4036: The "Fody.WeavingTask" task was not found...
```


### Visual Studio is being difficult

For a small number of users Visual Studio refuses to pick up the build task. The only workaround is to restart Visual Studio.

Unfortunately there is nothing that can be done to fix this in Fody as it occurs before any interaction with the Fody implementation.


### Invalid symbols in the solution path

There is a bug in MSBuild that causes it to inject incorrect path variables when there certain symbols in the directory path containing the solution.

So for example the solution is located at:

```
G:\Code\@MyClassLibrary\MyClassLibrary.sln
```

MSBuild will incorrectly substitute `$(SolutionDir)` with:

```
G:\Code\%40MyClassLibrary\
```

Instead of:

```
G:\Code\@MyClassLibrary\
```

There is nothing that can be done to fix this in Fody. If this error occurs the only option is to remove the symbols from the solution path.


## Edit and continue

Edit and continue is not supported. There is no extension point to re-weave an assembly after the edit part.