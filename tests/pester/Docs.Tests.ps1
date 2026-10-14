#requires -Version 5.1
# Pester v6 tests that keep the PlatyPS command help in sync with the module surface.
#
# These tests intentionally parse the Markdown directly (no PlatyPS dependency) so they run anywhere
# the module is staged. They fail if a cmdlet or parameter is added/renamed without updating the docs,
# or if any authoring placeholder is left behind.

# The -ForEach data below must be built during Pester's *Discovery* phase (that is when parameterized
# tests are generated), so it lives in BeforeDiscovery rather than BeforeAll (which runs later, during
# the Run phase). The module is imported here only so Get-Command can enumerate the command surface.
BeforeDiscovery {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $manifestPath = (Resolve-Path (Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1')).Path
    Import-Module $manifestPath -Force

    $script:DocsDir = (Resolve-Path (Join-Path (Join-Path $repoRoot 'docs') 'PSSlideShow')).Path

    $commonParameters = @(
        [System.Management.Automation.Cmdlet]::CommonParameters +
        [System.Management.Automation.Cmdlet]::OptionalCommonParameters
    )

    $script:CommandCases = Get-Command -Module PSSlideShow | Sort-Object Name | ForEach-Object {
        @{
            Name       = $_.Name
            Path       = Join-Path $script:DocsDir ($_.Name + '.md')
            Parameters = @($_.Parameters.Keys | Where-Object { $_ -notin $commonParameters } | Sort-Object)
        }
    }

    $script:ParameterCases = foreach ($command in $script:CommandCases) {
        foreach ($parameter in $command.Parameters) {
            @{
                Command   = $command.Name
                Path      = $command.Path
                Parameter = $parameter
            }
        }
    }
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

# Run-phase setup: values referenced *inside* It/BeforeAll bodies must be (re)established here, because
# variables assigned during Discovery (e.g. in BeforeDiscovery) do not flow into the Run phase.
BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $script:DocsDir = (Resolve-Path (Join-Path (Join-Path $repoRoot 'docs') 'PSSlideShow')).Path
    $script:CommandNames = @(Get-Command -Module PSSlideShow | Select-Object -ExpandProperty Name)
}

Describe 'Command help Markdown' {
    It '<Name> has a Markdown help file' -ForEach $script:CommandCases {
        Test-Path -LiteralPath $Path | Should-BeTrue -Because "docs/PSSlideShow/$Name.md must exist for the exported cmdlet $Name"
    }

    It '<Name> has a non-empty synopsis' -ForEach $script:CommandCases {
        $content = Get-Content -LiteralPath $Path -Raw
        if ($content -match '(?ms)^##\s+SYNOPSIS\s*\r?\n\r?\n(.+?)\r?\n\r?\n##\s') {
            $synopsis = $Matches[1].Trim()
        }
        else {
            $synopsis = ''
        }
        $synopsis | Should-BeTruthy -Because "$Name must document a SYNOPSIS"
        $synopsis | Should-NotMatchString -Expected '\{\{' -Because "$Name SYNOPSIS still contains a placeholder"
    }

    It '<Name> has no authoring placeholders' -ForEach $script:CommandCases {
        $content = Get-Content -LiteralPath $Path -Raw
        # PlatyPS leaves {{ ... }} placeholders for content you must fill in.
        $content | Should-NotMatchString -Expected '\{\{' -Because "$Name still has one or more {{ ... }} placeholders to fill in"
    }
}

Describe 'Documented parameters' {
    It '<Command> documents -<Parameter>' -ForEach $script:ParameterCases {
        $content = Get-Content -LiteralPath $Path -Raw
        $pattern = '(?m)^###\s+-' + [regex]::Escape($Parameter) + '\s*$'
        $content | Should-MatchString -Expected $pattern -Because "$Command must have a '### -$Parameter' section"
    }
}

Describe 'Module landing page' {
    BeforeAll {
        $script:ModulePagePath = Join-Path $script:DocsDir 'PSSlideShow.md'
        $script:ModulePage = Get-Content -LiteralPath $script:ModulePagePath -Raw
    }

    It 'exists' {
        Test-Path -LiteralPath $script:ModulePagePath | Should-BeTrue
    }

    It 'has no authoring placeholders' {
        $script:ModulePage | Should-NotMatchString -Expected '\{\{'
    }

    It 'links <Name>' -ForEach $script:CommandCases {
        $link = '\[' + [regex]::Escape($Name) + '\]\(' + [regex]::Escape($Name) + '\.md\)'
        $script:ModulePage | Should-MatchString -Expected $link -Because "the module page must link to $Name.md"
    }
}

Describe 'No orphaned help files' {
    It 'every command Markdown file maps to an exported cmdlet' {
        $expected = $script:CommandNames
        $orphans = @(
            Get-ChildItem -LiteralPath $script:DocsDir -Filter '*.md' |
                Where-Object { $_.BaseName -ne 'PSSlideShow' } |
                Where-Object { $_.BaseName -notin $expected } |
                Select-Object -ExpandProperty Name
        )
        $orphans.Count | Should-Be 0 -Because "these Markdown files have no matching exported cmdlet: $($orphans -join ', ')"
    }
}
