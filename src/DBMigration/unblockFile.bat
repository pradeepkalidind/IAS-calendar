@echo off
cmd /c echo ../ > %~dp0%db-config.ps1:Zone.Identifier
for %%f in (*.ps*) do cmd /c echo . > %%f:Zone.Identifier
