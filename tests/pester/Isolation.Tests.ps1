#requires -Version 5.1
# Pester v6 tests for M4 dependency isolation.
# On PowerShell 7 the module's private dependencies must load into a dedicated AssemblyLoadContext so
# they cannot clash with the host or other modules; on both editions the dependencies must live in a
# 'dependencies' subfolder rather than next to the module assembly.

$script:IsCoreEdition = $PSVersionTable.PSEdition -eq 'Core'

BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $script:ManifestPath = (Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path).Path
    $script:ModuleRoot = Split-Path $script:ManifestPath
    Import-Module $script:ManifestPath -Force
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

Describe 'Dependency isolation' {
    It 'imports the module and runs a cmdlet' {
        $widget = New-SlideText 'hello'
        $widget | Should-NotBeNull
    }

    It 'loads Spectre.Console into a dedicated AssemblyLoadContext' -Skip:(-not $script:IsCoreEdition) {
        $asm = [System.Reflection.Assembly]::Load('Spectre.Console')
        $context = [System.Runtime.Loader.AssemblyLoadContext]::GetLoadContext($asm)
        $context.Name | Should-Be 'PSSlideShow'
    }

    It 'loads the native syntax highlighter through the staged module' {
        $lines = [PSSlideShow.Rendering.SyntaxHighlighter]::HighlightLines('Get-Process', 'powershell')
        ($lines -join "`n") | Should-MatchString -Expected '\[#'
    }

    It 'ships private dependencies in a dependencies subfolder, not beside the module' {
        $tfm = if ($script:IsCoreEdition) { 'net10.0' } else { 'net472' }
        $binDir = Join-Path $script:ModuleRoot "bin/$tfm"
        Test-Path (Join-Path $binDir 'Spectre.Console.dll') | Should-BeFalse
        Test-Path (Join-Path $binDir 'dependencies/Spectre.Console.dll') | Should-BeTrue
    }
}
