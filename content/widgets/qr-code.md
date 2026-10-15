---
title: QR code
description: Share a link or short text your audience can scan with a phone.
cmdlets: [New-SlideQrCode]
---

Use a QR code to let people open a link from a slide without typing it. The black-on-white code
includes its quiet zone and scales in whole modules to fit the available space.

```powershell
New-Slide 'Scan me' {
    New-SlideQrCode 'https://example.com'
}
```

Keep the URL short: longer text makes the code larger. If the whole code cannot fit, a resize
message appears instead of a clipped code. Test scanning it with the terminal, font, and display
you plan to use when presenting.
