If you get the below error you have one of two problems

    error MSB4036: The "Fody.WeavingTask" task was not found...

## Visual Studio is being difficult 

For a small number of users Visual Studio refuses to pick up the build task until you restart Visual Studio. 

Unfortunately there is nothing I can do to fix this issue as it occurs before my code is reached.

## You have symbols in your path 

There is a bug in MSBuild that causes it to inject incorrect path variables when you have certain symbols in the directory path containing your solution.

### How it manifests 

So for example if your solution is located at 

`G:\Code\@MyClassLibrary\MyClassLibrary.sln`

And you have this code in `MyClassLibrary.csproj`


    <UsingTask TaskName="Fody.WeavingTask" 
    AssemblyFile="$(SolutionDir)Tools\Fody\Fody.dll" />
     <Target Name="AfterCompile">
      <Fody.WeavingTask />
    </Target>

MSBuild will incorrectly substitute `$(SolutionDir)` with

`G:\Code\%40MyClassLibrary\` 

instead of 

`G:\Code\@MyClassLibrary\`

### The workaround 

As with the previous error there is nothing I can do about this. If you hit this issue the only option you have is to remove the symbols from your path.