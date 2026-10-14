---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-Slide
---

# New-Slide

## SYNOPSIS

Creates a single slide from a content script block.

## SYNTAX

### __AllParameterSets

```
New-Slide [[-Title] <string>] [-Content] <scriptblock> [-CodeScope <CodeScope>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Invokes the **Content** script block and collects every widget it emits, in order, as the slide's
content stack. The `New-Slide*` widget cmdlets are only meaningful inside this block. The resulting
slide object is typically piped to (or collected for) `Start-SlideShow`.

## EXAMPLES

### Example 1: A title slide

```powershell
New-Slide -Title 'Welcome' -Content {
    New-SlideFiglet -Text 'SlideShow'
    New-SlideText   -Text 'A terminal slide show.' -Justify Center
}
```

Builds a slide titled *Welcome* containing a FIGlet banner above a centered line of text.

## PARAMETERS

### -CodeScope

The runspace-sharing policy for runnable code on this slide. The default is `Slide`, which gives the
slide its own runspace so its code blocks share state with each other but not with other slides.

```yaml
Type: PSSlideShow.Model.CodeScope
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

### -Content

The script block that emits the slide's widgets. Required. Call `New-Slide*` cmdlets inside it to
build the slide's content stack.

```yaml
Type: System.Management.Automation.ScriptBlock
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

Optional title text shown at the top of the slide. An empty or absent title reserves a blank title
row so slides stay vertically aligned.

```yaml
Type: System.String
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable,
-InformationAction, -InformationVariable, -OutBuffer, -OutVariable, -PipelineVariable,
-ProgressAction, -Verbose, -WarningAction, and -WarningVariable. For more information, see
[about_CommonParameters](https://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

### PSSlideShow.Model.Slide

A slide object suitable for piping to `Start-SlideShow`.

## NOTES

The **Content** block runs once, at authoring time, to collect its widgets; it is not re-invoked
during the show.

## RELATED LINKS

- [Start-SlideShow](Start-SlideShow.md)

