---
title: Alert
description: Give a note, tip, or warning a clear visual priority.
cmdlets: [New-SlideAlert]
---

Alerts combine a coloured bar, a severity symbol, a title, and supporting text. They work well
for a warning before a live demo or a short take-away.

```powershell
New-Slide 'Remember' {
    New-SlideAlert -Severity Tip -Text 'Reset the demo before presenting it again.'
}
```

Reserve strong severities such as `Caution` for information that genuinely needs attention.
