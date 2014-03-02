param($installPath, $toolsPath, $package, $project)


# Need to load MSBuild assembly if it's not loaded yet.
Add-Type -AssemblyName 'Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
  
# Grab the loaded MSBuild project for the project
$buildProject = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($project.FullName) | Select-Object -First 1

$fodyPathProperty = $buildProject.GetProperty("FodyPath") 

if ($fodyPathProperty)
{
	$buildProject.RemoveProperty($fodyPathProperty);
}

$project.Save()

