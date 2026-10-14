---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideColumn
---

# New-SlideColumn

## SYNOPSIS

Defines a column region inside New-SlideColumns.

## SYNTAX

### __AllParameterSets

```
New-SlideColumn [[-Body] <scriptblock>] [-Ratio <int>] [-Size <int>] [-MinSize <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Defines one column region for a `New-SlideColumns` layout. The **Body** script block supplies the
widgets that fill the column. Use **Ratio**, **Size**, and **MinSize** to control the column's width
when the parent layout uses `-Fill`.

## EXAMPLES

### Example 1: A weighted column

```powershell
New-SlideColumns -Fill {
    New-SlideColumn -Ratio 2 { New-SlideText -Text 'Wide' }
    New-SlideColumn -Ratio 1 { New-SlideText -Text 'Narrow' }
}
```

Gives the first column twice the width of the second.

## PARAMETERS

### -Body

The script block whose widgets fill this column.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 0
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -MinSize

The minimum column width in cells. Applies only when the parent `New-SlideColumns` uses `-Fill`.

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

### -Ratio

The proportional share of the available width. Applies only when the parent `New-SlideColumns` uses
`-Fill`.

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

### -Size

A fixed column width in cells. Applies only when the parent `New-SlideColumns` uses `-Fill`.

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

## OUTPUTS

### PSSlideShow.Model.LayoutRegion

A layout region consumed by `New-SlideColumns`.

## NOTES

Use this cmdlet only inside a `New-SlideColumns` body.

## RELATED LINKS

- [New-SlideColumns](New-SlideColumns.md)

