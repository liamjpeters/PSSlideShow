---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideRows
---

# New-SlideRows

## SYNOPSIS

Arranges regions stacked vertically.

## SYNTAX

### __AllParameterSets

```
New-SlideRows [-Body] <scriptblock> [-Fill] [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Arranges its child regions stacked vertically in rows. The **Body** script block should contain
`New-SlideRow` calls, each defining one row. Widgets emitted directly in the body are wrapped in
their own row.

## EXAMPLES

### Example 1: Two rows

```powershell
New-SlideRows {
    New-SlideRow { New-SlideText -Text 'Top' }
    New-SlideRow { New-SlideText -Text 'Bottom' }
}
```

Stacks two text widgets vertically.

## PARAMETERS

### -Body

The script block whose `New-SlideRow` calls build the rows.

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

Ratio/Size/MinSize on the child rows only take effect when **Fill** is specified.

## RELATED LINKS

- [New-SlideRow](New-SlideRow.md)
- [New-SlideColumns](New-SlideColumns.md)

