using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Writer;

namespace PDFToolsDemo
{
    public class MyPDFSettings
    {
        public PdfAStandard PDFAStandard { get; }
        public PageSize PageSize { get; }
        public bool IsPortrait { get; }
        public PDFPadding Padding { get; }
        public double VerticalMargin { get; }

        /// <summary>
        /// Some basic settings for PDF building
        /// </summary>
        /// <param name="padding">Distances from edges to inward around the page</param>
        /// <param name="verticalMargin">Distance between element blocks</param>
        /// <param name="size">Page size</param>
        /// <param name="aStandard">PDF A Standard</param>
        /// <param name="isPortrait">Is page portrait</param>
        public MyPDFSettings(PDFPadding padding, double verticalMargin = 5, PageSize size = PageSize.A4,
            PdfAStandard aStandard = PdfAStandard.A2A, bool isPortrait = true)
        {
            Padding = padding;
            PageSize = size;
            IsPortrait = isPortrait;
            PDFAStandard = aStandard;
            VerticalMargin = verticalMargin;
        }
    }
}