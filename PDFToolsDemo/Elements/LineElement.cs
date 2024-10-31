using System.Drawing;
using UglyToad.PdfPig.Core;

namespace PDFToolsDemo.Elements
{
    public class LineElement : IElement
    {
        public EHorizontalAlignment HorizontalAlignment { get; set; }
        public EVerticalAlignment VerticalAlignment { get; set; }
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

        public double LineWidth { get; }
        public Color Color { get; set; } = Color.Black;
        public PdfPoint From { get; private set; }
        public PdfPoint To { get; private set; }

        /// <summary>
        /// Simple line element
        /// </summary>
        /// <param name="start">Line start point</param>
        /// <param name="end">Line end point</param>
        /// <param name="lineWidth">Line width</param>
        public LineElement(PdfPoint start, PdfPoint end, double lineWidth = 1)
        {
            From = start;
            To = end;
            LineWidth = lineWidth;

            Rect = PDFTools.CalculateTwoPointBounds(start, end);
        }

        /// <summary>
        /// Get correct point when moving the element to it's correct location.
        /// </summary>
        /// <param name="x">Moving along X</param>
        /// <param name="y">Moving along Y</param>
        /// <returns></returns>
        public PdfPoint MoveFrom(double x, double y)
        {
            return new PdfPoint(From.X + x, From.Y + y);
        }

        /// <summary>
        /// Get correct point when moving the element to it's correct location.
        /// </summary>
        /// <param name="x">Moving along X</param>
        /// <param name="y">Moving along Y</param>
        /// <returns></returns>
        public PdfPoint MoveTo(double x, double y)
        {
            return new PdfPoint(To.X + x, To.Y + y);
        }

        /// <summary>
        /// Get bound of the two points when moving the element to it's correct location.
        /// </summary>
        /// <param name="x">Moving along X</param>
        /// <param name="y">Moving along Y</param>
        /// <returns></returns>
        public PdfRectangle Move(double x, double y)
        {
            return new PdfRectangle(Rect.Left + x, Rect.Bottom + y, Rect.Left + x, Rect.Top + y);
        }
    }
}
