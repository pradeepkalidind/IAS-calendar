. ./eng/scripts/env.ps1

dotnet restore .\CalendarService.sln

dotnet build .\src\DBMigration\db-migration.csproj
if ($LASTEXITCODE -ne 0) { Write-Error 'build failed'; }

$targetDir = 'tmp\nupkgs'
if (-not (Test-Path $targetDir)) { New-Item $targetDir -Type Directory }

$version = "1.0.0-$ReleaseVersion"

# -NoPackageAnalysis
dotnet pack .\src\DBMigration\db-migration.csproj -c $BuildConfig -p:Version=$version -o tmp\nupkgs
if ($LASTEXITCODE -ne 0) { Write-Error 'nuget pack failed'; }

# push to azure artifacts
$source = "https://pkgs.dev.azure.com/vialto/myMobility-CoreApp/_packaging/nuget-for-pipeline/nuget/v3/index.json"
dotnet nuget push tmp/nupkgs/CalendarDB.$version.nupkg --source $source --api-key "azuredevops" --skip-duplicate
Write-Output "CalendarDB.$version" | Out-File -Encoding ASCII tmp/nupkgs/pkgs.txt