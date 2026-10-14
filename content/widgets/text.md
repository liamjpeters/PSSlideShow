---
title: Text
description: Markup-aware paragraphs for explanations, headings, and callouts.
cmdlets: [New-SlideText]
---

Use text for the main narrative of a slide. Spectre markup lets you emphasise a phrase without
changing the rest of the terminal's colour palette.

```powershell
New-Slide 'The point' {
    New-SlideText 'Show the [bold]result[/], not every step.' -Justify Center
}
```

Keep lines short enough for the audience to read at a glance. For a larger heading, pair text with
the Figlet widget rather than filling the slide with paragraphs.
