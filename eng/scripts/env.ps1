$ErrorActionPreference = "Stop"

Set-Variable 'root' $(Get-Location) -Scope Global

function Set-Var($name, $envName, $default) {
    if (!(Test-Path variable:$name)) {
        if (Test-Path env:$envName) {
            echo "set $name as $(Get-Content Env:$envName) from env:$envName"
            Set-Variable $name $(Get-Content Env:$envName) -Scope Global
        }
        else {
            Set-Variable $name $default -Scope Global
        }
    }
}

Set-Var 'BuildConfig' 'BuildConfig' 'Debug'
Set-Var 'ReleaseVersion' 'ReleaseVersion' 'dev'
