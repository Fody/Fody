If you are seeing this page then one of your projects has been updated to the new Fody nuget.

Steps you should follow

* Ensure the Fody nuget has been applied to all projects in the current solution. 
  The nuget will take care of converting projects from the VSIX approach to the nuget package approach.
* Uninstall the Fody VSIX. Moving forward it will not be required.
* Manually delete `Tools/Fody` from both your local copy and source control. 
  You will need to close Visual Studio beforehand since the files will be locked.

For the latest version of this file go here https://github.com/Fody/Fody/blob/master/NuGet/readme.txt