using Spectre.Console;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// A friendly, module-owned wrapper around Spectre's <see cref="Canvas"/> that is exposed to a
    /// <c>New-SlideCanvas -Paint</c> script block as <c>$_</c>/<c>$PSItem</c>. It hides Spectre types
    /// so paint scripts stay in PowerShell-friendly terms (color names, hex, or RGB bytes).
    /// </summary>
    public sealed class SlideCanvas {
        private readonly Canvas _canvas;

        public SlideCanvas(Canvas canvas) {
            _canvas = canvas;
        }

        /// <summary>Canvas width in pixels.</summary>
        public int Width => _canvas.Width;

        /// <summary>Canvas height in pixels.</summary>
        public int Height => _canvas.Height;

        /// <summary>Sets a pixel using a color name, hex string, or <see cref="System.ConsoleColor"/>.</summary>
        public void SetPixel(int x, int y, object color) {
            if (InBounds(x, y)) {
                _canvas.SetPixel(x, y, ColorResolver.Resolve(color));
            }
        }

        /// <summary>Sets a pixel using 24-bit RGB byte values.</summary>
        public void SetPixel(int x, int y, int r, int g, int b) {
            if (InBounds(x, y)) {
                _canvas.SetPixel(x, y, ColorResolver.FromRgb(r, g, b));
            }
        }

        /// <summary>Fills the entire canvas with a single color.</summary>
        public void Fill(object color) => Fill(color, 0, 0, Width, Height);

        /// <summary>Fills a rectangular region with a single color.</summary>
        public void Fill(object color, int x, int y, int width, int height) {
            var resolved = ColorResolver.Resolve(color);
            for (var yy = y; yy < y + height; yy++) {
                for (var xx = x; xx < x + width; xx++) {
                    if (InBounds(xx, yy)) {
                        _canvas.SetPixel(xx, yy, resolved);
                    }
                }
            }
        }

        internal Canvas Inner => _canvas;

        private bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
    }
}
