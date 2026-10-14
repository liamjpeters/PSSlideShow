# Adding a keyboard shortcut

[`KeyboardShortcuts`](../src/PSSlideShow/Input/KeyboardShortcuts.cs) maps keys to actions **and**
describes the footer hints, keeping input and on-screen controls in sync.

The walkthrough below adds `G` as an alias for jumping to the last slide, then shows how to add a new
action.

## The flow

```
key press ─► KeyboardShortcuts.Resolve ─► ShortcutAction ─► SlideShowHost.Dispatch ─► mutate SlideShow ─► re-render
```

- [`IInputSource`](../src/PSSlideShow/Input/IInputSource.cs) yields a `ConsoleKeyInfo`.
- [`KeyboardShortcuts.Resolve`](../src/PSSlideShow/Input/KeyboardShortcuts.cs) turns it into a
  [`ShortcutAction`](../src/PSSlideShow/Input/ShortcutAction.cs).
- [`SlideShowHost.Dispatch`](../src/PSSlideShow/Hosting/SlideShowHost.cs) acts on it.
- [`KeyboardShortcuts.FooterControls`](../src/PSSlideShow/Input/KeyboardShortcuts.cs) decides which hint
  appears in the footer, based on [`SlideState`](../src/PSSlideShow/Input/SlideState.cs).

## Binding an existing action to a new key

To make `G` an alias for "jump to last slide", add cases to the `Resolve` switch in
[`KeyboardShortcuts`](../src/PSSlideShow/Input/KeyboardShortcuts.cs):

```csharp
case ConsoleKey.End:
case ConsoleKey.G:                 // new alias
    return ShortcutAction.Last;
```

That is all — `Dispatch` already handles `ShortcutAction.Last`.

For a modifier shortcut, check the modifier explicitly so the unmodified key keeps its own meaning. For
example, `Ctrl+C` can be bound without making plain `C` quit:

```csharp
if (key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control)) {
    return ShortcutAction.Quit;
}
```

The production console input source must also set `Console.TreatControlCAsInput` to `true` while the
show is running, then restore its previous value. Otherwise, the terminal may raise `CancelKeyPress`
instead of returning `Ctrl+C` through `Console.ReadKey`.

## Adding a brand-new action

Suppose you want `Enter`-free auto-run: a `T` key that toggles a "theme" (illustrative). The full recipe:

### 1. Add the action

Extend [`ShortcutAction`](../src/PSSlideShow/Input/ShortcutAction.cs):

```csharp
public enum ShortcutAction
{
    None, Next, Previous, First, Last, ScrollUp, ScrollDown, Run, Reset, Quit,
    ToggleTheme            // new
}
```

### 2. Map a key to it

In [`KeyboardShortcuts.Resolve`](../src/PSSlideShow/Input/KeyboardShortcuts.cs):

```csharp
case ConsoleKey.T:
    return ShortcutAction.ToggleTheme;
```

### 3. Handle it in the run loop

Add a `case` to [`SlideShowHost.Dispatch`](../src/PSSlideShow/Hosting/SlideShowHost.cs). `Dispatch`
receives the `SlideShow` and the host's collaborators; mutate state here and the loop re-renders
automatically after `Dispatch` returns:

```csharp
case ShortcutAction.ToggleTheme:
    // e.g. flip a flag the renderer reads; then the next RenderFrame reflects it.
    break;
```

Do **not** render inside `Dispatch` — the loop calls `RenderFrame` immediately afterward. Keep the case
fast and side-effect-scoped to state the renderer reads.

### 4. Show a footer hint (optional)

If the key should be advertised, add it to
[`KeyboardShortcuts.FooterControls`](../src/PSSlideShow/Input/KeyboardShortcuts.cs). The method yields
hints in display order and can gate them on the current
[`SlideState`](../src/PSSlideShow/Input/SlideState.cs):

```csharp
yield return "[Q] Quit";
yield return "[T] Theme";              // always shown

if (state.Overflow)
    yield return "[↑↓] Scroll";
```

If your hint depends on new state (for example, "only when a slide has X"), add a field to
[`SlideState`](../src/PSSlideShow/Input/SlideState.cs) and populate it where the renderer builds the
state (in [`Renderer.Render`](../src/PSSlideShow/Rendering/Renderer.cs)).

## Test it

Because input is abstracted behind [`IInputSource`](../src/PSSlideShow/Input/IInputSource.cs), the whole
loop runs headless. Drive a scripted sequence of keys and assert on the `TestConsole` output — see the
host tests in [tests/PSSlideShow.Tests](../tests/PSSlideShow.Tests). You can also unit-test the mapping
directly:

```csharp
[Fact]
public void G_jumps_to_last_slide()
{
    var action = KeyboardShortcuts.Resolve(new ConsoleKeyInfo('g', ConsoleKey.G, false, false, false));
    Assert.Equal(ShortcutAction.Last, action);
}
```

## Validate

Run the full [build and test](../README.md#development) workflow, including the shortcut mapping and
host tests.
