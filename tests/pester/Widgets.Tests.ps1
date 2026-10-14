#requires -Version 5.1
# Pester v6 tests for the M2 widget cmdlets (code, layout, tree, charts, calendar, canvas).

BeforeAll {
    $repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
    $manifestPath = Join-Path (Join-Path $repoRoot 'module') 'PSSlideShow.psd1' | Resolve-Path
    Import-Module $manifestPath.Path -Force
}

AfterAll {
    Remove-Module PSSlideShow -Force -ErrorAction SilentlyContinue
}

Describe 'New-SlideCode' {
    It 'makes PowerShell script blocks runnable and derives the title' {
        $widget = New-SlideCode { Get-Process }
        $widget.IsRunnable | Should-BeTrue
        $widget.Language | Should-Be 'powershell'
        $widget.Title | Should-Be 'PowerShell'
    }

    It 'honours -NonRunnable' {
        $widget = New-SlideCode -NonRunnable { Get-Process }
        $widget.IsRunnable | Should-BeFalse
    }

    It 'accepts literal code for other languages' {
        $widget = New-SlideCode -Code '{ "a": 1 }' -Language json
        $widget.IsRunnable | Should-BeFalse
        $widget.Title | Should-Be 'JSON'
    }

    It 'dedents indented code' {
        $widget = New-SlideCode -Code "    line1`n    line2" -Language text
        $widget.Code | Should-Be "line1`nline2"
    }

    It 'runs -Setup and -Cleanup around the block without displaying them' {
        $widget = New-SlideCode -Setup { $x = 1 } -Cleanup { Remove-Variable x } { $x }
        $widget.Code.Trim() | Should-Be '$x'
        $script = $widget.Script.ToString()
        $script | Should-MatchString -Expected 'x = 1'
        $script | Should-MatchString -Expected 'Remove-Variable'
        $script | Should-MatchString -Expected 'finally'
    }

    It 'omits the finally wrapper when only -Setup is given' {
        $widget = New-SlideCode -Setup { $x = 1 } { $x }
        $widget.Script.ToString() | Should-MatchString -Expected 'x = 1'
        $widget.Script.ToString() | Should-NotMatchString -Expected 'finally'
    }
}

Describe 'New-SlideCode -FilePath line ranges' {
    BeforeAll {
        $script:codeFile = Join-Path $TestDrive 'code-source.ps1'
        Set-Content -LiteralPath $script:codeFile -Value @(
            "Write-Output 'setup'"      # line 1
            "Write-Output 'show-one'"   # line 2
            "Write-Output 'show-two'"   # line 3
            "Write-Output 'teardown'"   # line 4
        )
    }

    It 'displays the whole file when no line numbers are given' {
        $widget = New-SlideCode -FilePath $script:codeFile -NonRunnable
        $widget.Code | Should-MatchString -Expected 'setup'
        $widget.Code | Should-MatchString -Expected 'teardown'
    }

    It 'treats -StartLine without -EndLine as "to end of file"' {
        $widget = New-SlideCode -FilePath $script:codeFile -StartLine 3 -NonRunnable
        $widget.Code | Should-NotMatchString -Expected 'show-one'
        $widget.Code | Should-MatchString -Expected 'show-two'
        $widget.Code | Should-MatchString -Expected 'teardown'
    }

    It 'treats -EndLine without -StartLine as "from first line"' {
        $widget = New-SlideCode -FilePath $script:codeFile -EndLine 2 -NonRunnable
        $widget.Code | Should-MatchString -Expected 'setup'
        $widget.Code | Should-MatchString -Expected 'show-one'
        $widget.Code | Should-NotMatchString -Expected 'teardown'
    }

    It 'numbers the gutter from -StartLine' {
        $widget = New-SlideCode -FilePath $script:codeFile -StartLine 2 -EndLine 3 -NonRunnable
        $widget.StartLineNumber | Should-Be 2
    }

    It 'executes a wider range than it displays' {
        $widget = New-SlideCode -FilePath $script:codeFile -StartLine 2 -EndLine 3 -RunFromLine 1 -RunToLine 4
        # Displayed: only the two "show" lines.
        $widget.Code | Should-NotMatchString -Expected 'setup'
        $widget.Code | Should-NotMatchString -Expected 'teardown'
        # Executed: the full range, including the invisible setup/teardown lines.
        $widget.Script.ToString() | Should-MatchString -Expected 'setup'
        $widget.Script.ToString() | Should-MatchString -Expected 'teardown'
    }

    It 'errors when the displayed lines fall outside the execution range' {
        {
            New-SlideCode -FilePath $script:codeFile -StartLine 1 -EndLine 4 -RunFromLine 2 -RunToLine 3
        } | Should-Throw -ExceptionType ([System.Management.Automation.PSArgumentException])
    }
}

Describe 'New-SlideTree' {
    It 'collects nodes and nested children' {
        $widget = New-SlideTree -Label 'root' -Body {
            New-SlideTreeNode -Label 'a'
            New-SlideTreeNode -Label 'b' -Body {
                New-SlideTreeNode -Label 'c'
            }
        }
        $widget.Nodes.Count | Should-Be 2
        $widget.Nodes[1].Children.Count | Should-Be 1
        $widget.Nodes[1].Children[0].Label | Should-Be 'c'
    }

    It 'rejects New-SlideTreeNode outside a tree' {
        { New-SlideTreeNode -Label 'orphan' } | Should-Throw
    }
}

