# Adding a widget

A widget is a renderable unit of slide content. Adding one is the primary extension point of the module
and, for a display-only widget, requires no changes to the renderer, scrolling, or footer. There are two
parts: a **model class** that knows how to draw itself, and a **cmdlet** that authors it.

The walkthrough below adds a `New-SlideQuote` widget that renders an indented, styled block quote.

## 1. The model class

Create the widget under [src/PSSlideShow/Model/Widgets](../src/PSSlideShow/Model/Widgets). Inherit
[`SlideWidget`](../src/PSSlideShow/Model/SlideWidget.cs) (not `ISlideWidget` directly) so you inherit the
shared `HPadding`/`VPadding` behaviour for free. Implement `Render`, returning a Spectre `IRenderable`.

```csharp
using System.Collections.Generic;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model
{
    /// <summary>A styled block quote. Supports Spectre markup.</summary>
    public sealed class QuoteWidget : SlideWidget
    {
        public QuoteWidget(string text, string? attribution = null)
        {
            Text = text ?? string.Empty;
            Attribution = attribution;
        }

        public string Text { get; }
        public string? Attribution { get; }

        public override IRenderable Render(WidgetRenderContext context)
        {
            var rows = new List<IRenderable> { new Markup($"[italic]“{Text}”[/]") };
            if (!string.IsNullOrWhiteSpace(Attribution))
                rows.Add(new Markup($"— {Attribution}", context.Theme.Chrome));

            // Indent the whole quote a couple of cells using the available width from the context.
            return new Padder(new Rows(rows), new Padding(2, 0, 0, 0));
        }
    }
}
```

Key rules:

- **`Render` must be side-effect free.** It is called every frame; do all parsing/formatting from the
  constructor arguments and the [`WidgetRenderContext`](../src/PSSlideShow/Model/WidgetRenderContext.cs)
  (console, `Width`, `Height`, `Theme`). Never read global state.
- **Use the theme, not hard-coded colors.** Pull styles from `context.Theme`
  ([`SlideTheme`](../src/PSSlideShow/Rendering/SlideTheme.cs)) so your widget follows the terminal's
  palette. Use [`ColorResolver.Parse`](../src/PSSlideShow/Rendering/ColorResolver.cs) for a user-supplied
  color string.
- **Respect the width.** `context.Width` is the space you have; Spectre containers like `Rows`, `Grid`,
  and `Padder` wrap and fit to it.
- **`IsRunnable` defaults to `false`.** Leave it unless your widget participates in the `[Enter] Run`
  flow (only `CodeWidget` does today).

## 2. The cmdlet

Create the cmdlet under [src/PSSlideShow/Cmdlets/Widgets](../src/PSSlideShow/Cmdlets/Widgets). Inherit
[`WidgetCommand`](../src/PSSlideShow/Cmdlets/Widgets/WidgetCommand.cs) so you get `-HPadding`/`-VPadding`
automatically, and emit the widget with `WriteWidget` (which applies that padding).

```csharp
using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets
{
    /// <summary>
    /// <para type="synopsis">Adds a block quote to a slide.</para>
    /// <para type="description">Supports Spectre markup in the quote text.</para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideQuote")]
    [OutputType(typeof(QuoteWidget))]
    public sealed class NewSlideQuoteCommand : WidgetCommand
    {
        /// <summary>The quote text (may contain Spectre markup).</summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Text { get; set; } = string.Empty;

        /// <summary>Optional attribution shown under the quote.</summary>
        [Parameter(Position = 1)]
        public string? Attribution { get; set; }

        protected override void EndProcessing() =>
            WriteWidget(new QuoteWidget(Text, Attribution));
    }
}
```

Conventions:

- Name cmdlets `New-Slide<Thing>` with the `New` verb.
- Use `[OutputType(typeof(...))]` for discoverability.
- Write `<para type="synopsis">` / `<para type="description">` XML comments as source material for
  the authored Markdown help. The Markdown under [docs/PSSlideShow](PSSlideShow) is what gets compiled
  into `Get-Help`; the XML comments alone are not enough.
- Build once in `EndProcessing` (or accumulate pipeline input in `ProcessRecord`, then build in
  `EndProcessing` — see [`NewSlideTableCommand`](../src/PSSlideShow/Cmdlets/Widgets/NewSlideTableCommand.cs)).

### Container widgets (optional)

If your widget nests child cmdlets (like `New-SlideColumns`/`New-SlideColumn`), invoke a `-Body` script
block and push a scope so children can validate placement:

```csharp
using (SlideBuildContext.Push("Quote"))
{
    foreach (var result in Body.Invoke())
        if (result?.BaseObject is ISlideWidget child) children.Add(child);
}
```

Child cmdlets then guard with `SlideBuildContext.InScope("Quote")` and throw a terminating error if used
elsewhere — see [`NewSlideTreeCommand`](../src/PSSlideShow/Cmdlets/Widgets/NewSlideTreeCommand.cs) for the
pattern. Note that nested cmdlets inherit from `PSCmdlet` (not `WidgetCommand`), since padding applies
only to top-level widgets.

## 3. Register it for export

Add the cmdlet name to `CmdletsToExport` in [module/PSSlideShow.psd1](../module/PSSlideShow.psd1). This
list is explicit, so a new cmdlet is not visible until it is added.

## 4. Test it

Add an xUnit test that renders the widget against a `TestConsole` (see
[tests/PSSlideShow.Tests](../tests/PSSlideShow.Tests) and the `TestSupport` helpers) and a Pester test
asserting the cmdlet binds its parameters (see [tests/pester/Widgets.Tests.ps1](../tests/pester/Widgets.Tests.ps1)).

```csharp
[Fact]
public void Quote_indents_the_text()
{
    var output = TestSupport.Render(new QuoteWidget("Hello", "Ada"), width: 40, height: 10);
    Assert.Contains("Hello", output);
    Assert.Contains("Ada", output);
}
```

## 5. Build and document it

Stage the new cmdlet before generating its help. Skip tests for now because the documentation
guard cannot pass until the new page exists:

```powershell
pwsh -NoProfile -File .\build.ps1 -SkipTests
```

In a **throwaway `pwsh` session**, create the new command page and update the module landing page.
`-Update` refreshes existing pages but cannot create a page for a new cmdlet:

```powershell
Import-Module .\module\PSSlideShow.psd1
New-MarkdownCommandHelp -CommandInfo (Get-Command New-SlideQuote -Module PSSlideShow) -OutputFolder .\docs\PSSlideShow
.\build-docs.ps1 -Update -SkipTest -SkipMaml
```

Fill in the new page and any `{{ ... }}` placeholders with real prose, then delete PlatyPS's `.bak`
files. Close that session before rebuilding so its native DLL lock does not block staging. Finally:

```powershell
pwsh -NoProfile -File .\build.ps1 -Configuration Debug -Docs
```

The final build validates and stages `Get-Help`, then runs xUnit and Pester (in both PowerShell
editions on Windows).
