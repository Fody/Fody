param($installPath, $toolsPath, $package, $project)


function Set-NugetPackageRefAsDevelopmentDependency($package, $project)
{
	Write-Host "Set-NugetPackageRefAsDevelopmentDependency" 
    $packagesconfigPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "packages.config")
	$packagesconfig = [xml](get-content $packagesconfigPath)
	$packagenode = $packagesconfig.SelectSingleNode("//package[@id=`'$($package.id)`']")
	$packagenode.SetAttribute('developmentDependency','true')
	$packagesconfig.Save($packagesconfigPath)
}

$project.Save()

Set-NugetPackageRefAsDevelopmentDependency $package $project
