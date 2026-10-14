---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideTable
---

# New-SlideTable

## SYNOPSIS

Adds a table to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideTable [-Rows] <Object[]> [-Columns <string[]>] [-Border <SlideBorder>] [-Title <string>]
 [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a table to a slide. Rows may be hashtables or objects, supplied through **Rows** or the
pipeline. Columns are inferred from the first row unless you supply **Columns** to fix the header set
and order.

## EXAMPLES

### Example 1: A table from hashtables

```powershell
New-SlideTable -Rows @(
    @{ Name = 'Alice'; Score = 92 }
    @{ Name = 'Bob';   Score = 87 }
) -Title 'Results'
```

Adds a rounded-border table captioned *Results* with **Name** and **Score** columns.

## PARAMETERS

### -Border

The table border style. The default is `Rounded`.

```yaml
Type: PSSlideShow.Model.SlideBorder
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

### -Columns

Explicit column headers and order. When omitted, columns are inferred from the first row.

```yaml
Type: System.String[]
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

### -Rows

The rows of data, as hashtables or objects. Accepts pipeline input.

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

### -Title

Optional table caption shown above the table.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object[]

Rows (hashtables or objects) can be piped to this cmdlet.

## OUTPUTS

### PSSlideShow.Model.TableWidget

A table widget to be emitted inside a `New-Slide` content block.

## NOTES

When columns are inferred, only the keys/properties present on the first row are used; later rows
with extra keys do not add columns.

## RELATED LINKS

- [New-Slide](New-Slide.md)

