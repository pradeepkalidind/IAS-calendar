. ./eng/scripts/env.ps1
$previousLocation = Get-Location
$projectRoot = "$root\test\api-test"
$projectName = "api-test"

Write-Host "`Run tests ..." -f Green
dotnet restore $projectRoot\$projectName.csproj
dotnet test $projectRoot\$projectName.csproj
if ($LASTEXITCODE -ne 0)
{
    Write-Error 'API Test failed';
}