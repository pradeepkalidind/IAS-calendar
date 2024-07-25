$root = $MyInvocation.MyCommand.Path | Split-Path -parent

. $root\db-config.ps1

iex "sqlcmd -E -S ""$dataSource"" -d master  -i ""$root\application_db.sql"" -v ApplicationDatabaseName=""$initialCatalog"" -v ApplicationDatabaseUser=""$user"" -v ApplicationDatabasePwd=""$password"""

& "dotnet" "publish" "$root\db-migration.csproj"
$packageRoot = "$root\bin\debug\netstandard2.0\publish"
$connectionString = "Data Source=$($dataSource);Initial Catalog=$($initialCatalog);Integrated Security=True;Connect Timeout=1200;"
$binFile = Get-ChildItem $packageRoot -Include "DBMigration.dll" -Recurse
& "dotnet" "tool" "install" "--tool-path" "./" "FluentMigrator.DotNet.Cli" "--version" "3.3.2"
& "./dotnet-fm.exe" "rollback" "all" "-p" "sqlserver2014" "-c" $connectionString "-a" $binFile.FullName --task rollback:all
& "./dotnet-fm.exe" "migrate" "-p" "sqlserver2014" "-c" $connectionString "-a" $binFile.FullName

if ($LASTEXITCODE -eq 0) {
    Write-Host "Migration Completed."
} else
{
    Write-Host "Migration Failed."
}