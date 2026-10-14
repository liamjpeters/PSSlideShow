---
title: List
description: Bulleted or numbered points that the audience can scan quickly.
cmdlets: [New-SlideList]
---

Lists work well for an agenda, a short sequence, or a summary. Use numbering when the order
matters.

```powershell
New-Slide 'Plan' {
    New-SlideList 'Introduce the problem', 'Show the solution', 'Run the demo' -Numbered
}
```

Each item can contain Spectre markup. If an item needs several lines of explanation, give it a
separate slide instead.
