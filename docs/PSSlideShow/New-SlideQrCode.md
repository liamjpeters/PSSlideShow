---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 10/15/2026
PlatyPS schema version: 2024-05-01
title: New-SlideQrCode
---

# New-SlideQrCode

## SYNOPSIS

Adds a complete QR code to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideQrCode [-Text] <string> [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Encodes **Text** as a black-on-white QR code with medium error correction and a four-module
white quiet zone. It uses terminal half-blocks and only whole-number scales, so resizing never
stretches individual modules. The largest scale that fits both the available width and height is
used. The code is centred within its available width and height. If the whole code cannot fit,
a resize message appears instead of a clipped code.

The QR code can share a slide with other widgets or sit inside columns and rows. A slide with
more content may scroll; scroll until the whole QR code is visible before scanning it. Its quiet
zone replaces the usual widget spacing, so **VPadding** defaults to 0.

## EXAMPLES

### Example 1: A link for the audience

```powershell
New-Slide 'Scan me' {
    New-SlideQrCode -Text 'https://example.com'
}
```

Presents a QR code linking to a short URL. Try it in a real terminal and scan it with a phone;
longer text can require a larger code and therefore a larger terminal.

### Example 2: Feedback beside the code

```powershell
New-Slide 'Feedback' {
    New-SlideColumns -VPadding 0 {
        New-SlideColumn { New-SlideText 'Please give me feedback at this link.' }
        New-SlideColumn { New-SlideQrCode 'https://example.com' }
    }
}
```

Puts explanatory text next to the QR code. If its column is too narrow, the code is replaced
with a resize message rather than being clipped.

## PARAMETERS

### -HPadding

Left/right padding around the code in terminal cells. Defaults to 0. Padding reduces the width
available for the QR matrix.

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

### -Text

Text or a URL to encode. It cannot be empty or whitespace. Longer payloads produce larger QR
matrices, which may require a larger terminal window.

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

Top/bottom padding around the code in terminal rows. Defaults to 0 because the QR matrix already
includes its own quiet zone. Extra padding reduces the height available for the code.

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

### PSSlideShow.Model.QrCodeWidget

A QR code widget to emit inside a `New-Slide` content block.

## NOTES

Terminal character cells are not guaranteed to have the same physical proportions on every
font and terminal. Half-block rendering approximates square modules when cells are roughly
twice as tall as they are wide. Use a modern ANSI/VT-capable terminal and test scanning with
your intended display and font.

## RELATED LINKS

- [New-Slide](New-Slide.md)
