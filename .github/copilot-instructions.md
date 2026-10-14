# Copilot instructions for PSSlideShow

Guidance for AI agents working in this repo. Read this first, then the linked docs as needed.

## What this is

PSSlideShow is a **binary PowerShell module written in C#** that renders an interactive, keyboard-driven
**terminal slide show** using [Spectre.Console](https://spectreconsole.net/). Authors write a deck as a
`.ps1` script that emits slides via a `New-Slide*` DSL, then present it with `Start-SlideShow`.

- **Multi-targeted:** one code base builds `net10.0` (PowerShell 7.6+) and `net472` (Windows PowerShell 5.1).
  Both must keep working.
- **Dependency loading:** PowerShell 7 loads private assemblies into an isolated `AssemblyLoadContext`
  and resolves native Onigwrap from the module's runtime folder. Windows PowerShell 5.1 uses
  `AssemblyResolve` and native preloading instead; isolation there is best-effort, not guaranteed.

## Architecture (where code lives)

Data flows **Cmdlets (DSL) → Model (data) → Rendering (frame) → Hosting (run loop)**. See
[docs/architecture.md](../docs/architecture.md) for the directory map and full walkthrough. Widgets
implement `ISlideWidget.Render`, which **must be side-effect free** (it is called every frame) and draw
only from its `WidgetRenderContext`, never global state — this keeps them testable against a Spectre
`TestConsole`.

## Build

```powershell
pwsh -NoProfile -File .\build.ps1 -Configuration Debug   # build both TFMs, stage help, run xUnit + Pester
pwsh -NoProfile -File .\build.ps1 -SkipTests             # build and stage binaries, license, and help
```

`build.ps1` restores/builds both frameworks, wipes and re-stages `module/bin/<tfm>` (module assembly in
`bin/<tfm>`, private deps in `bin/<tfm>/dependencies` which default probing does **not** search),
stages the MIT license and compiled MAML help, then runs the tests. Building requires the .NET 10 SDK
and Microsoft.PowerShell.PlatyPS v1 **even with `-SkipTests`**; tests also require Pester v6. Extra
switches: `-SkipDotNetTests`, `-SkipPesterTests`, `-Docs` (adds PlatyPS Markdown validation).

> **Native DLL lock:** `libonigwrap.dll` (used for syntax highlighting) stays locked by any `pwsh`
> process that imported the module, so the staging `Remove-Item` fails until that terminal closes. If a
> build fails while deleting `module/bin`, close the terminal that ran the module (or use a fresh
> throwaway session) and rebuild. Don't work around it by skipping staging.

## Testing

Two suites; run both (that's what `build.ps1` does):

- **xUnit** in `tests/PSSlideShow.Tests/` — unit tests for the C# code (rendering, execution, input,
  widgets). `dotnet test tests/PSSlideShow.Tests/PSSlideShow.Tests.csproj -c Debug`.
- **Pester v6** in `tests/pester/` — tests the staged module's surface and docs. Import the module via the
  manifest in `BeforeAll` for ordinary tests; tests that need command metadata for discovery-time
  `-ForEach` data must import it in `BeforeDiscovery` instead. Remove it in `AfterAll`.

On Windows, `build.ps1` runs Pester in both PowerShell 7 and Windows PowerShell 5.1; on other platforms,
only PowerShell 7. The [CI workflow](workflows/ci.yml) builds and tests PowerShell 7 on Windows,
Ubuntu, and macOS, with the Windows PowerShell 5.1 suite on Windows only.

**Pester v6 conventions (important):**

- Use the **hyphenated** `Should-*` assertions, e.g. `Should-Be`, `Should-NotBe`, `Should-BeTrue`,
  `Should-BeFalse`, `Should-BeTruthy`, `Should-NotBeNull`, `Should-MatchString`, `Should-NotMatchString`.
  There is **no** space-separated `Should -Be` in v6. `Should-MatchString`/`Should-NotMatchString` take
  the pattern via `-Expected` (regex).
- Respect the **two-phase model**. Data that drives `-ForEach` (parameterized tests) must be built during
  **Discovery** — put it at the top level or in a `BeforeDiscovery { }` block, **not** `BeforeAll`.
  Values used *inside* `It`/`BeforeAll` bodies at run time must be (re)established in `BeforeAll`, because
  variables assigned during Discovery do not flow into the Run phase. `-ForEach` expands **hashtable**
  keys (`@{ Name = ... }`) into variables — `[pscustomobject]` properties are not expanded.

See [tests/pester/Docs.Tests.ps1](../tests/pester/Docs.Tests.ps1) for a worked example of that split.

## Keeping docs in sync

Command help is authored as **Markdown under `docs/PSSlideShow/`** (one file per cmdlet plus
`PSSlideShow.md`) using [Microsoft.PowerShell.PlatyPS](https://learn.microsoft.com/powershell/utility-modules/platyps/overview)
(v1). This Markdown is the **source of truth** and is committed. The compiled MAML
(`PSSlideShow.dll-Help.xml`) is a **build artifact** placed next to each staged DLL — **do not commit it**.

The normal build compiles and stages MAML help. To work on the Markdown separately (the module must
already be staged):

```powershell
.\build-docs.ps1                              # validate Markdown and re-stage MAML without rebuilding
.\build-docs.ps1 -Update -SkipTest -SkipMaml  # sync existing pages before new help text is finished
.\build.ps1 -Docs                             # build, validate Markdown, stage help, run both test suites
```

When you **add or change a cmdlet or parameter**:

1. Stage the changed assembly with `pwsh -NoProfile -File .\build.ps1 -SkipTests`. The documentation
   guard cannot pass until the help is updated.
2. For a **new cmdlet**, first create its Markdown help page with `New-MarkdownCommandHelp` as shown in
   [docs/adding-a-widget.md](../docs/adding-a-widget.md); `-Update` only refreshes pages that already
   exist. In a throwaway `pwsh`, run `.\build-docs.ps1 -Update -SkipTest -SkipMaml` to sync parameters,
   syntax, and the module landing page without validating incomplete help. It imports the module and
   locks its native dependency on Windows.
3. Fill in every `{{ ... }}` placeholder using the C# XML doc comments (`<para type="synopsis">`,
   `<para type="description">`, `<example>`) as source material, then delete the `.bak` files. Exit the
   throwaway session before rebuilding.
4. Run `pwsh -NoProfile -File .\build.ps1 -Configuration Debug -Docs` to validate and stage help and
   run xUnit plus Pester (in both PowerShell editions on Windows).

**Docs guardrail:** [tests/pester/Docs.Tests.ps1](../tests/pester/Docs.Tests.ps1) fails if a cmdlet or
parameter lacks a help section, if any `{{ ... }}` placeholder remains, or if a help file is orphaned
(no matching exported cmdlet). Keep it green — it is what prevents docs drift.

MAML gotchas (already handled by `build-docs.ps1`, but know them):

- `Export-MamlCommandHelp` nests output under `<OutputFolder>/<ModuleName>/…` — export to a temp folder,
  then copy the `*-Help.xml` out.
- Get-Help resolves a **binary** cmdlet's external help **next to the DLL** in a `$PSUICulture` subfolder
  (`module/bin/<tfm>/<PSUICulture>/PSSlideShow.dll-Help.xml`) for **both** TFMs — not next to the `.psd1`.
  `build-docs.ps1` defaults `-Locale` to `$PSUICulture` for this reason.
- Staging wipes `module/bin`, but `build.ps1` regenerates MAML immediately afterward. Use
  `build-docs.ps1` directly only when updating help without rebuilding the module.

## Repo conventions

- **Script encoding:** any `.ps1` containing non-ASCII characters (em dash `—`, bullets, box-drawing,
  accents) **must be saved as UTF-8 *with* BOM**. Windows PowerShell 5.1 parses BOM-less files as ANSI
  using the system code page and mangles UTF-8 at parse time. Pure-ASCII files don't need the BOM.
  `example.ps1` is UTF-8-with-BOM for this reason.
- **Manifest:** exported cmdlets are listed in `module/PSSlideShow.psd1` (`CmdletsToExport`). Add new
  cmdlets there.
- Prefer editing existing files and following existing patterns; keep both target frameworks building.

## Extension points

- **Add a widget:** [docs/adding-a-widget.md](../docs/adding-a-widget.md) — a `Model/Widgets` class
  inheriting `SlideWidget` plus a `New-Slide*` cmdlet; register it in the manifest and add tests + help.
- **Add a keyboard shortcut:** [docs/adding-a-keyboard-shortcut.md](../docs/adding-a-keyboard-shortcut.md)
  — extend `Input/KeyboardShortcuts` / `ShortcutAction`; the footer hints come from the same source.

## After making changes

Run `pwsh -NoProfile -File .\build.ps1 -Configuration Debug` for both targets and test suites. If a
cmdlet or parameter changed, follow **Keeping docs in sync** above first, then use `-Docs` on the final
build. The Pester suite includes `tests/pester/Docs.Tests.ps1` to catch missing or incomplete help.
