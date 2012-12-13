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

try 
{
		echo "Installing Fody"
		$tempVsixPath = join-path $env:temp FodyVsPackage.vsix

		if( test-path $tempVsixPath ) 
		{
				rm -force $tempVsixPath 
		}
			
		(new-object net.webclient).DownloadFile("http://visualstudiogallery.msdn.microsoft.com/074a2a26-d034-46f1-8fe1-0da97265eb7a/file/62042/32/FodyVsPackage.vsix", $tempVsixPath)
		
		$vsixInstallerPath = GetVSIXInstallerPath
		Start-Process -FilePath $vsixInstallerPath -ArgumentList "/q $tempVsixPath" -Wait -PassThru;

		Write-ChocolateySuccess "Fody"
} 
catch 
{
		Write-ChocolateyFailure "Fody" "$($_.Exception.Message)"
		throw
}
