# Authoring and presenting

For requirements, building, and the bundled tour, start with the [README](../README.md).
The [command reference](PSSlideShow/PSSlideShow.md) has the full syntax and per-cmdlet examples.

## Authoring a deck

A deck emits slides. Each `New-Slide` runs its content script block and collects the widgets it emits,
in order. Present the slides from the pipeline or a file:

```powershell
New-Slide 'Agenda' {
    New-SlideList 'Introduction', 'Live demo'
} | Start-SlideShow
```

Text widgets accept [Spectre markup](https://spectreconsole.net/markup) such as `[bold]`, `[green]`,
and `[link]`.

### Loading a deck from a file

`Start-SlideShow -Path` runs a `.ps1` deck file and collects the slides it emits:

```powershell
Start-SlideShow -Path .\deck.ps1
```

The deck runs as a script file, so `$PSScriptRoot` and `$PSCommandPath` refer to the deck. Use
`$PSScriptRoot` to find resources stored alongside it.

## Navigation

Once the show launches:

| Keys | Action |
| --- | --- |
| `N` / `→` / `Space` / `PgDn` | Next slide |
| `P` / `←` / `PgUp` | Previous slide |
| `Home` / `End` | First / last slide |
| `↑` / `↓` | Scroll a slide taller than the window |
| `Enter` | Run the next code block on a code slide |
| `R` | Reset a slide's runspace and output |
| `Q` / `Esc` / `Ctrl+C` | Quit |

The footer shows only the controls that apply to the current slide (for example, `[Enter] Run` appears
only when a runnable block is still pending). The single source of truth for these bindings is
[KeyboardShortcuts.cs](../src/PSSlideShow/Input/KeyboardShortcuts.cs); see
[adding-a-keyboard-shortcut.md](adding-a-keyboard-shortcut.md) to add your own.

For presentation options such as `-StartAt`, `-NoAlternateScreen`, `-ContentPadding`, and
`-SyntaxTheme`, see [Start-SlideShow](PSSlideShow/Start-SlideShow.md).

## Widgets

All widget cmdlets share `-HPadding` (left/right, default `0`) and `-VPadding` (top/bottom, default `1`)
for per-widget spacing. Nested cmdlets (`New-SlideColumn`, `New-SlideRow`, `New-SlideTreeNode`) do not
take padding — they are composed inside their parent.

- **Text and callouts:** `New-SlideText`, `New-SlideFiglet`, `New-SlideList`, `New-SlideAlert`.
- **Structured data:** `New-SlideTable`, `New-SlideTree`, charts, calendars, and canvases.
- **Layout:** `New-SlideColumns` and `New-SlideRows` with nested regions.
- **Code and live data:** `New-SlideCode` and `New-SlideDynamic`.

See the [command reference](PSSlideShow/PSSlideShow.md) or `Get-Help New-SlideText -Full` for
individual parameters and examples.

## Runnable code slides

A `New-SlideCode` block written in PowerShell is runnable by default. Press `Enter` to execute the next
pending block on the slide; its captured output appears below the code. Multiple runnable blocks on one
slide run in sequence. `-CodeScope` on `New-Slide` controls runspace sharing:

- `Slide` (default) — one runspace per slide; blocks on the same slide share state.
- `Show` — a single runspace shared across the whole deck.
- `Block` — every block runs in its own isolated runspace.

Press `R` to reset a slide: its runspace is discarded and its captured output cleared.

Decks and runnable blocks execute PowerShell with your permissions. Use trusted decks; runspace
scoping keeps demo state separate but does not sandbox code.

### Hidden setup and wider run ranges

Sometimes a demo needs code that runs but should not clutter the slide:

- In the `-ScriptBlock` form, `-Setup` runs before the visible block and `-Cleanup` runs after it (in a
  `finally`, so it runs even if the block throws). Neither is displayed.
- In the `-FilePath` form, `-StartLine`/`-EndLine` choose what is *shown* while `-RunFromLine`/`-RunToLine`
  choose what is *executed* (usually a wider slice). The shown lines must fall within the executed lines,
  otherwise the cmdlet errors. All four line numbers are optional — an absent start implies the first
  line and an absent end implies the last.

## Layout: columns and rows

`New-SlideColumns` / `New-SlideRows` arrange regions side-by-side or stacked. By default columns split
the width evenly; give a region `-Size` (fixed cells) or `-Ratio` (proportional share) to weight it.
`-Fill` switches from a content-sized flow layout to a fixed-size layout that clips to the viewport.

```powershell
New-Slide 'Two columns' {
    New-SlideColumns {
        New-SlideColumn { New-SlideText 'Left half.' }
        New-SlideColumn { New-SlideText 'Right half.' }
    }
}
```

## Periodically updated content

[`New-SlideDynamic`](PSSlideShow/New-SlideDynamic.md) refreshes data in a dedicated background
runspace, then runs its view script block on the presentation thread to produce widgets. Neither
script block runs on every repaint; keep the update block self-contained because it does not share
the presentation runspace.

## A note on script encoding (Windows PowerShell 5.1)

Windows PowerShell 5.1 parses a BOM-less `.ps1` file using the system ANSI code page, which mangles
non-ASCII characters (em dashes, bullets, box-drawing) in string literals. Save decks that contain such
characters as **UTF-8 with BOM** so they render correctly on both editions. PowerShell 7 defaults to
UTF-8 and is unaffected.
