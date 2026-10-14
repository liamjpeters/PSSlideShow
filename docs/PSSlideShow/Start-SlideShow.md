---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: Start-SlideShow
---

# Start-SlideShow

## SYNOPSIS

Launches the interactive slide show.

## SYNTAX

### Slides (Default)

```
Start-SlideShow [[-Slides] <Slide[]>] [-StartAt <int>] [-NoAlternateScreen] [-ContentPadding <int>]
 [-SyntaxTheme <SyntaxThemePreference>]
```

### Path

```
Start-SlideShow -Path <string> [-StartAt <int>] [-NoAlternateScreen] [-ContentPadding <int>]
 [-SyntaxTheme <SyntaxThemePreference>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Presents the supplied slides full-screen and keyboard-driven, blocking until the user quits.
Provide slides through the **Slides** parameter (or the pipeline), or load a deck script with
**Path**. While the show runs, use the arrow keys or Space to move between slides, Enter to run the
current slide's code, and `q`, `Esc`, or `Ctrl+C` to quit.

## EXAMPLES

### Example 1: Present a deck file

```powershell
Start-SlideShow -Path ./deck.ps1
```

Runs `deck.ps1`, collects every slide it emits, and presents them full-screen.

### Example 2: Present slides from the pipeline

```powershell
./deck.ps1 | Start-SlideShow -StartAt 3
```

Pipes the slides emitted by `deck.ps1` into the show and opens on the third slide.

## PARAMETERS

### -ContentPadding

Left/right gutter, in cells, that insets the slide content from the terminal edges. The title,
rules, and footer stay full-width. The default is 2; set 0 to run content flush to the edge.

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

### -NoAlternateScreen

Renders inline in the current buffer instead of switching to the terminal's alternate screen. Useful
for debugging or when running in a host that does not support the alternate screen buffer.

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

### -Path

Path to a `.ps1` deck whose emitted slides are collected and presented. Mutually exclusive with
**Slides**.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Path
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Slides

The slides to present, as produced by `New-Slide`. Accepts pipeline input.

```yaml
Type: PSSlideShow.Model.Slide[]
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Slides
  Position: 0
  IsRequired: false
  ValueFromPipeline: true
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -StartAt

The 1-based index of the slide to open on. The default is 1 (the first slide).

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

### -SyntaxTheme

Palette used to syntax-highlight code blocks. `Auto` (the default) detects the terminal background
and picks a light or dark palette; `Dark` or `Light` forces one. Supply an explicit value when the
automatic detection guesses wrong for your terminal.

```yaml
Type: PSSlideShow.Rendering.SyntaxThemePreference
DefaultValue: Auto
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
AcceptedValues:
- Auto
- Dark
- Light
HelpMessage: ''
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSSlideShow.Model.Slide[]

Slides produced by `New-Slide` can be piped to this cmdlet.

## OUTPUTS

### System.Object

This cmdlet does not return output. It takes over the terminal until the show is quit.

## NOTES

The cmdlet switches the console output encoding to UTF-8 for the duration of the show so that
bullets, box-drawing, and tree glyphs render correctly, then restores the previous encoding on exit.

## RELATED LINKS

- [New-Slide](New-Slide.md)

