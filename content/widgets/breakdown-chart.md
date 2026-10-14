---
title: Breakdown chart
description: Show how categories contribute to one total.
cmdlets: [New-SlideBreakdownChart]
---

Unlike a bar chart, the breakdown chart uses one proportional bar. It suits a composition such as
time spent across tasks or the split of a budget.

```powershell
New-Slide 'Time spent' {
    New-SlideBreakdownChart -Item @(
        @{ Label = 'Build'; Value = 70; Color = 'blue' }
        @{ Label = 'Review'; Value = 30; Color = 'cyan' }
    ) -ShowPercentage
}
```

Use a small number of categories so the legend remains legible.
