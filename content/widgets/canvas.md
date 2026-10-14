---
title: Canvas
description: Paint individual pixels for diagrams and small illustrations.
cmdlets: [New-SlideCanvas]
---

The `-Paint` script block runs while the deck is built, not once per frame. It receives the canvas
as `$_`, with `$Width` and `$Height` available for drawing.

```powershell
New-Slide 'Diagonal' {
    New-SlideCanvas -Width 16 -Height 16 -Paint {
        $_.Fill('grey15')
        for ($i = 0; $i -lt $Width; $i++) {
            $_.SetPixel($i, $i, 'lime')
        }
    }
}
```

Use `-MaxWidth` to scale a large canvas down to the available space.
