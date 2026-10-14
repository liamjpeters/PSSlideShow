---
title: Figlet
description: Large terminal lettering for title slides and section breaks.
cmdlets: [New-SlideFiglet]
---

Figlet turns short text into an ASCII-art banner. It works best for a title or a single keyword;
long phrases may wrap or clip in narrow terminals.

```powershell
New-Slide 'Welcome' {
    New-SlideFiglet 'PSSlideShow' -Foreground green
    New-SlideText 'A presentation in your terminal.' -Justify Center
}
```

Use `-FontPath` when a deck needs a specific FIGlet font. Check the result at your presentation's
actual terminal width.
