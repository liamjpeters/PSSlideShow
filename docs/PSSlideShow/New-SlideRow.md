---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideRow
---

# New-SlideRow

## SYNOPSIS

Defines a row region inside New-SlideRows.

## SYNTAX

### __AllParameterSets

```
New-SlideRow [[-Body] <scriptblock>] [-Ratio <int>] [-Size <int>] [-MinSize <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Defines one row region for a `New-SlideRows` layout. The **Body** script block supplies the widgets
that fill the row. **Ratio**, **Size**, and **MinSize** control the row's height when the parent
layout uses `-Fill`.

## EXAMPLES

### Example 1: A fixed-height row

```powershell
New-SlideRows -Fill {
    New-SlideRow -Size 3 { New-SlideText -Text 'Header' }
    New-SlideRow { New-SlideText -Text 'Body' }
}
```

Gives the header row a fixed height of 3 rows and lets the body fill the rest.

## PARAMETERS

### -Body

The script block whose widgets fill this row.

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

The minimum row height in cells. Applies only when the parent `New-SlideRows` uses `-Fill`.

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

The proportional share of the available height. Applies only when the parent `New-SlideRows` uses
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

A fixed row height in cells. Applies only when the parent `New-SlideRows` uses `-Fill`.

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

A layout region consumed by `New-SlideRows`.

## NOTES

Use this cmdlet only inside a `New-SlideRows` body.

## RELATED LINKS

- [New-SlideRows](New-SlideRows.md)

