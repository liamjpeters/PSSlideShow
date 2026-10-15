# PSSlideShow

Author and present **interactive terminal slide shows** in PowerShell. A deck is a `.ps1` script that
emits slides with `New-Slide*` cmdlets; `Start-SlideShow` presents them with
[Spectre.Console](https://spectreconsole.net/) in the terminal.

```powershell
# deck.ps1
New-Slide 'Welcome' {
    New-SlideFiglet 'PSSlideShow' -Foreground green
    New-SlideText   'A [bold]terminal[/] slide show, defined in PowerShell.' -Justify Center
}

New-Slide 'Live demo' {
    New-SlideCode -ScriptBlock {
        Get-Process | Sort-Object CPU -Descending | Select-Object -First 5 Name, CPU
    }
}
```

```powershell
Start-SlideShow -Path .\deck.ps1
```

## Features

- Write decks entirely in PowerShell, with text, layouts, tables, trees, charts, calendars, canvas
  drawings, QR codes, and [Spectre markup](https://spectreconsole.net/markup).
- Show syntax-highlighted code and run PowerShell live on a slide; dynamic widgets can refresh in the
  background.
- Use terminal-palette colors that adapt to light and dark themes. Private dependencies are isolated
  on PowerShell 7 and loaded best-effort from the module on Windows PowerShell 5.1.

## Requirements

- **PowerShell 7.6+** (uses the `net10.0` build) **or Windows PowerShell 5.1** (uses the `net472` build).
- A terminal with ANSI/VT support (Windows Terminal or any modern terminal).
- To build from source: the .NET 10 SDK and Microsoft.PowerShell.PlatyPS v1; the default build also
  runs tests and requires Pester v6.

## Getting started

Build from source and run the bundled tour:

```powershell
pwsh -NoProfile -File .\build.ps1
.\example.ps1
```

For your own deck, import the built module before presenting it:

```powershell
Import-Module .\module\PSSlideShow.psd1
Start-SlideShow -Path .\deck.ps1
```

See the [usage guide](docs/usage.md) for authoring, navigation, layouts, and live code. Decks execute
PowerShell, so only run scripts you trust.

## Inspiration

PSSlideShow was inspired by [Jordan Borean](https://github.com/jborean93)'s
[Method to the Madness - How PowerShell Invokes .NET Methods](https://www.youtube.com/watch?v=hKRbQUDYqeI)
talk at PSConfEU 2026. He presented from the terminal using a set of commands he'd put together.
After the presentation, I asked him how he'd done it; I've formalised and (hopefully) polished the
approach into a reusable module.

## Documentation

- [Usage guide](docs/usage.md) — decks, controls, code execution, and layout.
- [Command reference](docs/PSSlideShow/PSSlideShow.md) — parameters and examples; also available via
  `Get-Help New-SlideCode -Full` after building.
- [Architecture](docs/architecture.md) — how a keypress becomes a frame.
- [Extension guides](docs/adding-a-widget.md) — add a widget; [keyboard shortcuts](docs/adding-a-keyboard-shortcut.md)
  — add a key binding.

## Development

The default [build](build.ps1) stages both frameworks, the license, and `Get-Help`, then runs xUnit
and Pester:

```powershell
pwsh -NoProfile -File .\build.ps1 -Configuration Debug
```

Use `-SkipTests` to stage without running tests, or `-Docs` to additionally validate the authored
Markdown help. [CI](.github/workflows/ci.yml) is configured for PowerShell 7 on Windows, Ubuntu, and
macOS; on Windows the build also runs Pester in Windows PowerShell 5.1.
Dependabot checks NuGet and GitHub Actions versions every Wednesday at 09:00 UK time
([configuration](.github/dependabot.yml)).

For new cmdlets and help updates, follow the [widget guide](docs/adding-a-widget.md#5-build-and-document-it).
On Windows, close any session that imported the module before rebuilding: it may lock the native
`libonigwrap.dll`.

## Releases

After CI passes, publish a GitHub release for a tag matching `v<ModuleVersion>` in the
[module manifest](module/PSSlideShow.psd1), for example `v0.1.0`. The
[release workflow](.github/workflows/release.yml) checks that version, builds and stages the module
in Release mode, and attaches a `PSSlideShow-<version>.zip` containing the installable
`PSSlideShow/` folder. It does not publish to PowerShell Gallery or rerun tests.

## License

Licensed under the [MIT License](LICENSE).
