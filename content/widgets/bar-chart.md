---
title: Bar chart
description: Compare several values on a shared scale.
cmdlets: [New-SlideBarChart]
---

Bar charts make differences between discrete values easy to see. Each item supplies a label and
a numeric value.

```powershell
New-Slide 'Requests' {
    New-SlideBarChart -Item @(
        @{ Label = 'API'; Value = 42; Color = 'blue' }
        @{ Label = 'Web'; Value = 28; Color = 'green' }
    )
}
```

Set `-MaxValue` when several slides should use the same scale, so the visual comparison stays
honest.
