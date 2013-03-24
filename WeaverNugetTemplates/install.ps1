param($installPath, $toolsPath, $package, $project)


function RemoveForceProjectLevelHack($project)
{
	Foreach ($item in $project.ProjectItems) 
	{
		if ($item.Name -eq "Fody_ToBeDeleted.txt")
		{
			$item.Delete()
		}
	}
}

function Update-FodyConfig($addinName, $project)
{
	
    $fodyWeaversPath = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($project.FullName), "FodyWeavers.xml")

	$FodyLastProjectPath = $env:FodyLastProjectPath
	$FodyLastWeaverName = $env:FodyLastWeaverName
	$FodyLastXmlContents = $env:FodyLastXmlContents
	
	if (
		($FodyLastProjectPath -eq $project.FullName) -and 
		($FodyLastWeaverName -eq $addinName))
	{
		[System.IO.File]::WriteAllText($fodyWeaversPath, $FodyLastXmlContents)
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