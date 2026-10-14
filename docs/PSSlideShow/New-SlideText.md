---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideText
---

# New-SlideText

## SYNOPSIS

Adds a paragraph of text to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideText [-Text] <string> [-Justify <Justify>] [-Foreground <string>] [-HPadding <int>]
 [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a line or paragraph of text to a slide. The text supports Spectre.Console markup such as
`[bold]`, `[green]`, and `[underline]`. Use this cmdlet inside a `New-Slide` content block.

## EXAMPLES

### Example 1: Centered, colored text

```powershell
New-SlideText -Text 'Hello, [bold green]world[/]!' -Justify Center
```

Adds a centered paragraph with the word *world* rendered bold and green.

## PARAMETERS

### -Foreground

Optional foreground color, given as a color name or hex value (for example `green` or `#ff8800`).

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

### -Justify

Horizontal justification of the text: `Left` (default), `Center`, or `Right`.

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

### -Text

The text to display. May contain Spectre.Console markup.

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

### PSSlideShow.Model.TextWidget

A text widget to be emitted inside a `New-Slide` content block.

## NOTES

Markup tags must be balanced (for example `[bold]...[/]`); malformed markup renders literally.

## RELATED LINKS

- [New-Slide](New-Slide.md)
- [New-SlideFiglet](New-SlideFiglet.md)

