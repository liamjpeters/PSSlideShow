# PSSlideShow module loader.
# Selects the correct target-framework build for the current PowerShell edition, then loads the binary
# module in two steps so its private dependencies stay isolated:
#   1. Load the assembly (metadata only) and call ModuleInitializer.Initialize(). That method touches
#      only framework/AssemblyLoadContext types, so it runs before any private dependency is needed and
#      installs the resolver (a dedicated AssemblyLoadContext on .NET; an AssemblyResolve handler +
#      native preload on .NET Framework).
#   2. Import the module by path. Enumerating the cmdlets reflects over parameters that use
#      Spectre.Console types (e.g. Justify), which now resolve through the isolated context instead of
#      leaking into the host. Importing by path (rather than by Assembly object) keeps re-imports with
#      -Force working correctly.
$ErrorActionPreference = 'Stop'

$isCore = $PSVersionTable.PSEdition -eq 'Core'
$tfm = if ($isCore) { 'net10.0' } else { 'net472' }
$assemblyPath = Join-Path $PSScriptRoot "bin/$tfm/PSSlideShow.dll"

if (-not (Test-Path -LiteralPath $assemblyPath)) {
    throw "PSSlideShow: binary not found for '$tfm' at '$assemblyPath'. Run build.ps1 first."
}

[System.Reflection.Assembly]::LoadFrom($assemblyPath) | Out-Null
[PSSlideShow.Hosting.ModuleInitializer]::Initialize()
Import-Module -Name $assemblyPath



