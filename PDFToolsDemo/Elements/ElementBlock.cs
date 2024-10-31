using UglyToad.PdfPig.Core;

namespace PDFToolsDemo.Elements
{
    public class ElementBlock
    {
        /// <summary>
        /// All elements in this block
        /// </summary>
        public List<IElement> Elements { get; set; }
        /// <summary>
        /// Element rectangle
        /// </summary>
        public PdfRectangle Rect { get; set; }

        /// <summary>
        /// A block containing all elements to be drawn.
        /// <see cref="Rect"/> is created from X0 Y0 point
        /// </summary>
        /// <param name="w">Block width</param>
        /// <param name="h">Block height</param>
        public ElementBlock(double w, double h)
        {
            Elements = new List<IElement>();
            Rect = new PdfRectangle(0, 0, w, h);
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