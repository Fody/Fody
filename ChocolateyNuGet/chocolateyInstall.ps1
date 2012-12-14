function GetVSIXInstallerPath()
{
    if( test-path $env:VS110COMNTOOLS ) 
		{
				return join-path $env:VS110COMNTOOLS "../IDE/VSIXInstaller.exe"
		}
    if( test-path $env:VS100COMNTOOLS ) 
		{
				return join-path $env:VS100COMNTOOLS "../IDE/VSIXInstaller.exe"
		}
		throw "Could not find VS100COMNTOOLS or VS110COMNTOOLS environment variables"
}

function Get-Script-Directory
{
    $scriptInvocation = (Get-Variable MyInvocation -Scope 1).Value
    return Split-Path $scriptInvocation.MyCommand.Path
}
try 
{
		echo "Installing Fody"
		$vsixPath = join-path Get-Script-Directory FodyVsPackage.vsix

		$vsixInstallerPath = GetVSIXInstallerPath
		Start-Process -FilePath $vsixInstallerPath -ArgumentList "/q $vsixPath" -Wait -PassThru;

		Write-ChocolateySuccess "Fody"
} 
catch 
{
		Write-ChocolateyFailure "Fody" "$($_.Exception.Message)"
		throw
}
