using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;
using System.Drawing;

namespace PDFToolsDemo.Elements
{
    public class PageNumberElement : IElement
    {
        public EHorizontalAlignment HorizontalAlignment { get; set; } = EHorizontalAlignment.Left;
        public EVerticalAlignment VerticalAlignment { get; set; } = EVerticalAlignment.Center;
        public PdfRectangle Rect { get; private set; }
        private ElementBorder? m_border;
        public ElementBorder Border
        {
            get
            {
                if (m_border == null) m_border = new ElementBorder();
                return m_border;
            }
            set { m_border = value; }
        }

        public string Text { get; set; } = "";
        public string NumberSeparator { get; set; }
        public bool ShowTotalPagesNumber { get; set; }
        public Color TextColor { get; set; } = Color.Black;
        public double FontSize { get; }
        public PdfDocumentBuilder.AddedFont Font { get; }
        public PdfRectangle TextRect { get; private set; }

        /// <summary>
        /// Page number element
        /// </summary>
        /// <param name="numberSeparator">Separator for current and total page number</param>
        /// <param name="showTotalPagesNumber">Should total number of pages to be drawn</param>
        /// <param name="rect">Element size and location</param>
        /// <param name="font">Font of the text</param>
        /// <param name="fontSize">Size of the font</param>
        public PageNumberElement(string numberSeparator, bool showTotalPagesNumber, PdfRectangle rect, PdfDocumentBuilder.AddedFont font, double fontSize = 12)
        {
            NumberSeparator = numberSeparator;
            ShowTotalPagesNumber = showTotalPagesNumber;
            FontSize = fontSize;
            Font = font;
            Rect = rect;
        }

        /// <summary>
        /// Calculates text size and set's it to <see cref="TextRect"/>
        /// </summary>
        /// <param name="page"></param>
        public void CalculateTextRect(PdfPageBuilder page)
        {
            var letters = page.MeasureText(Text, FontSize, new PdfPoint(0, 0), Font);

            double h = 0;
            foreach (var letter in letters)
                if (letter.GlyphRectangle.Height > h) h = letter.GlyphRectangle.Height;

            TextRect = new PdfRectangle(0, 0, letters[letters.Count - 1].EndBaseLine.X, h);
        }

        /// <summary>
        /// Get correct location and size when moving the element to it's correct location.
        /// </summary>
        /// <param name="x">Moving along X</param>
        /// <param name="y">Moving along Y</param>
        /// <returns></returns>
        public PdfRectangle Move(double x, double y)
        {
            return new PdfRectangle(Rect.Left + x, Rect.Bottom + y, Rect.Right + x, Rect.Top + y);
        }
    }
}
