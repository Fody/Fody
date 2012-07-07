# How to manually add to a project

**Setup can be simplified by installing the [Visual Studio package](http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a)**


However if you want to manually set up Fody...

  * Get the latest download http://code.google.com/p/fody/downloads/list
  * Unzip the download. Place the files from 'MSBuild' where Visual Studio can find it. In my case it is in a directory called _Tools\Fody_ at the root of my solution. 
  * Add a the `Fody.WeavingTask` to your project.
  
  eg
  
    <Project>
     ...
      <UsingTask 
          TaskName="Fody.WeavingTask"
          AssemblyFile="$(SolutionDir)Tools\Fody\Fody.dll" />
      <Target Name="AfterCompile">
        <Fody.WeavingTask AssemblyPath="@(IntermediateAssembly)" SolutionDir="$(SolutionDir)" ProjectPath="$(ProjectPath)" References="@(ReferencePath)" />
      </Target>
    </Project>
  
  * Add a "FodyWeavers.xml" to the project. See [SampleUsage]
  * Build. 