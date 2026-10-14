---
title: Columns
description: Show related widgets side by side.
cmdlets: [New-SlideColumns, New-SlideColumn]
---

Columns suit comparisons such as before/after or two alternative approaches. Each column has its
own body and can hold several widgets.

```powershell
New-Slide 'Before and after' {
    New-SlideColumns {
        New-SlideColumn { New-SlideText '[bold]Before[/]'; New-SlideText 'Manual steps' }
        New-SlideColumn { New-SlideText '[bold]After[/]'; New-SlideText 'One command' }
    }
}
```

Use `-Ratio` or `-Size` on a column when equal widths do not fit the content. Avoid too many
columns on small terminals.
