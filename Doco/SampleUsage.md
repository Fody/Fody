# Sample Usage


## Prerequisites 

  * [Fody Visual Studio package](http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a) 
  * [NuGet Visual Studio package](http://visualstudiogallery.msdn.microsoft.com/27077b70-9dad-4c64-adcf-c7cf6bc9970c) (required to consume addins via nuget)

The above two steps need only be done once for a machine.

  * Open your project in Visual Studio
  * Select the project in Solution Explorer
  * Enable Fody for the project by using the top level menu 'Project > Fody > Configure'. Click OK. 
  * Notice a file 'FodyWeavers.xml' has been added to the project
  * Install a Fody add-in using NuGet. See http://docs.nuget.org/docs/start-here/using-the-package-manager-console for more info. For example Virtuosity (http://code.google.com/p/virtuosity/)

    `Install-Package Virtuosity.Fody`

  * Add the add-in to 'FodyWeavers.xml'


    <?xml version="1.0" encoding="utf-8" ?>
    <Weavers>
      <Virtuosity/>
    </Weavers>


  * Build.
    

  