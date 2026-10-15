---
title: PSSlideShow
description: Interactive terminal slide shows authored in PowerShell.
---

## Get started

You need PowerShell 7.6+ or Windows PowerShell 5.1 and a terminal with ANSI/VT support. To build
from source, install the .NET 10 SDK, Microsoft.PowerShell.PlatyPS v1, and Pester v6:

```powershell
pwsh -NoProfile -File .\build.ps1
.\example.ps1
```

The example imports the built module and starts a tour of its widgets. For your own deck:

```powershell
Import-Module .\module\PSSlideShow.psd1

New-Slide 'Hello' {
    New-SlideText 'A [bold]terminal[/] slide show.'
} | Start-SlideShow
```

Or save several `New-Slide` calls to `deck.ps1` and run `Start-SlideShow -Path .\deck.ps1`.
Decks execute PowerShell with your permissions, so only run scripts you trust.

Read the [cmdlet reference]({{< ref "cmdlets/_index.md" >}}) for parameter details, or explore
[widgets]({{< ref "widgets/_index.md" >}}) for presentation ideas.

## Inspiration

PSSlideShow was inspired by [Jordan Borean](https://github.com/jborean93)'s
[Method to the Madness - How PowerShell Invokes .NET Methods](https://www.youtube.com/watch?v=hKRbQUDYqeI)
talk at PSConfEU 2026. He presented from the terminal using a set of commands he'd put together.
After the presentation, I asked him how he'd done it; I've formalised and (hopefully) polished the
approach into a reusable module.
