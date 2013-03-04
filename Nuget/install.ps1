param($installPath, $toolsPath, $package, $project)


function RemoveForceProjectLevelHack($project)
{
	if (Test-Path "content/Fody_ToBeDeleted.txt")
	{
		$itemToRemove = $project.ProjectItems.Item("Fody_ToBeDeleted.txt")	
		$itemToRemove.Delete()
	}	
}

function Update-FodyConfig($addinName, $project)
{
	
    $fodyWeaversPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "FodyWeavers.xml")

    if (!(Test-Path ($fodyWeaversPath)))
    {
        Throw "Could not find FodyWeavers.xml in this project. Please enable Fody for this projet http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a"
    }   

	$FodyLastProjectPath = $env:FodyLastProjectPath
	$FodyLastWeaverName = $env:FodyLastWeaverName
	$FodyLastXmlContents = $env:FodyLastXmlContents
	
	if (
		($FodyLastProjectPath -eq $project.FullName) -and 
		($FodyLastWeaverName -eq $addinName))
	{
		$FodyLastXmlContents > $fodyWeaversPath
		return
	}
	
    $xml = [xml](get-content $fodyWeaversPath)

    $weavers = $xml["Weavers"]
    $node = $weavers.SelectSingleNode($addinName)

    if (-not $node)
    {
        $newNode = $xml.CreateElement($addinName)
        $weavers.AppendChild($newNode)
    }

    $xml.Save($fodyWeaversPath)
}

function Fix-ReferencesCopyLocal($package, $project)
{
    $asms = $package.AssemblyReferences | %{$_.Name}

    foreach ($reference in $project.Object.References)
    {
        if ($asms -contains $reference.Name + ".dll")
        {
            if($reference.CopyLocal -eq $true)
            {
                $reference.CopyLocal = $false;
            }
        }
    }
}

RemoveForceProjectLevelHack $project

Update-FodyConfig $package.Id.Replace(".Fody", "") $project

Fix-ReferencesCopyLocal $package $project