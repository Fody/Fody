param($installPath, $toolsPath, $package, $project)


function Fix-ReferencesCopyLocal($package, $project)
{
    Write-Host "Fix-ReferencesCopyLocal $($package.Id)"
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

Fix-ReferencesCopyLocal $package $project