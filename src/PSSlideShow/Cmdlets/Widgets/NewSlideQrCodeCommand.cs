using System;
using System.Management.Automation;
using QRCoder;
using PSSlideShow.Authoring;
using PSSlideShow.Model;

namespace PSSlideShow.Cmdlets.Widgets {
    /// <summary>
    /// <para type="synopsis">Adds a phone-scannable QR code to a slide.</para>
    /// <para type="description">
    /// Encodes text or a URL as a black-on-white QR matrix with its quiet zone. The code uses
    /// whole terminal half-blocks instead of scaling image pixels; it must be the only widget
    /// on its slide. If the complete code cannot fit, the slide shows a resize message instead.
    /// </para>
    /// </summary>
    /// <example>
    ///   <code>New-Slide 'Scan me' { New-SlideQrCode 'https://example.com' }</code>
    /// </example>
    [Cmdlet(VerbsCommon.New, "SlideQrCode")]
    [OutputType(typeof(QrCodeWidget))]
    public sealed class NewSlideQrCodeCommand : WidgetCommand {
        /// <summary>Text or URL to encode. Must not be empty or whitespace.</summary>
        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty]
        public string Text { get; set; } = string.Empty;

        public NewSlideQrCodeCommand() {
            VPadding = 0;
        }

        protected override void EndProcessing() {
            if (string.IsNullOrWhiteSpace(Text)) {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException("QR code text must not be empty or whitespace.", nameof(Text)),
                    "InvalidQrCodeText",
                    ErrorCategory.InvalidArgument,
                    Text));
                return;
            }

            if (SlideBuildContext.Current is string scope && scope != "Slide") {
                ThrowTerminatingError(new ErrorRecord(
                    new PSArgumentException("A QR code must be a top-level widget on its own slide."),
                    "QrCodeCannotBeNested",
                    ErrorCategory.InvalidOperation,
                    Text));
                return;
            }

            try {
                using var qr = QRCodeGenerator.GenerateQrCode(Text, QRCodeGenerator.ECCLevel.M);
                var rows = qr.ModuleMatrix;
                var size = rows.Count;
                var modules = new bool[size, size];
                for (var y = 0; y < size; y++) {
                    for (var x = 0; x < size; x++) {
                        modules[y, x] = rows[y][x];
                    }
                }

                WriteWidget(new QrCodeWidget(modules));
            }
            catch (QRCoder.Exceptions.DataTooLongException ex) {
                ThrowTerminatingError(new ErrorRecord(
                    ex,
                    "QrCodeDataTooLong",
                    ErrorCategory.InvalidArgument,
                    Text));
            }
        }
    }
}
