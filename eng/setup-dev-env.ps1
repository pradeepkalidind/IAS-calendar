
if ($PSVersionTable.PSVersion.Major -ne 5) {
    throw "Only support powershell 5.x"
}

Import-Module WebAdministration
Import-Module SQLPS

$appPoolName = "Calendar-local"
$appPoolDotNetVersion = "v4.0"  
$appPoolUsername = 'Calendar-local'
$appPoolPassword = 'TWr0ys1ngh4m'

$iisWebsitePublishFolderPath = "$(Get-Location)\src\calendar-service\bin\Debug\publish"
$iisWebsitePhysicalPath = "C:\IIS\CalendarService-local"
$iisWebsiteName = "CalendarService-local"

$dataSource = "(local)"
$dbCatalog="Calendar"
$dbUser = "${env:COMPUTERNAME}\${appPoolUsername}"
$dbMigrationPublishDir = "$(Get-Location)\src\DBMigration\bin\Debug\publish"

iisreset.exe /stop

if(Test-Path $iisWebsitePhysicalPath){
    Write-Host "Folder Exists"
    Remove-Item $iisWebsitePhysicalPath -Force -Recurse
}

. $PSScriptRoot\scripts\Publish-CalendarService.ps1
Copy-Item -Path $iisWebsitePublishFolderPath -Destination $iisWebsitePhysicalPath -PassThru -Recurse

try {
    #    Setup IIS
    if ($null -eq (Get-LocalUser | Where-Object {$_.Name -eq $appPoolUsername})) {
        New-LocalUser -Name $appPoolUsername -AccountNeverExpires -PasswordNeverExpires -Password $(ConvertTo-SecureString $appPoolPassword -AsPlainText -Force) -FullName $appPoolUsername -Description "Created by PowerShell"
        Write-Host "Not found app pool user, create app pool user: ${appPoolUsername}"
    }
    
    $appPoolUser = Get-LocalUser -Name $appPoolUsername
    if ($null -ne $appPoolUser.PasswordExpires) {
        Set-LocalUser -Name $appPoolUsername -PasswordNeverExpires $true
        Write-Host "Set app pool user password never expires"
    }
    
    if ($null -eq (Get-WebAppPoolState | Where-Object {$_.ItemXPath.Contains($appPoolName)})) {
        Write-Host "Not found app pool: ${appPoolName}"
        New-WebAppPool -Name $appPoolName
    }
    
    $appPoolProcessModel = Get-ItemProperty -Path "IIS:\AppPools\${appPoolName}" -Name processModel
    if (($appPoolProcessModel.identityType -ne 'SpecificUser') -or ($appPoolProcessModel.userName -eq '') -or ($appPoolProcessModel.password -eq '')) {
        Set-ItemProperty -Path "IIS:\AppPools\${appPoolName}" -Name processModel -Value @{userName=$appPoolUsername;password=$appPoolPassword;identitytype=3}
        Write-Host "Set app pool identity to specific user: ${appPoolUsername}"
    }

    if (!(Test-Path IIS:\Sites\$iisWebsiteName -pathType container))  {
        New-WebSite -Name $iisWebsiteName -Port 10910 -IPAddress '*' -PhysicalPath $iisWebsitePhysicalPath -ApplicationPool $appPoolName
        Write-Host "Set app site ApplicationPool to specific site: ${iisAppPoolName}"
    }
    
    # Setup db
    . $PSScriptRoot\scripts\Publish-CalendarServiceMigrator.ps1

    if ($null -eq (Invoke-Sqlcmd -HostName localhost -Database master -Query "SELECT 1 FROM sys.databases WHERE name='${dbCatalog}'")) {
        Invoke-Sqlcmd -HostName localhost -Database master -Query "CREATE DATABASE [${dbCatalog}]"
        Write-Host "Not found database, creating a new one: ${dbCatalog}"
    }

    if ($null -eq (Invoke-Sqlcmd -HostName localhost -Database master -Query "SELECT 1 FROM sys.server_principals WHERE name='${dbUser}' AND type='U' ")) {
        Invoke-Sqlcmd -HostName localhost -Database master -Query "CREATE LOGIN [${dbUser}] FROM WINDOWS"
        Write-Host "Not found db login, creating a new one: ${dbUser}"
    }

    if ($null -eq (Invoke-Sqlcmd -HostName localhost -Database $dbCatalog -Query "SELECT 1 FROM sysusers WHERE name='${dbUser}'")) {
        Invoke-Sqlcmd -HostName localhost -Database $dbCatalog -Query "CREATE USER [${dbUser}] FOR LOGIN [${dbUser}]"
        Invoke-Sqlcmd -HostName localhost -Database $dbCatalog -Query "ALTER USER [${dbUser}] WITH DEFAULT_SCHEMA=[dbo]"
        Invoke-Sqlcmd -HostName localhost -Database $dbCatalog -Query "EXEC sp_addrolemember N'db_owner',[${dbUser}]"
        Write-Host "Not found db user, creating a new one: ${dbUser}"
    }

    iex "$dbMigrationPublishDir\Tools\Migrate.exe -a $dbMigrationPublishDir\DBMigration.dll -db sqlserver2014 -conn ""Data Source=$dataSource;Initial Catalog=$dbCatalog;Integrated Security=True;"" -profile ""Debug"""

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration Completed."
    } else
    {
        Write-Host "Migration Failed."
    }
}
finally {
    iisreset.exe /start
    Pop-Location
}