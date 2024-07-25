[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$sonarUrl,
    [Parameter(Mandatory = $true)]
    [string]$sonarToken
)

Write-Host "Start to run sonar scan"
dotnet sonarscanner begin /k:IAS-calendar `
    /d:sonar.host.url="$sonarUrl" /d:sonar.login="$sonarToken" `
    /d:sonar.cs.dotcover.reportsPaths="TestResults/dotCover.Output.html" `
    /d:sonar.dotnet.excludeTestProjects=false
if(!$?) {
    Write-Error "failed to run sonar scan"
    exit 1
}

Write-Host "Start build to collect info"
dotnet build
if(!$?) {
    Write-Error "failed to build"
    exit 1
}

Write-Host "Start to run dotcover to collect coverage info"
dotnet dotcover test --dcOutput=TestResults/dotCover.Output.html --dcReportType=HTML --dcFilters="-:module=DBMigration;-:module=Calendar.API.Test;-:module=Calendar.Tests.Unit" -l trx --no-build -m:1
if(!$?) {
    Write-Error "failed to run dotcover"
    exit 1
}

Write-Host "End sonar scan, submiting report"
dotnet sonarscanner end /d:sonar.login="$sonarToken"
if(!$?) {
    Write-Error "failed to end sonar scan"
    exit 1
}