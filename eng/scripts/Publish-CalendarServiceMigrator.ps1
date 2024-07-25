. ./eng/scripts/env.ps1
$previousLocation = Get-Location
$projectRoot = "$root\src\DBMigration"
$projectName = "db-migration"
$publishDir = "$projectRoot\bin\$BuildConfig\publish"

if (-not $NoRestore)
{
    dotnet restore $projectRoot\$projectName.csproj
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error 'restore failed';
    }
}

dotnet publish $projectRoot\$projectName.csproj -c $BuildConfig -o $publishDir --version-suffix $ReleaseVersion --verbosity m
if ($LASTEXITCODE -ne 0)
{
    Write-Error 'msbuild failed';
}

Write-Output "Published to $publishDir successfully"

Set-Location $previousLocation