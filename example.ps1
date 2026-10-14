#requires -Version 5.1
<#
    example.ps1  —  The complete PSSlideShow feature tour.

    Every widget and every notable option gets its own slide, and each slide's
    title (plus a line of intro text) tells you what to look for below it.

    Run it from your own terminal (Windows PowerShell 5.1 or PowerShell 7+):

        pwsh       -NoProfile -File .\example.ps1   # PowerShell 7+
        powershell -NoProfile -File .\example.ps1   # Windows PowerShell 5.1

    Navigation once it launches:
        [N] / [->] / [Space] / [PgDn]   Next slide
        [P] / [<-] / [PgUp]             Previous slide
        [Home] / [End]                  First / last slide
        [Up] / [Down]                   Scroll a tall slide
        [Enter]                         Run the code on a code slide
        [R]                             Reset a slide's runspace
        [Q] / [Esc]                     Quit

    Tip: this deck is itself a normal .ps1 that emits Slide objects, so you can
    also launch it with:  Start-SlideShow -Path .\example.ps1
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Import the freshly built module from this repo (run build.ps1 first).
$manifest = Join-Path (Join-Path $PSScriptRoot 'module') 'PSSlideShow.psd1'
if (-not (Test-Path $manifest)) {
    throw "Module not found at '$manifest'. Build it first:  pwsh -NoProfile -File .\build.ps1"
}
Import-Module $manifest -Force

# A tiny throwaway file for the '-RunFromLine / -RunToLine' demo below. New-SlideCode reads it while the
# deck is being built, so it only has to exist now — not while presenting.
$runRangeDemo = Join-Path ([System.IO.Path]::GetTempPath()) 'psslideshow-runrange-demo.ps1'
@'
$greeting = 'Hello'        # line 1 - runs, but hidden on the slide
$subject  = 'wider range'  # line 2 - runs, but hidden on the slide
"$greeting from a $subject!"  # line 3 - the only line shown
'@ | Set-Content -LiteralPath $runRangeDemo

