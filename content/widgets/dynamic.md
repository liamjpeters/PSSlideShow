---
title: Dynamic
description: Refresh a widget's data while the slide is active.
cmdlets: [New-SlideDynamic]
---

The `-Update` block runs in a dedicated background runspace. When it produces a snapshot, the
`-View` block turns that snapshot into ordinary slide widgets on the presentation thread.

```powershell
New-Slide 'Clock' {
    New-SlideDynamic -Interval ([TimeSpan]::FromSeconds(2)) -Update {
        Get-Date
    } -View {
        param($snapshot)
        New-SlideText "Updated at $($snapshot.ToString('HH:mm:ss'))"
    }
}
```

Keep updates self-contained: their runspace does not inherit variables from the deck. Neither
block runs during every repaint.
