using UglyToad.PdfPig.Core;

namespace PDFToolsDemo.Elements
{
    public interface IElement
    {
        public EHorizontalAlignment HorizontalAlignment { get; set; }
        public EVerticalAlignment VerticalAlignment { get; set; }
        public PdfRectangle Rect { get; }
        public ElementBorder Border { get; }
    }
}