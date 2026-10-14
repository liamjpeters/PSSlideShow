#requires -Version 5.1
<#
.SYNOPSIS
    Restores, builds (both TFMs), stages the module, and runs the tests.

.DESCRIPTION
    One entry point for the whole build:
      * dotnet build for net10.0 and net472
      * stage the compiled binaries + dependencies into module/bin/<tfm>
      * include the MIT license in the staged module
      * compile and stage Get-Help for both builds (validate Markdown with -Docs)
      * run the xUnit suite (unless -SkipDotNetTests)
      * run Pester against the staged module in PowerShell 7 and, on Windows, 5.1
        (unless -SkipPesterTests)

.PARAMETER Configuration
    Debug or Release. Default: Release.

.PARAMETER SkipTests
    Skip both test suites.
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $SkipDotNetTests,
    [switch] $SkipPesterTests,
    [switch] $SkipTests,

    # Run PlatyPS validation as well as the default MAML help compilation.
    [switch] $Docs
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$moduleProject = Join-Path $root 'src/PSSlideShow/PSSlideShow.csproj'
$testProject = Join-Path $root 'tests/PSSlideShow.Tests/PSSlideShow.Tests.csproj'
$moduleDir = Join-Path $root 'module'
$stageDir = Join-Path $moduleDir 'bin'

Write-Host '==> Restoring & building' -ForegroundColor Cyan

dotnet build $moduleProject -c $Configuration

if ($LASTEXITCODE -ne 0) {
    throw "Build failed ($LASTEXITCODE)."
}

Write-Host '==> Staging module binaries' -ForegroundColor Cyan

if (Test-Path $stageDir) {
    Remove-Item $stageDir -Recurse -Force
}

foreach ($tfm in @('net10.0', 'net472')) {
    $src = Join-Path $root "src/PSSlideShow/bin/$Configuration/$tfm"

    if (-not (Test-Path $src)) {
        Write-Warning "No build output for $tfm at $src (skipping)."
        continue
    }

    # The module assembly stays in bin/<tfm>; its private dependencies go into a
    # bin/<tfm>/dependencies subfolder that the default assembly probing does NOT search, so the
    # module's ModuleInitializer is the only thing that loads them (into an isolated
    # AssemblyLoadContext on .NET). This keeps Spectre.Console / TextMateSharp / System.Text.Json
    # from clashing with the host or other modules.
    $dest = Join-Path $stageDir $tfm
    $depDest = Join-Path $dest 'dependencies'
    New-Item -ItemType Directory -Path $dest -Force | Out-Null
    New-Item -ItemType Directory -Path $depDest -Force | Out-Null

    Get-ChildItem -LiteralPath $src -File | ForEach-Object {
        $target = if ($_.Name -like 'PSSlideShow.*') { $dest } else { $depDest }
        Copy-Item -LiteralPath $_.FullName -Destination $target -Force
    }

    # Native dependencies (runtimes/<rid>/native) ship alongside the managed dependencies.
    $srcRuntimes = Join-Path $src 'runtimes'
    if (Test-Path $srcRuntimes) {
        Copy-Item -Path $srcRuntimes -Destination (Join-Path $depDest 'runtimes') -Recurse -Force
    }

    Write-Host "    staged $tfm" -ForegroundColor DarkGray
}

# The Onigwrap native libraries (used by TextMateSharp for syntax highlighting) are only emitted
# under the .NET (Core) build's runtimes/<rid>/native folder; the .NET Framework build does not copy
# them. They are identical across target frameworks, so mirror them into the net472 dependencies too
# so the module loader can find libonigwrap.dll under Windows PowerShell 5.1.
$coreRuntimes = Join-Path $stageDir 'net10.0/dependencies/runtimes'
$fxDeps = Join-Path $stageDir 'net472/dependencies'
if ((Test-Path $coreRuntimes) -and (Test-Path $fxDeps)) {
    $fxRuntimes = Join-Path $fxDeps 'runtimes'
    if (-not (Test-Path $fxRuntimes)) {
        Copy-Item -Path $coreRuntimes -Destination $fxRuntimes -Recurse -Force
        Write-Host '    mirrored native runtimes into net472' -ForegroundColor DarkGray
    }
}

Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination $moduleDir -Force

Write-Host '==> Staging command help' -ForegroundColor Cyan
& (Join-Path $root 'build-docs.ps1') -SkipTest:(-not $Docs)

if (-not $SkipTests -and -not $SkipDotNetTests) {
    Write-Host '==> Running xUnit tests' -ForegroundColor Cyan
    dotnet test $testProject -c $Configuration --nologo
    if ($LASTEXITCODE -ne 0) { throw "xUnit tests failed ($LASTEXITCODE)." }
}

if (-not $SkipTests -and -not $SkipPesterTests) {
    $pesterRunner = Join-Path $root 'tests/run-pester.ps1'
    Write-Host '==> Running Pester tests in PowerShell 7' -ForegroundColor Cyan
    & pwsh -NoProfile -NonInteractive -File $pesterRunner
    if ($LASTEXITCODE -ne 0) { throw "PowerShell 7 Pester tests failed ($LASTEXITCODE)." }

    if ([Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT) {
        $windowsPowerShell = Join-Path $env:WINDIR 'System32/WindowsPowerShell/v1.0/powershell.exe'
        if (-not (Test-Path -LiteralPath $windowsPowerShell)) {
            throw "Windows PowerShell 5.1 not found at $windowsPowerShell."
        }
        Write-Host '==> Running Pester tests in Windows PowerShell 5.1' -ForegroundColor Cyan
        & $windowsPowerShell -NoProfile -NonInteractive -File $pesterRunner
        if ($LASTEXITCODE -ne 0) { throw "Windows PowerShell 5.1 Pester tests failed ($LASTEXITCODE)." }
    }
}

Write-Host '==> Done' -ForegroundColor Green
