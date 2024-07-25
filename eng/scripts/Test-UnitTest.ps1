. ./eng/scripts/env.ps1
$previousLocation = Get-Location
$projectRoot = "$root\test\unit-test"
$projectName = "unit-test"

Write-Host "`Run tests ..." -f Green
dotnet restore $projectRoot\$projectName.csproj
dotnet dotcover test --dcOutput=TestResults\dotCover.Output.html --dcReportType=HTML $projectRoot\$projectName.csproj
if ($LASTEXITCODE -ne 0)
{
    Write-Error 'Unit Test failed';
}