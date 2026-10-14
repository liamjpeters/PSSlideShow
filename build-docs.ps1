#requires -Version 5.1
<#
.SYNOPSIS
    Maintains the PlatyPS command help for PSSlideShow: (re)syncs the Markdown, tests it, and
    compiles the MAML external help that Get-Help consumes.

.DESCRIPTION
    Uses Microsoft.PowerShell.PlatyPS (v1+) to work with the Markdown help under docs/PSSlideShow:

      * -Update  imports the staged module and runs Update-MarkdownCommandHelp / Update-MarkdownModuleFile
                 so the Markdown picks up new commands, parameters, and syntax. Existing prose is kept;
                 PlatyPS leaves {{ ... }} placeholders for anything new that you must fill in by hand.
                 (PlatyPS writes .bak backups next to each updated file.)
      * always    tests every command Markdown file with Test-MarkdownCommandHelp and fails the run if
                 any file reports a Warning or Error diagnostic (unless -SkipTest).
      * always    compiles the Markdown to MAML (PSSlideShow.dll-Help.xml) with Export-MamlCommandHelp
                 and drops it into the <locale> folder next to each staged DLL, which is where Get-Help
                 looks for a binary cmdlet's external help (unless -SkipMaml).

    The module must be staged first (run build.ps1) so bin/<tfm>/PSSlideShow.dll exists.

.PARAMETER Update
    Re-sync the Markdown from the staged module before testing/compiling. Requires importing the
    module, which loads its native dependencies; run this from a throwaway pwsh session because those
    files stay locked until the process exits.

.PARAMETER Locale
    UI culture folder to write the MAML into. Get-Help resolves a binary cmdlet's external help from
    the folder matching $PSUICulture, so this defaults to the current UI culture. Override it (e.g.
    -Locale en-US) when producing help for a different culture.

.PARAMETER SkipTest
    Skip the Test-MarkdownCommandHelp validation pass.

.PARAMETER SkipMaml
    Skip compiling the MAML external help.
#>
[CmdletBinding()]
param(
    [switch] $Update,
    [string] $Locale = $PSUICulture,
    [switch] $SkipTest,
    [switch] $SkipMaml
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$docsDir = Join-Path $root 'docs/PSSlideShow'
$moduleManifest = Join-Path $root 'module/PSSlideShow.psd1'
$moduleFile = Join-Path $docsDir 'PSSlideShow.md'

$platyPS = Get-Module Microsoft.PowerShell.PlatyPS -ListAvailable |
Sort-Object Version -Descending |
Select-Object -First 1
if (-not $platyPS) {
    throw 'Microsoft.PowerShell.PlatyPS is not installed. Run: Install-PSResource -Name Microsoft.PowerShell.PlatyPS'
}
Import-Module $platyPS.Path -Force

# Enumerate the command Markdown files (everything except the module landing page).
function Get-CommandMarkdown {
    Measure-PlatyPSMarkdown -Path (Join-Path $docsDir '*.md') |
    Where-Object Filetype -Match 'CommandHelp'
}

if ($Update) {
    Write-Host '==> Importing staged module' -ForegroundColor Cyan
    Import-Module $moduleManifest -Force

    Write-Host '==> Updating command Markdown from the module' -ForegroundColor Cyan
    Get-CommandMarkdown | Update-MarkdownCommandHelp -Path { $_.FilePath } | Out-Null

    Write-Host '==> Updating module landing page' -ForegroundColor Cyan
    Get-CommandMarkdown |
    Import-MarkdownCommandHelp -Path { $_.FilePath } |
    Update-MarkdownModuleFile -Path $moduleFile | Out-Null

    Write-Warning 'Review the updated files for new {{ ... }} placeholders and delete the .bak backups when done.'
}

if (-not $SkipTest) {
    Write-Host '==> Testing command Markdown' -ForegroundColor Cyan
    $failed = @()
    foreach ($md in Get-CommandMarkdown) {
        $help = Import-MarkdownCommandHelp -Path $md.FilePath
        $problems = $help.Diagnostics.Messages | Where-Object Severity -In 'Warning', 'Error'
        if ($problems) {
            $failed += [pscustomobject]@{ File = Split-Path $md.FilePath -Leaf; Problems = $problems }
            foreach ($p in $problems) {
                Write-Warning ('{0}: [{1}] {2}' -f (Split-Path $md.FilePath -Leaf), $p.Severity, ($p.Message -join ' '))
            }
        }
    }
    if ($failed) {
        throw "Markdown help test failed for $($failed.Count) file(s). See the warnings above."
    }
    Write-Host '    all command Markdown files passed' -ForegroundColor DarkGray
}

if (-not $SkipMaml) {
    Write-Host '==> Compiling MAML external help' -ForegroundColor Cyan
    $commandHelp = Get-CommandMarkdown | Import-MarkdownCommandHelp -Path { $_.FilePath }

    # Export-MamlCommandHelp nests its output under an <OutputFolder>/<ModuleName>/ subfolder, so compile
    # once into a temp folder and then place the produced *-Help.xml where Get-Help expects it.
    $temp = Join-Path ([System.IO.Path]::GetTempPath()) ('psslideshow-maml-' + [guid]::NewGuid())
    New-Item -ItemType Directory -Path $temp -Force | Out-Null
    try {
        $commandHelp | Export-MamlCommandHelp -OutputFolder $temp -Force | Out-Null
        $maml = Get-ChildItem -Path $temp -Recurse -Filter '*-Help.xml' | Select-Object -First 1
        if (-not $maml) {
            throw 'Export-MamlCommandHelp did not produce a MAML file.'
        }

        # A binary cmdlet's external help is resolved relative to its assembly, so the MAML has to sit in
        # a <locale> folder next to each staged DLL. Staging wipes module/bin, so this runs after build.ps1.
        $tfmDirs = Get-ChildItem -Path (Join-Path $root 'module/bin') -Directory -ErrorAction SilentlyContinue |
        Where-Object { Test-Path (Join-Path $_.FullName 'PSSlideShow.dll') }
        if (-not $tfmDirs) {
            throw 'No staged PSSlideShow.dll found under module/bin. Run build.ps1 first.'
        }

        foreach ($tfm in $tfmDirs) {
            $target = Join-Path $tfm.FullName $Locale
            New-Item -ItemType Directory -Path $target -Force | Out-Null
            $dest = Join-Path $target 'PSSlideShow.dll-Help.xml'
            Copy-Item -LiteralPath $maml.FullName -Destination $dest -Force
            Write-Host "    wrote $dest" -ForegroundColor DarkGray
        }
    } finally {
        Remove-Item -LiteralPath $temp -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host '==> Docs done' -ForegroundColor Green
