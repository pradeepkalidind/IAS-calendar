param(
	[string] $config_path,
	[string[]] $features
)

$packageRoot = $MyInvocation.MyCommand.Path | Split-Path -parent

Function Install-CalendarDatabase {
	Import-Module $packageRoot\tools\ps-utils -force

	if(!$config_path) {
		$config_path = "$packageRoot/config.ini"
	}

	$config = Import-Config $config_path
	$dbName = $config.dbName

	if (($features -ne $null)  -and $features.Contains('copy-files')) {
	    Write-Host "start copy files for [$dbName]..."
		$configFile = "$packageRoot/config.ini"
		if($configFile -and (Test-Path $configFile)){
		    Clear-Content $configFile
		    foreach ($key in $config.keys) {
		        if ($key -ne "CopyFile") {
		            $value = $config.Get_Item($key)
		            Add-Content $configFile "$key=$value"
		        }
		    }
		}

		$sourcePath = Split-Path $configFile -Parent
		$physicalPath = $config.physicalPath
		if($sourcePath -ne $physicalPath){
		     Clear-Directory $physicalPath | Out-Null
		     Copy-Item "$sourcePath\*" -Destination $physicalPath -Recurse
		}

		Write-Host "Content of DB [$dbName] is updated."
	} else {
		Write-Host "Start install [$dbName]..."
		. $packageRoot/install-commons.ps1
		$dbServer = $config.dbServer
		$dbUsers = $config.dbUsers
		$password = $config.password
		$resetDb = [System.Convert]::ToBoolean($config.resetDb)
		Deploy-DB $packageRoot $dbServer $dbName $dbUsers $password $resetDb
	}



}
Install-CalendarDatabase | Out-Default