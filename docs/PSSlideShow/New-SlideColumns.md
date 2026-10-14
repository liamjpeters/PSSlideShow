---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideColumns
---

# New-SlideColumns

## SYNOPSIS

Arranges regions side-by-side.

## SYNTAX

### __AllParameterSets

```
New-SlideColumns [-Body] <scriptblock> [-Fill] [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Arranges its child regions side-by-side in columns. The **Body** script block should contain
`New-SlideColumn` calls, each defining one column. Widgets emitted directly in the body are wrapped
in their own column.

## EXAMPLES

### Example 1: Two columns

```powershell
New-SlideColumns {
    New-SlideColumn { New-SlideText -Text 'Left' }
    New-SlideColumn { New-SlideText -Text 'Right' }
}
```

Lays out two text widgets side by side.

## PARAMETERS

### -Body

The script block whose `New-SlideColumn` calls build the columns.

```yaml
Type: System.Management.Automation.ScriptBlock
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

### -Fill

Uses a fixed-size layout that clips overflow instead of a content-sized flow layout.

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

## OUTPUTS

### PSSlideShow.Model.LayoutWidget

A layout widget to be emitted inside a `New-Slide` content block.

## NOTES

Ratio/Size/MinSize on the child columns only take effect when **Fill** is specified.

## RELATED LINKS

- [New-SlideColumn](New-SlideColumn.md)
- [New-SlideRows](New-SlideRows.md)

