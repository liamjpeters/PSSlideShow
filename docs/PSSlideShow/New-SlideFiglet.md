---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideFiglet
---

# New-SlideFiglet

## SYNOPSIS

Adds large FIGlet banner text to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideFiglet [-Text] <string> [-Justify <Justify>] [-Foreground <string>] [-FontPath <string>]
 [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Renders **Text** as a large FIGlet banner, ideal for slide titles and section dividers. The built-in
font is used unless you supply a custom FIGlet font with **FontPath**.

## EXAMPLES

### Example 1: A banner heading

```powershell
New-SlideFiglet -Text 'PSSlideShow' -Foreground cyan
```

Renders a centered cyan FIGlet banner.

## PARAMETERS

### -FontPath

Optional path to a FIGlet font (`.flf`) file. Defaults to the built-in font when omitted.

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

### -Foreground

Optional color, given as a color name or hex value.

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

Horizontal justification of the banner: `Left`, `Center` (default), or `Right`.

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

The text to render as a banner.

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

### PSSlideShow.Model.FigletWidget

A FIGlet widget to be emitted inside a `New-Slide` content block.

## NOTES

FIGlet output is wide; keep banner text short so it fits the slide's content width.

## RELATED LINKS

- [New-Slide](New-Slide.md)
- [New-SlideText](New-SlideText.md)

