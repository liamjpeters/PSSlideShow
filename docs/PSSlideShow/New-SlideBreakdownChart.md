---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideBreakdownChart
---

# New-SlideBreakdownChart

## SYNOPSIS

Adds a proportional breakdown chart to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideBreakdownChart [-Item] <Object[]> [-ShowPercentage] [-HideTags] [-HideTagValues]
 [-FullSize] [-ValueFormatter <scriptblock>] [-Width <int>] [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a proportional breakdown (stacked-bar) chart to a slide. Items may be hashtables or objects with
`Label`, `Value`, and optional `Color` members, supplied through **Item** or the pipeline.

## EXAMPLES

### Example 1: A breakdown with percentages

```powershell
New-SlideBreakdownChart -Item @(
    @{ Label = 'C#';         Value = 70; Color = 'Blue' }
    @{ Label = 'PowerShell'; Value = 30; Color = 'Cyan' }
) -ShowPercentage
```

Adds a breakdown chart with each slice labeled by percentage.

## PARAMETERS

### -FullSize

Renders the full-size (non-compact) layout.

```yaml
Type: System.Management.Automation.SwitchParameter
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

### -HideTags

Hides the tag (legend) row.

```yaml
Type: System.Management.Automation.SwitchParameter
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

### -HideTagValues

Hides the value shown next to each tag.

```yaml
Type: System.Management.Automation.SwitchParameter
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

### -Item

The chart items, as hashtables or objects with `Label`, `Value`, and optional `Color` members.
Accepts pipeline input.

```yaml
Type: System.Object[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: true
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ShowPercentage

Shows each slice as a percentage of the total.

```yaml
Type: System.Management.Automation.SwitchParameter
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

### -ValueFormatter

A script block that formats each value for display. The value is available as `$_`.

```yaml
Type: System.Management.Automation.ScriptBlock
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

### -Width

The chart width in cells. Defaults to the content width.

```yaml
Type: System.Nullable`1[System.Int32]
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object[]

Chart items (hashtables or objects) can be piped to this cmdlet.

## OUTPUTS

### PSSlideShow.Model.BreakdownChartWidget

A breakdown chart widget to be emitted inside a `New-Slide` content block.

## NOTES

The chart renders in a compact layout by default; use **FullSize** for the taller layout.

## RELATED LINKS

- [New-SlideBarChart](New-SlideBarChart.md)

