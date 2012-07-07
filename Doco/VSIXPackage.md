#How the VSIX Package works

## Installing the VSIX Package 

The package can be obtained by

 * Installing from the [Visual Studio Gallery](http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a) 
 * From within Visual Studio by searching for "Fody" in the "Online" tab of "Extension Manager"

## Keeping the VSIX Package up-to-date 

In Visual Studio go to 

`Tools > Options > Environment > Extension Manager`

Tick “Automatically check for updates to installed extensions"

## How the VSIX Package updates Fody.dll 

Visual Studio locks assemblies containing MSBuild Tasks when a solution is open. This makes updating `Fody.dll` to the latest version difficult. 
The VSIX Package takes the following approach

 * When a solution is opened all projects are checked to see if they use `Fody.dll`
 * If any of these need to be updated the path is stored in the users app data under 
   `%appdata%\Fody\TaskAssembliesToUpdate.txt`
 * The next time Visual Studio is opened the VSIX Package checks `TaskAssembliesToUpdate.txt` and attempts to update all the files listed there. To cope with multiple instances of Visual Studio being open locked files are skipped and tried latter