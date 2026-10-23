---
title: Alert
description: Give a note, tip, or warning a clear visual priority.
cmdlets: [New-SlideAlert]
---

Alerts combine a coloured bar, a severity symbol, a title, and supporting text. They work well
for a warning before a live demo or a short take-away.

```powershell
New-Slide -Title 'Alerts' {
    New-SlideAlert -Severity Caution -Text 'Red usually means bad'
    New-SlideAlert -Severity Tip -Title 'Success!' -Text 'Green is usually the desired outcome' -NoSymbol
    New-SlideAlert -Severity Note -Text 'Blue is a universally informational colour/color 🙂'
}
```

{{< theme-image light="img/widgets/alert-light.png" dark="img/widgets/alert-dark.png" alt="Terminal slide showing caution, success, and note alert examples." >}}

Reserve strong severities such as `Caution` for information that genuinely needs attention.
