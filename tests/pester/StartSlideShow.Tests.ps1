#requires -Version 5.1
# Pester v6 tests for the Start-SlideShow entry point (parameter binding & deck loading).
# The interactive host itself is exercised by the C# xUnit tests (scripted input source); here we
# validate the cmdlet's command metadata and the -Path deck-loading path without entering the UI.

BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $manifestPath = Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path
    Import-Module $manifestPath.Path -Force
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

Describe 'Start-SlideShow' {
    It 'is exported as a cmdlet' {
        $command = Get-Command -Name 'Start-SlideShow' -Module PSSlideShow -ErrorAction SilentlyContinue
        $command | Should-NotBeNull
        $command.CommandType | Should-Be 'Cmdlet'
    }

    It 'accepts slides from the pipeline' {
        $command = Get-Command -Name 'Start-SlideShow'
        $slides = $command.Parameters['Slides']
        $slides | Should-NotBeNull
        ($slides.Attributes.ValueFromPipeline -contains $true) | Should-BeTrue
    }

    It 'exposes -Path, -StartAt and -NoAlternateScreen parameters' {
        $command = Get-Command -Name 'Start-SlideShow'
        $command.Parameters.ContainsKey('Path') | Should-BeTrue
        $command.Parameters.ContainsKey('StartAt') | Should-BeTrue
        $command.Parameters.ContainsKey('NoAlternateScreen') | Should-BeTrue
    }

    It 'exposes a -ContentPadding parameter' {
        $command = Get-Command -Name 'Start-SlideShow'
        $command.Parameters.ContainsKey('ContentPadding') | Should-BeTrue
        $command.Parameters['ContentPadding'].ParameterType | Should-Be ([int])
    }

    It 'warns when there are no slides to show' {
        $warnings = $null
        Start-SlideShow -Slides @() -NoAlternateScreen -WarningAction SilentlyContinue -WarningVariable warnings
        $warnings | Should-NotBeNull
    }

    It 'runs -Path as a file with the deck script context' {
        $deckDir = Join-Path $TestDrive 'deck files'
        New-Item -ItemType Directory -Path $deckDir | Out-Null
        $deckPath = Join-Path $deckDir "team's deck.ps1"
        $markerPath = Join-Path $deckDir 'resolved.txt'
        Set-Content -LiteralPath $deckPath -Value @'
if ($PSCommandPath -ne $MyInvocation.MyCommand.Path) {
    throw 'The deck must run as a script file.'
}
[System.IO.File]::WriteAllText((Join-Path $PSScriptRoot 'resolved.txt'), $PSCommandPath)
'@

        Start-SlideShow -Path $deckPath -WarningAction SilentlyContinue

        (Get-Content -LiteralPath $markerPath -Raw) | Should-Be $deckPath
    }

    It 'loads full help from the staged binary module' {
        $help = Get-Help Start-SlideShow -Full
        ($help.Description | Out-String) | Should-MatchString -Expected 'keyboard-driven'
    }
}
