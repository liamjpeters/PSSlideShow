---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/04/2026
PlatyPS schema version: 2024-05-01
title: New-SlideAlert
---

# New-SlideAlert

## SYNOPSIS

Adds a GitHub-style alert callout to a slide.

## SYNTAX

### __AllParameterSets

```
New-SlideAlert [-Severity] <AlertSeverity> [-Text] <string> [[-Title] <string>] [-NoSymbol]
 [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Renders a callout modelled on GitHub's Markdown alerts: a thick coloured bar down the left edge, a
coloured title (prefixed with a severity symbol), and body text below it. The colour and symbol are
chosen from the `-Severity`:

| Severity    | Colour | Symbol |
| ----------- | ------ | ------ |
| `Note`      | Blue   | ⓘ      |
| `Tip`       | Green  | ★      |
| `Important` | Purple | ❢      |
| `Warning`   | Amber  | ▲      |
| `Caution`   | Red    | ⊘      |

The body text supports Spectre.Console markup such as `[bold]` and `[green]`. Use this cmdlet inside a
`New-Slide` content block.

## EXAMPLES

### Example 1: A warning callout

```powershell
New-SlideAlert -Severity Warning -Text 'This action cannot be undone.'
```

Adds an amber callout titled *Warning*.

### Example 2: A tip with a custom title and markup

```powershell
New-SlideAlert -Severity Tip -Title 'Pro tip' -Text 'Press [green]Enter[/] to run the code.'
```

Adds a green callout titled *Pro tip* with markup in the body.

## PARAMETERS

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

### -NoSymbol

Hides the severity symbol shown before the title.

```yaml
Type: System.Management.Automation.SwitchParameter
DefaultValue: 'False'
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

### -Severity

The alert severity, which selects the colour and symbol: `Note`, `Tip`, `Important`, `Warning`, or
`Caution`.

```yaml
Type: PSSlideShow.Model.AlertSeverity
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
AcceptedValues:
- Note
- Tip
- Important
- Warning
- Caution
HelpMessage: ''
```

### -Text

The body text. May contain Spectre.Console markup.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 1
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Title

The heading shown for the alert. Defaults to the severity name (for example `Warning`).

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: (All)
  Position: 2
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

### PSSlideShow.Model.AlertWidget

An alert widget to be emitted inside a `New-Slide` content block.

## NOTES

Markup tags in the body must be balanced (for example `[bold]...[/]`); malformed markup renders
literally. Use `-NoSymbol` if your terminal does not render the severity glyphs.

## RELATED LINKS

- [New-Slide](New-Slide.md)
- [New-SlideText](New-SlideText.md)
