[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$PackageName,
    [Parameter(Mandatory = $true, Position = 1)]
    [string]$ProjectRoot
)
. ./eng/scripts/env.ps1
$previousLocation = Get-Location

# pack to zip
if (-not (Test-Path tmp\publish)) { New-Item tmp\publish -Type Directory }
$publishDir = "$ProjectRoot\bin\$BuildConfig\publish"
Write-Output "Pack application from $publishDir"
Get-ChildItem $publishDir

Compress-Archive $PublishDir\* tmp\publish\$PackageName.zip -Force

Write-Output "Pack to tmp\publish\$PackageName.zip successfully"
Set-Location $previousLocation