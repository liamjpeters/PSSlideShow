using System.Management.Automation;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a table to a slide.</para>
    /// <para type="description">
    /// Rows may be hashtables or objects; columns are inferred from the first row unless
    /// <c>-Columns</c> is supplied.
    /// </para>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "SlideTable")]
    [OutputType(typeof(TableWidget))]
    public sealed class NewSlideTableCommand : WidgetCommand {
        /// <summary>Rows of data (arrays of PSObject/hashtables).</summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public object[] Rows { get; set; } = System.Array.Empty<object>();

        /// <summary>Explicit column headers/order (optional).</summary>
        [Parameter]
        public string[]? Columns { get; set; }

        /// <summary>Table border style. Default <c>Rounded</c>.</summary>
        [Parameter]
        public SlideBorder Border { get; set; } = SlideBorder.Rounded;

        /// <summary>Optional table caption.</summary>
        [Parameter]
        public string? Title { get; set; }

        private readonly System.Collections.Generic.List<object> _rows = new();

        protected override void ProcessRecord() {
            foreach (var row in Rows) {
                if (row != null) _rows.Add(row);
            }
        }

        protected override void EndProcessing() =>
            WriteWidget(new TableWidget(_rows, Columns, Border, Title));
    }
}
