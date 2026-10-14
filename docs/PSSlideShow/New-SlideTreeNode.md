---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideTreeNode
---

# New-SlideTreeNode

## SYNOPSIS

Adds a node to the enclosing tree.

## SYNTAX

### __AllParameterSets

```
New-SlideTreeNode [-Label] <string> [[-Body] <scriptblock>] [-Collapsed]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a node to the enclosing tree. Valid only inside a `New-SlideTree` body (or another
`New-SlideTreeNode` body). Supply a **Body** to nest further child nodes. Labels support
Spectre.Console markup.

## EXAMPLES

### Example 1: A nested node

```powershell
New-SlideTreeNode -Label 'Model' -Body {
    New-SlideTreeNode -Label 'Widgets'
}
```

Adds a *Model* node with a single *Widgets* child.

## PARAMETERS

### -Body

Optional script block of nested `New-SlideTreeNode` calls that become this node's children.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Collapsed

Renders this node collapsed, hiding its children.

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

### -Label

The node label. Supports Spectre.Console markup.

```yaml
Type: System.String
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### PSSlideShow.Model.TreeNodeWidget

A tree node consumed by the enclosing `New-SlideTree` or `New-SlideTreeNode`.

## NOTES

Using this cmdlet outside a tree body raises a terminating error.

## RELATED LINKS

- [New-SlideTree](New-SlideTree.md)

