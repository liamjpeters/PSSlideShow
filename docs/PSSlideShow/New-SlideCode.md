---
document type: cmdlet
external help file: PSSlideShow.dll-Help.xml
HelpUri: ''
Locale: en-GB
Module Name: PSSlideShow
ms.date: 08/03/2026
PlatyPS schema version: 2024-05-01
title: New-SlideCode
---

# New-SlideCode

## SYNOPSIS

Adds a syntax-highlighted code block to a slide.

## SYNTAX

### ScriptBlock (Default)

```
New-SlideCode [-ScriptBlock] <scriptblock> [-Setup <scriptblock>] [-Cleanup <scriptblock>]
 [-Language <string>] [-NonRunnable] [-HideLineNumbers] [-Container <CodeContainer>]
 [-HPadding <int>] [-VPadding <int>]
```

### Code

```
New-SlideCode -Code <string> [-Language <string>] [-NonRunnable] [-HideLineNumbers]
 [-Container <CodeContainer>] [-HPadding <int>] [-VPadding <int>]
```

### FilePath

```
New-SlideCode -FilePath <string> [-StartLine <int>] [-EndLine <int>] [-RunFromLine <int>]
 [-RunToLine <int>] [-Language <string>] [-NonRunnable] [-HideLineNumbers]
 [-Container <CodeContainer>] [-HPadding <int>] [-VPadding <int>]
```

## ALIASES

This cmdlet has no aliases.

## DESCRIPTION

Adds a syntax-highlighted code block to a slide. Supply the code with **ScriptBlock** (the default),
**Code** (a here-string, typically for non-PowerShell code), or **FilePath** (read from a file, with
optional **StartLine**/**EndLine** slicing).

PowerShell blocks are runnable by default: while presenting, press Enter to execute the block and
capture its output beneath the code. Pass **NonRunnable** to make a PowerShell block display-only.
Code in other languages is always display-only.

In the **ScriptBlock** set, **Setup** and **Cleanup** run around the visible block (before and after)
without being displayed, which is handy for preparing and tidying up demo state. In the **FilePath**
set, **StartLine**/**EndLine** select the displayed slice while **RunFromLine**/**RunToLine** select a
(usually wider) slice to execute; the displayed lines must fall within the executed lines. All four
line numbers are optional — an absent start implies the first line and an absent end implies the last.

## EXAMPLES

### Example 1: A runnable PowerShell block

```powershell
New-SlideCode -ScriptBlock { Get-Process | Select-Object -First 5 }
```

Adds a highlighted, runnable PowerShell block.

### Example 2: Display-only code from a file

```powershell
New-SlideCode -FilePath ./Program.cs -Language csharp -StartLine 10 -EndLine 30
```

Shows lines 10-30 of `Program.cs` with C# highlighting and line numbers starting at 10.

### Example 3: Invisible setup and cleanup

```powershell
New-SlideCode -Setup { $conn = Connect-Demo } -Cleanup { $conn.Dispose() } {
    $conn.GetData()
}
```

Displays only the `$conn.GetData()` line, but executes the connect step first and disposes afterward
(in a `finally`, so cleanup runs even if the block throws).

### Example 4: Show a slice, run a wider slice

```powershell
New-SlideCode -FilePath ./demo.ps1 -StartLine 8 -EndLine 12 -RunFromLine 1 -RunToLine 20
```

Displays lines 8-12 but executes lines 1-20, so earlier setup lines run without cluttering the slide.

## PARAMETERS

### -Cleanup

Script run after the visible block, inside a `finally` so it runs even if the block throws. It is not
displayed. Only valid in the **ScriptBlock** parameter set.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ScriptBlock
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Code

Literal code as text, typically a here-string. This is the route for inline non-PowerShell code.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: Code
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Container

The visual container style around the code. The default is `Simple`.

```yaml
Type: PSSlideShow.Model.CodeContainer
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

### -EndLine

The last line of the file to display (inclusive). Only valid with **FilePath**.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: FilePath
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -FilePath

Path to a file to read the code from.

```yaml
Type: System.String
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: FilePath
  Position: Named
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -HideLineNumbers

Hides the line-number gutter.

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

### -Language

The grammar id used for highlighting; it also determines the block title and runnability. The default
is `powershell`.

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

### -NonRunnable

Marks a PowerShell block as display-only so Enter does not execute it. Has no effect on other
languages, which are never runnable.

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

### -RunFromLine

The first line of the file to execute (1-based). Defaults to **StartLine**. Must be less than or equal
to **StartLine** so the displayed lines fall within the executed lines. Only valid with **FilePath**.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: FilePath
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -RunToLine

The last line of the file to execute (inclusive). Defaults to **EndLine**. Must be greater than or
equal to **EndLine** so the displayed lines fall within the executed lines. Only valid with
**FilePath**.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: FilePath
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -ScriptBlock

Literal PowerShell to display and, unless **NonRunnable** is specified, execute on demand.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ScriptBlock
  Position: 0
  IsRequired: true
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -Setup

Script run before the visible block. It is not displayed. Use it to prepare demo state. Only valid in
the **ScriptBlock** parameter set.

```yaml
Type: System.Management.Automation.ScriptBlock
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: ScriptBlock
  Position: Named
  IsRequired: false
  ValueFromPipeline: false
  ValueFromPipelineByPropertyName: false
  ValueFromRemainingArguments: false
DontShow: false
AcceptedValues: []
HelpMessage: ''
```

### -StartLine

The first line of the file to display (1-based). Line numbering starts here. Only valid with
**FilePath**.

```yaml
Type: System.Int32
DefaultValue: ''
SupportsWildcards: false
Aliases: []
ParameterSets:
- Name: FilePath
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

### PSSlideShow.Model.CodeWidget

A code widget to be emitted inside a `New-Slide` content block.

## NOTES

Runnable blocks execute in the slide's runspace, honoring the **CodeScope** set on `New-Slide`.
Leading indentation shared by all lines is stripped so the code aligns to the left edge.

## RELATED LINKS

- [New-Slide](New-Slide.md)

