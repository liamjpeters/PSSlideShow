---
title: Table
description: Compare structured data without hand-aligning columns.
cmdlets: [New-SlideTable]
---

Pass objects or hashtables as rows. The widget can infer columns, or you can set their order
explicitly for a more predictable presentation.

```powershell
New-Slide 'Environment' {
    New-SlideTable -Columns Name, Value -Rows @(
        [pscustomobject]@{ Name = 'Shell'; Value = 'PowerShell' }
        [pscustomobject]@{ Name = 'Target'; Value = 'Terminal' }
    )
}
```

Keep the number of columns small; a table that needs horizontal scrolling is difficult to read on
a projector.
