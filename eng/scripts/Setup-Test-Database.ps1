#publish db-migration
$previousLocation = Get-Location
$projectRoot = "$root\src\DBMigration"
$projectName = "db-migration"
$publishDir = "$projectRoot\bin\$BuildConfig\publish"

. $PSScriptRoot\Publish-CalendarServiceMigrator.ps1

$dataSource = "(local)"
$dbCatalog="Calendar"
# $dbUser = "${env:COMPUTERNAME}\${appPoolUsername}"
$dbUser = "${env:UserDomain}\${env:UserName}"

$dbMigrationPublishDir = "$(Get-Location)\src\DBMigration\bin\Debug\publish"

if ($null -eq (Invoke-Sqlcmd -HostName localhost -Database master -Query "SELECT 1 FROM sys.databases WHERE name='${dbCatalog}'")) {
    Invoke-Sqlcmd -HostName localhost -Database master -Query "CREATE DATABASE [${dbCatalog}]"
    Write-Host "Not found database, creating a new one: ${dbCatalog}"
}

dotnet run --project $projectRoot\$projectName.csproj "Data Source=$dataSource;Initial Catalog=$dbCatalog;Integrated Security=True;"
 
if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration Completed."
} else
{
    Write-Host "Migration Failed."
}