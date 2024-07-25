. ./eng/scripts/env.ps1
$previousLocation = Get-Location
$projectRoot = "$root\src\calendar-service"
$projectName = "calendar-service"
$publishDir = "$projectRoot\bin\$BuildConfig\publish"

if (-not $NoRestore)
{
    dotnet restore $projectRoot\$projectName.csproj
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error 'restore failed';
    }
}

# build project
dotnet publish $projectRoot\$projectName.csproj -c $BuildConfig -o $publishDir --version-suffix $ReleaseVersion --verbosity m
if ($LASTEXITCODE -ne 0)
{
    Write-Error 'dotnet publish failed';
}

# set version
$appConfigFile = "$publishDir\Calendar.Service.dll.config"
$appConfig = [xml](Get-Content $appConfigFile)
$appConfig.SelectSingleNode("//appSettings/add[@key='ApplicationVersion']").value = "$ReleaseVersion"
$appConfig.Save($appConfigFile)

Write-Output "Published to $publishDir successfully"

Set-Location $previousLocation