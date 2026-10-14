---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideBarChart
---

# New-SlideBarChart

## SYNOPSIS

Adds a horizontal bar chart to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideBarChart [-Item] <Object[]> [-Label <string>] [-LabelAlignment <Justify>]
 [-MaxValue <double>] [-HideValues] [-ValueFormatter <scriptblock>] [-Width <int>] [-HPadding <int>]
 [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a horizontal bar chart to a slide. Items may be hashtables
(`@{ Label = 'A'; Value = 10; Color = 'Green' }`) or objects with matching properties, supplied
through **Item** or the pipeline.

## EXAMPLES

### Example 1: A simple bar chart

```powershell
New-SlideBarChart -Item @(
    @{ Label = 'Alpha'; Value = 40; Color = 'Green' }
    @{ Label = 'Beta';  Value = 25; Color = 'Yellow' }
    @{ Label = 'Gamma'; Value = 15; Color = 'Red' }
) -Label 'Usage'
```

Adds a bar chart titled *Usage* with three colored bars.

## PARAMETERS

### -HideValues

Hides the numeric value shown next to each bar.

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

### -Label

An optional heading shown above the chart.

```yaml
Type: System.String
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

### -LabelAlignment

Alignment of the heading: `Left`, `Center` (default), or `Right`.

```yaml
Type: Spectre.Console.Justify
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

### -MaxValue

Fixes the value-axis maximum. When omitted, it is inferred from the data.

```yaml
Type: System.Nullable`1[System.Double]
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

### PSSlideShow.Model.BarChartWidget

A bar chart widget to be emitted inside a `New-Slide` content block.

## NOTES

Color values accept Spectre.Console color names or hex values; items without a color use the default
palette.

## RELATED LINKS

- [New-SlideBreakdownChart](New-SlideBreakdownChart.md)

