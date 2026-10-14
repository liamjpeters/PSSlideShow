using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Spectre.Console;
using Spectre.Console.Rendering;
using PSSlideShow.Rendering;

namespace PSSlideShow.Model {
    /// <summary>
    /// Tabular data. Rows may be hashtables or objects; columns are taken from <see cref="Columns"/>
    /// when supplied, otherwise inferred from the first row's keys/properties.
    /// </summary>
    public sealed class TableWidget : SlideWidget {
        public TableWidget(
            IReadOnlyList<object> rows,
            IReadOnlyList<string>? columns = null,
            SlideBorder border = SlideBorder.Rounded,
            string? title = null) {
            Rows = rows ?? new List<object>();
            Columns = columns;
            Border = border;
            Title = title;
        }

        public IReadOnlyList<object> Rows { get; }
        public IReadOnlyList<string>? Columns { get; }
        public SlideBorder Border { get; }
        public string? Title { get; }
        public override bool IsRunnable => false;

        public override IRenderable Render(WidgetRenderContext context) {
            var columns = ResolveColumns();
            var table = new Table { Border = BorderResolver.Resolve(Border) };
            if (!string.IsNullOrEmpty(Title)) {
                table.Title = new TableTitle(Title!);
            }

            foreach (var column in columns) {
                table.AddColumn(new TableColumn(Markup.Escape(column)));
            }

            foreach (var row in Rows) {
                var cells = columns
                    .Select(c => (IRenderable)new Markup(Markup.Escape(FormatValue(GetValue(row, c)))))
                    .ToArray();
                table.AddRow(cells);
            }

            return table;
        }

        private IReadOnlyList<string> ResolveColumns() {
            if (Columns != null && Columns.Count > 0) return Columns;
            if (Rows.Count == 0) return new List<string>();
            return InferColumns(Rows[0]);
        }

        private static IReadOnlyList<string> InferColumns(object row) {
            if (row is IDictionary dictionary) {
                return dictionary.Keys.Cast<object>().Select(k => k?.ToString() ?? string.Empty).ToList();
            }

            var pso = PSObject.AsPSObject(row);
            return pso.Properties
                .Where(p => p.IsGettable)
                .Select(p => p.Name)
                .ToList();
        }

        private static object? GetValue(object row, string column) {
            if (row is IDictionary dictionary) {
                return dictionary.Contains(column) ? dictionary[column] : null;
            }

            var pso = PSObject.AsPSObject(row);
            return pso.Properties[column]?.Value;
        }

        private static string FormatValue(object? value) {
            if (value == null) return string.Empty;
            if (value is PSObject pso) return pso.BaseObject?.ToString() ?? string.Empty;
            return value.ToString() ?? string.Empty;
        }
    }
}
