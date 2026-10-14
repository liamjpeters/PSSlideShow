---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/10/2026
PlatyPS schema version: 2024-05-01
title: New-SlideDynamic
---

# New-SlideDynamic

## SYNOPSIS

Adds a periodically updated view to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideDynamic [-Interval] <timespan> [-Update] <scriptblock> [-View] <scriptblock>
 [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Creates a dynamic widget whose data is refreshed in a dedicated background runspace. The output of
**Update** becomes a snapshot, and **View** receives that snapshot and emits the ordinary slide widgets
to display.

The first update runs before the slide's initial frame is rendered. Later updates run periodically in
the background while the slide is active. The view script block runs on the presentation thread when
a new snapshot is ready, so it can use the existing `New-Slide*` widget commands. Rendering reads the
latest completed view and does not invoke either script block.

A single output object from **Update** is passed directly to **View**. Multiple output objects are passed
as an object array. No output is passed as `$null`. Updates that fail retain the last successful view
and display the error when no successful view exists.

## EXAMPLES

### Example 1: CPU and memory values

```powershell
$dynamic = @{
    Interval = [TimeSpan]::FromSeconds(1)
    Update = {
        $cpu = (Get-Counter '\Processor(_Total)\% Processor Time').CounterSamples.CookedValue
        $os = Get-CimInstance Win32_OperatingSystem
        $total = [double]$os.TotalVisibleMemorySize
        $free = [double]$os.FreePhysicalMemory

        [pscustomobject]@{
            CpuPercent = [math]::Round($cpu, 1)
            UsedMemoryGB = [math]::Round(($total - $free) / 1MB, 2)
            TotalMemoryGB = [math]::Round($total / 1MB, 2)
        }
    }
    View = {
        param($snapshot)
        $table = @{
            Title = 'System health'
            Columns = @('Metric', 'Value')
            Rows = @(
                [pscustomobject]@{ Metric = 'CPU'; Value = "$($snapshot.CpuPercent)%" }
                [pscustomobject]@{ Metric = 'Memory'; Value = "$($snapshot.UsedMemoryGB) GB / $($snapshot.TotalMemoryGB) GB" }
            )
        }
        New-SlideTable @table
    }
}

New-SlideDynamic @dynamic
```

Shows a table that refreshes once per second. The update callback collects data and the view callback
converts the latest snapshot into a normal table widget.

### Example 2: Multiple view widgets

```powershell
$dynamic = @{
    Interval = [TimeSpan]::FromSeconds(2)
    Update = { Get-Date }
    View = {
        param($snapshot)
        New-SlideText "Updated: $($snapshot.ToString('HH:mm:ss'))"
        New-SlideAlert -Severity Tip -Text 'The view can emit more than one widget.'
    }
}

New-SlideDynamic @dynamic
```

The view can emit multiple widgets in order. They are rendered together as the dynamic widget's current
view.

## PARAMETERS

### -HPadding

Left/right padding, in cells, around the widget. The default is 0.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Interval

Time between completed background updates. The value must be positive.

```yaml
Type: System.TimeSpan
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Update

Script block that runs in a dedicated background runspace and produces the snapshot passed to **View**.
Keep the callback self-contained because it does not run in the presentation runspace.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -VPadding

Top/bottom padding, in rows, around the widget. The default is 1.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -View

Script block that runs on the presentation thread when an update completes. It receives the snapshot
as its first argument and through `$_` and `$PSItem`, and emits the slide widgets to render.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 2
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### PSSlideShow.Model.DynamicWidget

A dynamic widget to be emitted inside a `New-Slide` content block.

## NOTES

The module owns the terminal and processes completed snapshots on the presentation thread. Background
updates do not call Spectre.Console directly. If an update takes longer than its interval, the next
update waits for it to finish instead of overlapping it. When leaving a slide, its dynamic workers are
cancelled and their runspaces are disposed.

## RELATED LINKS

- [New-Slide](New-Slide.md)
