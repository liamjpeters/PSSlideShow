#requires -Version 5.1
# Pester v6 tests for the module manifest and its exported surface.

BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $manifestPath = Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path
    Import-Module $manifestPath.Path -Force
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

Describe 'Module manifest' {
    It 'is a valid manifest' {
        $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
        $manifestPath = Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path
        $manifest = Test-ModuleManifest -Path $manifestPath.Path -ErrorAction Stop
        $manifest.Name | Should-Be 'PSSlideShow'
    }

    It 'stages the MIT license with the module' {
        $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
        $source = Join-Path $repoRoot 'LICENSE'
        $staged = Join-Path (Join-Path $repoRoot 'module') 'LICENSE'
        Test-Path -LiteralPath $staged | Should-BeTrue
        (Get-Content -LiteralPath $staged -Raw) | Should-Be (Get-Content -LiteralPath $source -Raw)
    }

    It 'exports <_>' -ForEach @(
        'Start-SlideShow'
        'New-Slide'
        'New-SlideText'
        'New-SlideFiglet'
        'New-SlideList'
        'New-SlideTable'
        'New-SlideCode'
        'New-SlideColumns'
        'New-SlideColumn'
        'New-SlideRows'
        'New-SlideRow'
        'New-SlideTree'
        'New-SlideTreeNode'
        'New-SlideBarChart'
        'New-SlideBreakdownChart'
        'New-SlideCalendar'
        'New-SlideCanvas'
        'New-SlideQrCode'
        'New-SlideDynamic'
    ) {
        $command = Get-Command -Name $_ -Module PSSlideShow -ErrorAction SilentlyContinue
        $command | Should-NotBeNull
    }
}