Describe 'New-SlideBarChart' {
    It 'parses hashtable items' {
        $widget = New-SlideBarChart -Item @{ Label = 'A'; Value = 10 }, @{ Label = 'B'; Value = 20 }
        $widget.Items.Count | Should-Be 2
        $widget.Items[0].Label | Should-Be 'A'
        $widget.Items[1].Value | Should-Be 20
    }

    It 'accepts pipeline input' {
        $widget = @{ Label = 'A'; Value = 1 }, @{ Label = 'B'; Value = 2 } | New-SlideBarChart
        $widget.Items.Count | Should-Be 2
    }
}

Describe 'New-SlideBreakdownChart' {
    It 'binds display switches' {
        $widget = New-SlideBreakdownChart -Item @{ Label = 'A'; Value = 1 } -ShowPercentage -HideTags
        $widget.ShowPercentage | Should-BeTrue
        $widget.HideTags | Should-BeTrue
    }
}

Describe 'New-SlideCalendar' {
    It 'binds year and month' {
        $widget = New-SlideCalendar -Year 2024 -Month 3
        $widget.Year | Should-Be 2024
        $widget.Month | Should-Be 3
    }

    It 'interprets day-number events against the month' {
        $widget = New-SlideCalendar -Year 2024 -Month 3 -Event 5, 10
        $widget.Events.Count | Should-Be 2
        $widget.Events[0].Date.Day | Should-Be 5
        $widget.Events[0].Date.Month | Should-Be 3
    }
}

Describe 'New-SlideCanvas' {
    It 'exposes dimensions to the paint script and stores them' {
        $widget = New-SlideCanvas -Width 8 -Height 4 -Paint {
            if ($Width -ne 8 -or $Height -ne 4) { throw 'dimensions not available' }
            $_.Fill('blue')
        }
        $widget.Width | Should-Be 8
        $widget.Height | Should-Be 4
    }
}

Describe 'New-SlideAlert' {
    It 'binds severity, text and defaults the title to the severity name' {
        $widget = New-SlideAlert -Severity Warning -Text 'Careful now'
        $widget.Severity.ToString() | Should-Be 'Warning'
        $widget.Text | Should-Be 'Careful now'
        $widget.Heading | Should-Be 'Warning'
        $widget.ShowSymbol | Should-BeTrue
    }

    It 'uses a supplied title as the heading' {
        $widget = New-SlideAlert -Severity Tip -Text 'Do this' -Title 'Pro tip'
        $widget.Heading | Should-Be 'Pro tip'
    }

    It '-NoSymbol hides the severity symbol' {
        $widget = New-SlideAlert -Severity Note -Text 'x' -NoSymbol
        $widget.ShowSymbol | Should-BeFalse
    }

    It 'accepts every severity' {
        foreach ($sev in 'Note', 'Tip', 'Important', 'Warning', 'Caution') {
            $widget = New-SlideAlert -Severity $sev -Text 'body'
            $widget.Severity.ToString() | Should-Be $sev
        }
    }
}

Describe 'New-SlideColumns / New-SlideRows' {    It 'collects regions and their widgets' {
        $widget = New-SlideColumns {
            New-SlideColumn { New-SlideText 'L' }
            New-SlideColumn { New-SlideText 'R' }
        }
        $widget.Orientation.ToString() | Should-Be 'Columns'
        $widget.Regions.Count | Should-Be 2
        $widget.Regions[0].Widgets.Count | Should-Be 1
    }

    It 'binds -Fill and region sizing' {
        $widget = New-SlideRows -Fill {
            New-SlideRow -Size 3 { New-SlideText 'top' }
            New-SlideRow -Ratio 2 { New-SlideText 'bottom' }
        }
        $widget.Fill | Should-BeTrue
        $widget.Regions[0].Size | Should-Be 3
        $widget.Regions[1].Ratio | Should-Be 2
    }
}

Describe 'Widget padding' {
    It 'defaults to no horizontal padding and one row of vertical padding' {
        $widget = New-SlideText 'hello'
        $widget.HorizontalPadding | Should-Be 0
        $widget.VerticalPadding | Should-Be 1
    }

    It 'binds -HPadding and -VPadding on top-level widgets' {
        $widget = New-SlideText 'hello' -HPadding 4 -VPadding 2
        $widget.HorizontalPadding | Should-Be 4
        $widget.VerticalPadding | Should-Be 2
    }

    It 'clamps negative padding to zero' {
        $widget = New-SlideList -Items 'a', 'b' -HPadding -5 -VPadding -1
        $widget.HorizontalPadding | Should-Be 0
        $widget.VerticalPadding | Should-Be 0
    }

    It 'exposes the padding parameters across widget cmdlets' {
        foreach ($name in 'New-SlideText', 'New-SlideList', 'New-SlideTable', 'New-SlideTree',
            'New-SlideCode', 'New-SlideFiglet', 'New-SlideBarChart', 'New-SlideBreakdownChart',
            'New-SlideCalendar', 'New-SlideCanvas', 'New-SlideColumns', 'New-SlideRows') {
            $params = (Get-Command $name).Parameters
            ($params.ContainsKey('HPadding')) | Should-BeTrue -Because "$name should expose -HPadding"
            ($params.ContainsKey('VPadding')) | Should-BeTrue -Because "$name should expose -VPadding"
        }
    }

    It 'does not add padding parameters to nested cmdlets' {
        (Get-Command New-SlideTreeNode).Parameters.ContainsKey('HPadding') | Should-BeFalse
        (Get-Command New-SlideColumn).Parameters.ContainsKey('HPadding') | Should-BeFalse
    }
}
