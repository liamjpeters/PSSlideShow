namespace PSSlideShow.Model {
    /// <summary>
    /// Border styles shared by widgets that draw boxes (tables, calendars, code panels).
    /// Maps to a concrete <see cref="Spectre.Console.TableBorder"/> via
    /// <see cref="PSSlideShow.Rendering.BorderResolver"/>.
    /// </summary>
    public enum SlideBorder {
        None,
        Ascii,
        Square,
        Rounded,
        Heavy,
        Double,
        Minimal
    }
}