$slides = @(

    #region Text & banners --------------------------------------------------

    # 1 — Title slide: FIGlet banner + centered markup text.
    New-Slide 'Welcome — New-SlideFiglet & New-SlideText' {
        New-SlideFiglet 'PSSlideShow' -Foreground 'green'
        New-SlideText 'A [bold]terminal[/] slide-show engine built on [blue]Spectre.Console[/].' -Justify Center
        New-SlideText 'This deck demonstrates [italic]every[/] widget. Press [green]N[/] to begin, [green]Q[/] to quit.' -Justify Center
    }

    # 2 — New-SlideText: markup, justification and color.
    New-Slide 'New-SlideText — markup, justify & color' {
        New-SlideText 'Text supports Spectre markup and three justifications.'
        New-SlideText 'Left-justified (the default).' -Justify Left
        New-SlideText 'Center-justified.' -Justify Center
        New-SlideText 'Right-justified.' -Justify Right
        New-SlideText 'You can [bold]bold[/], [italic]italicise[/], [underline]underline[/] and [strikethrough]strike[/] inline.'
        New-SlideText 'Or color the whole line with -Foreground.' -Foreground 'deepskyblue1'
        New-SlideText 'Markup colors work too: [red]red[/], [green]green[/], [yellow]yellow[/], [magenta1]magenta[/].'
    }

    # 3 — New-SlideFiglet: banner justification.
    New-Slide 'New-SlideFiglet — big banner text' {
        New-SlideText 'FIGlet renders large ASCII-art headings. It centers by default; -Justify changes that. Pass -FontPath to use a custom .flf font.'
        New-SlideFiglet 'Left' -Justify Left -Foreground 'orange1'
        New-SlideFiglet 'Right' -Justify Right -Foreground 'aqua'
    }

    #endregion

    #region Lists & tables --------------------------------------------------

    # 4 — New-SlideList: bulleted, numbered and custom markers.
    New-Slide 'New-SlideList — bulleted, numbered & custom markers' {
        New-SlideText 'Three lists below: a default bulleted list, a numbered list, and one with a custom marker glyph.'
        New-SlideList 'Widgets are just objects', 'Slides collect widgets', 'The host renders them'
        New-SlideList 'Author', 'Build', 'Present' -Numbered
        New-SlideList 'Custom', 'Marker', 'Glyph' -Marker '→'
    }

    # 5 — New-SlideTable: inferred columns + border/title.
    New-Slide 'New-SlideTable — rows from hashtables' {
        New-SlideText 'Columns are inferred from the first row''s keys. Border style and a caption are optional.'
        New-SlideTable @(
            @{ Widget = 'Text'; Runnable = 'No'; Since = 'v0.1' }
            @{ Widget = 'Code'; Runnable = 'Yes'; Since = 'v0.1' }
            @{ Widget = 'Chart'; Runnable = 'No'; Since = 'v0.1' }
        ) -Title 'Widget catalogue' -Border Rounded
    }

    # 6 — New-SlideTable: explicit column order and a heavy border.
    New-Slide 'New-SlideTable — explicit columns & border style' {
        New-SlideText 'Pass -Columns to fix the header order (and subset the fields); -Border picks the box style.'
        New-SlideTable @(
            @{ Name = 'xUnit'; Kind = '.NET'; Tests = 61 }
            @{ Name = 'Pester'; Kind = 'PowerShell'; Tests = 43 }
        ) -Columns 'Name', 'Tests', 'Kind' -Border Heavy -Title 'Test suites'
    }

    #endregion

    #region Runnable code ---------------------------------------------------

    # 7 — New-SlideCode: runnable PowerShell (press Enter).
    New-Slide 'New-SlideCode — press [Enter] to run it live' {
        New-SlideText 'PowerShell blocks are runnable by default. Press [green]Enter[/] to execute; output appears below. Press [green]R[/] to reset.'
        New-SlideCode {
            Get-Process |
            Sort-Object -Property WorkingSet64 -Descending |
            Select-Object -First 5 Name, Id, @{ N = 'MB'; E = { [int]($_.WorkingSet64 / 1MB) } } |
            Format-Table -AutoSize
        } -Container Panel -HideLineNumbers
    }

    # 8 — New-SlideCode: container styles.
    New-Slide 'New-SlideCode — container styles' {
        New-SlideText 'The -Container option controls the frame: Simple (rules only), Rounded and Panel. Line numbers show by default.'
        New-SlideCode { 'Simple container (default)' } -Container Simple -NonRunnable
        New-SlideCode { 'Rounded container' } -Container Rounded -NonRunnable
        New-SlideCode { 'Panel container' } -Container Panel -NonRunnable
    }

    # 9 — New-SlideCode: non-PowerShell display code via -Code/-Language.
    New-Slide 'New-SlideCode — display any language' {
        New-SlideText 'Use -Code with -Language for display-only snippets in other languages (no run prompt).'
        New-SlideCode -Language 'csharp' -Code @'
public override IRenderable Render(WidgetRenderContext context)
{
    return new Markup($"[italic]{Text}[/]");
}
'@
    }

    # 10 — New-SlideCode: from a file with a line range.
    New-Slide 'New-SlideCode — from a file (-FilePath)' {
        New-SlideText 'Read code straight from disk. -StartLine/-EndLine slice it and the gutter numbers from -StartLine. Here we show this deck''s own import block.'
        New-SlideCode -FilePath $PSCommandPath -StartLine 30 -EndLine 36 -NonRunnable
    }

    # 11 — New-SlideCode: invisible -Setup and -Cleanup around the visible block.
    New-Slide 'New-SlideCode — invisible -Setup and -Cleanup' {
        New-SlideText '-Setup runs before the visible block and -Cleanup after it (in a finally), without being shown. Press [green]Enter[/]: the block uses $demo prepared by -Setup, and -Cleanup tidies it away afterwards.'
        New-SlideCode -Setup { $demo = 'prepared by -Setup' } -Cleanup { Remove-Variable demo -ErrorAction SilentlyContinue } {
            "The block can use `$demo = '$demo'"
        }
    }

    # 12 — New-SlideCode: run a wider range than you display.
    New-Slide 'New-SlideCode — run more than you show (-RunFromLine/-RunToLine)' {
        New-SlideText 'Show a focused slice with -StartLine/-EndLine, but execute a wider slice with -RunFromLine/-RunToLine. Only line 3 is displayed; pressing [green]Enter[/] runs lines 1-3, so the hidden setup lines still run.'
        New-SlideCode -FilePath $runRangeDemo -StartLine 3 -EndLine 3 -RunFromLine 1 -RunToLine 3
    }

    # 13 — New-Slide -CodeScope: two blocks sharing per-slide state.
    New-Slide 'New-Slide -CodeScope — blocks share a runspace' {
        New-SlideText 'With the default CodeScope of [green]Slide[/], every runnable block on the slide shares one runspace. Run the first, then the second: it sees $counter. (CodeScope also accepts Show and Block.)'
        New-SlideCode { $counter = 41; "Set `$counter to $counter" }
        New-SlideCode { $counter++; "Now `$counter is $counter" }
    } -CodeScope Slide

    #endregion

    #region Trees -----------------------------------------------------------

    # 14 — New-SlideTree: nested nodes with guide lines.
    New-Slide 'New-SlideTree — nested nodes' {
        New-SlideText 'Trees nest via New-SlideTreeNode. Guide-line glyphs are configurable; a node with -Collapsed hides its children.'
        New-SlideTree 'src/' {
            New-SlideTreeNode 'Model' {
                New-SlideTreeNode 'Slide.cs'
                New-SlideTreeNode 'Widgets' {
                    New-SlideTreeNode 'TextWidget.cs'
                    New-SlideTreeNode 'TreeWidget.cs'
                }
            }
            New-SlideTreeNode 'Rendering' {
                New-SlideTreeNode 'Renderer.cs'
                New-SlideTreeNode 'SlideTheme.cs'
            }
            New-SlideTreeNode 'Cmdlets (collapsed)' -Collapsed {
                New-SlideTreeNode 'you cannot see me'
            }
        } -Guide DoubleLine -GuideColor 'grey50'
    }

    #endregion

    #region Charts ----------------------------------------------------------

    # 15 — New-SlideBarChart: labelled, colored bars.
    New-Slide 'New-SlideBarChart — labelled values' {
        New-SlideText 'A horizontal bar chart from Label/Value/Color items (hashtables or the pipeline).'
        New-SlideBarChart @(
            @{ Label = 'xUnit'; Value = 61; Color = 'green' }
            @{ Label = 'Pester'; Value = 43; Color = 'blue' }
            @{ Label = 'Total'; Value = 104; Color = 'yellow' }
        ) -Label 'Passing tests'
    }

    # 16 — New-SlideBarChart: value formatter + fixed max.
    New-Slide 'New-SlideBarChart — formatter & fixed axis' {
        New-SlideText 'Fix the axis maximum with -MaxValue and format each value with -ValueFormatter (the value is $_). Here values render as percentages against a max of 100.'
        New-SlideBarChart @(
            @{ Label = 'Coverage'; Value = 82; Color = 'green' }
            @{ Label = 'Docs'; Value = 60; Color = 'aqua' }
            @{ Label = 'Polish'; Value = 45; Color = 'orange1' }
        ) -Label 'Readiness' -MaxValue 100 -ValueFormatter { "$_%" }
    }

    # 17 — New-SlideBreakdownChart: proportions with a percentage legend.
    New-Slide 'New-SlideBreakdownChart — proportions' {
        New-SlideText 'A single proportional bar with a tag legend. -ShowPercentage labels each slice; -FullSize uses the roomier layout.'
        New-SlideBreakdownChart @(
            @{ Label = 'C#'; Value = 65; Color = 'aqua' }
            @{ Label = 'PowerShell'; Value = 30; Color = 'purple' }
            @{ Label = 'Markdown'; Value = 5; Color = 'grey' }
        ) -ShowPercentage -FullSize
    }

    #endregion

    #region Dynamic updates -------------------------------------------------

    # 18 — New-SlideDynamic: background CPU and memory updates.
    New-Slide 'New-SlideDynamic — CPU and memory usage' {
        New-SlideText 'This view refreshes every 10 seconds. The update runs in the background; the view renders the latest snapshot.'

        $dynamicOptions = @{
            Interval = [TimeSpan]::FromSeconds(10)
            Update = {
                $cpu = [double](Get-Counter '\Processor(_Total)\% Processor Time').CounterSamples.CookedValue
                $os = Get-CimInstance Win32_OperatingSystem
                $totalMemory = [double]$os.TotalVisibleMemorySize
                $freeMemory = [double]$os.FreePhysicalMemory
                $memory = if ($totalMemory -gt 0) {
                    (($totalMemory - $freeMemory) / $totalMemory) * 100
                }
                else {
                    0
                }

                [pscustomobject]@{
                    CpuPercent = [math]::Round($cpu, 1)
                    MemoryPercent = [math]::Round($memory, 1)
                    Updated = Get-Date
                }
            }
            View = {
                param($snapshot)

                New-SlideText "Last updated: [green]$($snapshot.Updated.ToString('HH:mm:ss'))[/]"
                New-SlideBarChart @(
                    @{ Label = 'CPU'; Value = $snapshot.CpuPercent; Color = 'green' }
                    @{ Label = 'Memory'; Value = $snapshot.MemoryPercent; Color = 'aqua' }
                ) -Label 'Resource usage' -MaxValue 100 -ValueFormatter { '{0:N1}%' -f $_ }
            }
        }

        New-SlideDynamic @dynamicOptions
    }

    #endregion

    #region Calendar & canvas ----------------------------------------------

    # 19 — New-SlideCalendar: highlighted event days.
    New-Slide 'New-SlideCalendar — highlighted days' {
        New-SlideText 'A month view. Highlight days with -Event (day numbers, DateTimes, or @{ Date = ... }); -HighlightColor and -Border style it. -Culture localises the names.'
        New-SlideCalendar -Year 2026 -Month 8 -Event 2, 15, 28 -HighlightColor 'green' -Border Rounded
    }

    # 20 — New-SlideCanvas: pixel art with Fill + SetPixel.
    New-Slide 'New-SlideCanvas — pixel art' {
        New-SlideText 'The -Paint block draws on a pixel grid: $_ is the canvas, $Width/$Height its size. Use $_.Fill(color) and $_.SetPixel(x, y, color) (or r, g, b bytes).'
        New-SlideCanvas 24 24 {
            $_.Fill('grey15')
            for ($i = 0; $i -lt $Width; $i++) {
                $_.SetPixel($i, $i, 'green')                        # main diagonal
                $_.SetPixel(($Width - 1 - $i), $i, 'deepskyblue1')  # anti-diagonal
            }
            # A red RGB border along the top and bottom edges.
            for ($x = 0; $x -lt $Width; $x++) {
                $_.SetPixel($x, 0, 220, 40, 40)
                $_.SetPixel($x, ($Height - 1), 220, 40, 40)
            }
        }
    }

    #endregion

    #region Callouts --------------------------------------------------------

    # 21 — New-SlideAlert: GitHub-style coloured callouts.
    New-Slide 'New-SlideAlert — coloured callouts' {
        New-SlideText 'Highlight an aside with a coloured left bar, a symbol and a title. -Severity picks the colour: Note, Tip, Important, Warning or Caution. The body supports markup, and -Title overrides the default heading.'
        New-SlideAlert -Severity Note -Text 'Useful information the audience should know.'
        New-SlideAlert -Severity Tip -Text 'Give useful tips' -Title 'Tip'
        New-SlideAlert -Severity Important -Text 'Calls out critical information.' -Title 'Important'
        New-SlideAlert -Severity Warning -Text 'Urgent info that needs [bold]immediate[/] attention.'
        New-SlideAlert -Severity Caution -Text 'Advises about risks or negative outcomes.' -Title 'Danger zone'
    }

    #endregion

    #region Layout ----------------------------------------------------------

    # 22 — New-SlideColumns: flow columns side by side.
    New-Slide 'New-SlideColumns — side by side' {
        New-SlideText 'Compose widgets into columns. In flow mode each column sizes to its content.'
        New-SlideColumns {
            New-SlideColumn {
                New-SlideText '[bold]Left column[/]'
                New-SlideList 'Compose widgets', 'Into columns', 'Nest freely'
            }
            New-SlideColumn {
                New-SlideText '[bold]Right column[/]'
                New-SlideBarChart @(
                    @{ Label = 'A'; Value = 3 }
                    @{ Label = 'B'; Value = 7 }
                ) -HideValues
            }
        }
    }

    # 23 — New-SlideRows: stacked regions.
    New-Slide 'New-SlideRows — stacked regions' {
        New-SlideText 'New-SlideRows stacks its New-SlideRow regions vertically — handy for grouping.'
        New-SlideRows {
            New-SlideRow { New-SlideText '[bold]Header row[/] — a title area' }
            New-SlideRow { New-SlideList 'Body item one', 'Body item two' }
            New-SlideRow { New-SlideText '[dim]Footer row — a caption[/]' }
        }
    }

    # 24 — Fill layout: a proportional dashboard (-Fill + -Ratio).
    New-Slide 'Layout -Fill — a proportional dashboard' {
        New-SlideText 'Add -Fill to divide the whole content area into regions. -Ratio/-Size/-MinSize size them; a Fill slide does not scroll (overflow is clipped).'
        New-SlideColumns -Fill {
            New-SlideColumn -Ratio 2 {
                New-SlideText '[bold]Wide panel[/] (Ratio 2)'
                New-SlideBarChart @(
                    @{ Label = 'North'; Value = 12; Color = 'green' }
                    @{ Label = 'South'; Value = 9; Color = 'aqua' }
                    @{ Label = 'East'; Value = 7; Color = 'yellow' }
                ) -Label 'Sales'
            }
            New-SlideColumn -Ratio 1 {
                New-SlideText '[bold]Narrow panel[/] (Ratio 1)'
                New-SlideList 'Live', 'Metrics', 'Here'
            }
        }
    }

    #endregion

    #region Presentation options -------------------------------------------

    # 25 — Padding: HPadding/VPadding on any widget.
    New-Slide 'Widget padding — -HPadding & -VPadding' {
        New-SlideText 'Every widget cmdlet accepts -HPadding and -VPadding to inset it. The box below is pushed in horizontally and spaced out vertically.'
        New-SlideTable @(
            @{ Setting = 'HPadding'; Effect = 'Left/right inset' }
            @{ Setting = 'VPadding'; Effect = 'Blank lines above/below' }
        ) -Border Square -HPadding 12 -VPadding 2
    }

    # 26 — Closing slide.
    New-Slide 'That''s the tour' {
        New-SlideFiglet 'Thanks!' -Foreground 'green'
        New-SlideText 'You just saw every PSSlideShow widget. See [blue]docs/usage.md[/] for the reference, and [blue]docs/adding-a-widget.md[/] to build your own.' -Justify Center
        New-SlideText 'Press [green]Home[/] to jump back to the start, or [green]Q[/] to quit.' -Justify Center
    }

    #endregion
)

# Start-SlideShow options you can try:
#   -StartAt 7           open on a specific slide (1-based)
#   -ContentPadding 0    run content flush to the terminal edges (default 2)
#   -NoAlternateScreen   render inline instead of using the alternate screen buffer
Start-SlideShow -Slides $slides
