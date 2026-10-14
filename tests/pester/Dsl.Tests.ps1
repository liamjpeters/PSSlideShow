#requires -Version 5.1
# Pester v6 tests for the authoring DSL (cmdlet binding + widget collection).

BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $manifestPath = Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path
    Import-Module $manifestPath.Path -Force
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

Describe 'New-Slide' {
    It 'collects emitted widgets in order' {
        $slide = New-Slide -Title 'Demo' -Content {
            New-SlideText 'first'
            New-SlideList 'a', 'b'
        }

        $slide.Title | Should-Be 'Demo'
        $slide.Widgets.Count | Should-Be 2
        $slide.Widgets[0].GetType().Name | Should-Be 'TextWidget'
        $slide.Widgets[1].GetType().Name | Should-Be 'ListWidget'
    }

    It 'defaults CodeScope to Slide' {
        $slide = New-Slide -Content { New-SlideText 'x' }
        $slide.CodeScope.ToString() | Should-Be 'Slide'
    }

    It 'ignores non-widget output' {
        $slide = New-Slide -Content {
            'a stray string'
            New-SlideText 'kept'
        }
        $slide.Widgets.Count | Should-Be 1
    }
}

Describe 'New-SlideText' {
    It 'binds -Justify' {
        $widget = New-SlideText -Text 'hi' -Justify Center
        $widget.Justify.ToString() | Should-Be 'Center'
        $widget.IsRunnable | Should-BeFalse
    }
}

Describe 'New-SlideList' {
    It 'binds -Numbered' {
        $widget = New-SlideList -Items 'one', 'two' -Numbered
        $widget.Numbered | Should-BeTrue
        $widget.Items.Count | Should-Be 2
    }
}

Describe 'New-SlideTable' {
    It 'infers rows from hashtables' {
        $widget = New-SlideTable -Rows @(
            @{ Name = 'Alice'; Age = 30 }
            @{ Name = 'Bob'; Age = 25 }
        )
        $widget.Rows.Count | Should-Be 2
    }

    It 'accepts pipeline input' {
        $widget = @(
            [pscustomobject]@{ Name = 'Alice' }
            [pscustomobject]@{ Name = 'Bob' }
        ) | New-SlideTable
        $widget.Rows.Count | Should-Be 2
    }
}

Describe 'New-SlideDynamic' {
    It 'binds the interval and callback script blocks' {
        $widget = New-SlideDynamic -Interval ([TimeSpan]::FromSeconds(1)) -Update { 'snapshot' } -View {
            param($snapshot)
            New-SlideText $snapshot
        }

        $widget.GetType().Name | Should-Be 'DynamicWidget'
        $widget.UpdateInterval | Should-Be ([TimeSpan]::FromSeconds(1))
        $widget.UpdateScript | Should-NotBeNull
        $widget.ViewScript | Should-NotBeNull
    }

    It 'rejects a non-positive interval' {
        {
            New-SlideDynamic -Interval ([TimeSpan]::Zero) -Update { 'snapshot' } -View { New-SlideText 'view' }
        } | Should-Throw
    }
}
