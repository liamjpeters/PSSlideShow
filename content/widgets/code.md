---
title: Code
description: Syntax-highlighted examples that can also run live.
cmdlets: [New-SlideCode]
---

A PowerShell script block is runnable by default. Press Enter while presenting to execute the next
pending block and show its captured output. Code in other languages is display-only.

```powershell
New-Slide 'Live process list' {
    New-SlideCode -ScriptBlock {
        Get-Process | Sort-Object CPU -Descending |
            Select-Object -First 5 Name, CPU
    }
}
```

Use `-NonRunnable` for a PowerShell example you do not intend to execute. Live code runs with your
permissions; only present trusted decks and commands.
