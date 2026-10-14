---
title: Rows
description: Divide part of a slide into stacked regions.
cmdlets: [New-SlideRows, New-SlideRow]
---

Slides already stack ordinary widgets. Rows become useful when nested inside columns or when a
dashboard needs bounded regions.

```powershell
New-Slide 'Dashboard' {
    New-SlideRows -Fill {
        New-SlideRow -Ratio 1 { New-SlideText 'Summary' }
        New-SlideRow -Ratio 2 { New-SlideText 'Details' }
    }
}
```

`-Fill` uses the available slide height and clips overflowing content. Without it, rows flow with
their content and the slide can scroll.
