---
title: Calendar
description: Highlight dates in a month view.
cmdlets: [New-SlideCalendar]
---

Calendars make a release plan or a sequence of events easier to follow than a long list of dates.
Pass dates as `-Event` values.

```powershell
New-Slide 'October milestones' {
    New-SlideCalendar -Year 2026 -Month 10 -Event @(
        [datetime]'2026-10-14'
        [datetime]'2026-10-28'
    )
}
```

Set `-Culture` when the audience expects a different first day of the week or month names.
