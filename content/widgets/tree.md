---
title: Tree
description: Explain hierarchy with nested nodes.
cmdlets: [New-SlideTree, New-SlideTreeNode]
---

A tree is a good fit for a directory layout, a process breakdown, or an organisation chart.
Nested node bodies build the hierarchy.

```powershell
New-Slide 'Project' {
    New-SlideTree 'PSSlideShow' {
        New-SlideTreeNode 'src' {
            New-SlideTreeNode 'Rendering'
            New-SlideTreeNode 'Hosting'
        }
        New-SlideTreeNode 'docs'
    }
}
```

Keep trees shallow enough to fit the screen. The `Ascii` guide style is available for terminals
without box-drawing support.
