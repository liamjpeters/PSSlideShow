# Architecture

PSSlideShow is a binary PowerShell module written in C#. It targets **two frameworks** from one code
base — `net10.0` (PowerShell 7.6+) and `net472` (Windows PowerShell 5.1) — and renders through
[Spectre.Console](https://spectreconsole.net/). This document maps the pieces and how a keypress
becomes a repainted frame.

## Layers at a glance

```
Cmdlets (DSL)  ──►  Model (data)  ──►  Rendering (frame)  ──►  Hosting (run loop)
 New-Slide*         Slide / widgets     Renderer               SlideShowHost
                                        WidgetRenderContext    IInputSource / KeyboardShortcuts
                                                               Execution (runspaces)
```

Source lives under [src/PSSlideShow](../src/PSSlideShow), grouped by responsibility:

| Folder | Responsibility |
| --- | --- |
| `Cmdlets/` | The `New-Slide*` and `Start-SlideShow` cmdlets — the authoring surface. |
| `Authoring/` | `SlideBuildContext` (nesting/scope tracking) and script-block helpers. |
| `Model/` | Plain data: `Slide`, `SlideShow`, `ISlideWidget` and the widget classes. |
| `Rendering/` | `Renderer`, theme, footer, syntax highlighting, text/ANSI utilities. |
| `Input/` | `IInputSource`, `KeyboardShortcuts`, `ShortcutAction`, `SlideState`. |
| `Execution/` | `RunspaceManager` and `CodeRunner` for runnable code blocks. |
| `Hosting/` | `SlideShowHost` (run loop) and `ModuleInitializer` (dependency isolation). |

## Authoring: cmdlets build the model

Every widget cmdlet emits an [`ISlideWidget`](../src/PSSlideShow/Model/ISlideWidget.cs). `New-Slide`
invokes its `-Content` script block and collects whatever widgets it emits, in order, into a
[`Slide`](../src/PSSlideShow/Model/Slide.cs):

```csharp
foreach (var result in Content.Invoke())
    if (result?.BaseObject is ISlideWidget widget)
        widgets.Add(widget);
```

This capture-the-output approach is what makes the DSL feel declarative. Container widgets
(`New-SlideColumns`, `New-SlideTree`) do the same for their bodies. Nesting rules are enforced with
[`SlideBuildContext`](../src/PSSlideShow/Authoring/SlideBuildContext.cs), a thread-scoped stack: for
example `New-SlideTreeNode` calls `SlideBuildContext.InScope("Tree")` and throws if it is used outside
a tree body. `Start-SlideShow` collects the resulting `Slide` objects into a
[`SlideShow`](../src/PSSlideShow/Model/SlideShow.cs), which owns the current index and scroll offset.

## Model: widgets are data with a `Render` method

Widgets are inert data objects that know how to draw themselves. Most inherit
[`SlideWidget`](../src/PSSlideShow/Model/SlideWidget.cs), which adds `HorizontalPadding` /
`VerticalPadding` on top of the [`ISlideWidget`](../src/PSSlideShow/Model/ISlideWidget.cs) contract:

```csharp
public interface ISlideWidget
{
    IRenderable Render(WidgetRenderContext context);
    bool IsRunnable { get; }
}
```

`Render` returns a Spectre `IRenderable` and must be **side-effect free** — it is called every frame.
Everything it needs arrives in a [`WidgetRenderContext`](../src/PSSlideShow/Model/WidgetRenderContext.cs)
(the console, available width/height, and the theme), rather than from global state, which keeps widgets
testable against a `TestConsole`.

## Rendering: composing one frame

[`Renderer.Render`](../src/PSSlideShow/Rendering/Renderer.cs) builds a single frame for the current
slide:

1. Measure the terminal (`Profile.Width`/`Height`), reserving the final row so a full-height frame
   never scrolls the buffer.
2. Compute the content **gutter** (from `-ContentPadding`) and hand the widgets a width-reduced
   `WidgetRenderContext`. Title, rules, and footer stay full-width; only content is inset.
3. Render each top-level widget through `RenderPadded`, which applies the widget's own
   `HPadding`/`VPadding`.
4. Capture the composed content into lines ([`ContentCapture`](../src/PSSlideShow/Rendering/ContentCapture.cs)),
   apply the scroll offset, and detect overflow.
5. Assemble the frame as `Rows(title, rule, content, rule, footer)` and write it padded to every cell.

The frame is a pinned title, a scrollable content viewport, and a footer built by
[`FooterBuilder`](../src/PSSlideShow/Rendering/FooterBuilder.cs) from the same
[`KeyboardShortcuts`](../src/PSSlideShow/Input/KeyboardShortcuts.cs) source as the input handler. Colors
are semantic roles defined once in [`SlideTheme`](../src/PSSlideShow/Rendering/SlideTheme.cs), drawn from
the terminal's ANSI palette so the module never assumes a light or dark background.

Because the host repaints by homing the cursor (never clearing the whole screen) and every frame pads
all cells, each frame overwrites the previous one in place — no clear-then-draw flicker.

## Hosting: the run loop

[`SlideShowHost.Run`](../src/PSSlideShow/Hosting/SlideShowHost.cs) is the blocking loop:

```
enter alternate screen ─► render ─► wait for key ─► resolve action ─► dispatch ─► render ─► …
```

- **Input** is read through [`IInputSource`](../src/PSSlideShow/Input/IInputSource.cs). Production uses
  `ConsoleInputSource`; tests inject a scripted source, so the whole loop runs headless.
- **Key → action** goes through [`KeyboardShortcuts.Resolve`](../src/PSSlideShow/Input/KeyboardShortcuts.cs),
  producing a [`ShortcutAction`](../src/PSSlideShow/Input/ShortcutAction.cs).
- **Dispatch** mutates the `SlideShow` (navigate, scroll) or drives code execution (run/reset).
- While idle, the loop polls the terminal size and re-renders on resize so the slide always fits.

## Execution: runnable code blocks

A runnable [`CodeWidget`](../src/PSSlideShow/Model/Widgets/CodeWidget.cs) participates in the `[Enter]
Run` flow. [`RunnableCollector`](../src/PSSlideShow/Model/RunnableCollector.cs) gathers a slide's
runnable blocks in order (descending into layout containers). On `Enter`, the host asks
[`RunspaceManager`](../src/PSSlideShow/Execution/RunspaceManager.cs) for the block's runspace — keyed by
the slide's [`CodeScope`](../src/PSSlideShow/Model/CodeScope.cs) (`Slide` / `Show` / `Block`) — and
[`CodeRunner`](../src/PSSlideShow/Execution/CodeRunner.cs) executes it. The captured output and
run status live on the widget's `State` and are read while rendering. `R` disposes the slide's runspace
and resets each block.

## Dependency isolation

The module ships private dependencies (Spectre.Console, TextMateSharp, and native Onigwrap) in a
`dependencies/` subfolder outside default assembly probing. PowerShell 7 isolates them; Windows
PowerShell 5.1 resolves them best-effort from that folder. See [build.ps1](../build.ps1) for staging
and [PSSlideShow.psm1](../module/PSSlideShow.psm1) for the loader.

[`ModuleInitializer`](../src/PSSlideShow/Hosting/ModuleInitializer.cs) installs the resolver:

- **net10.0 (PowerShell 7):** a dedicated `AssemblyLoadContext` named `PSSlideShow`. A handler on
  `AssemblyLoadContext.Default.Resolving` redirects any assembly present in `dependencies/` into that
  context; framework assemblies fall through to the shared runtime. The custom context also resolves the
  native `libonigwrap` from `dependencies/runtimes/<rid>/native`.
- **net472 (Windows PowerShell 5.1):** no load contexts exist, so an `AppDomain.AssemblyResolve` handler
  loads deps from `dependencies/`, plus a `kernel32 LoadLibrary` native preload (best-effort — the
  runtime offers no true isolation there).

Because some cmdlets expose a Spectre type (`Justify`) as a parameter, importing the module forces that
dependency to load *while reflecting parameters*. The loader therefore runs in two steps: it loads the
assembly, calls `ModuleInitializer.Initialize()` (which touches only framework/ALC types, so it never
needs a private dependency), and only then imports by path so parameter reflection resolves through the
isolated context.

## Testing

- **xUnit** ([tests/PSSlideShow.Tests](../tests/PSSlideShow.Tests)) exercises the C# directly against a
  Spectre `TestConsole` — rendering, layout, code execution, and the run loop with a scripted input
  source. These do not import the module, so the isolation loader is not exercised here.
- **Pester** ([tests/pester](../tests/pester)) imports the built module and validates the cmdlet
  surface, the manifest, command help, and dependency isolation. On Windows,
  [build.ps1](../build.ps1) runs these tests in both PowerShell 7 and Windows PowerShell 5.1.

Both suites run from [build.ps1](../build.ps1) after binaries and command help are staged. See the
[README](../README.md#development) for the build command and CI platforms.
