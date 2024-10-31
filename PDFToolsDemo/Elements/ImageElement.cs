using UglyToad.PdfPig.Core;

namespace PDFToolsDemo.Elements
{
    public class ImageElement : IElement
    {
        public EHorizontalAlignment HorizontalAlignment { get; set; } = EHorizontalAlignment.Center;
        public EVerticalAlignment VerticalAlignment { get; set; } = EVerticalAlignment.Center;
        public PdfRectangle Rect{ get; private set; }
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

        public byte[] Image { get; }
        public ImageTypes ImageFileType { get; }

        /// <summary>
        /// Element for image
        /// </summary>
        /// <param name="image">Image as byte array</param>
        /// <param name="imageFileType">File type</param>
        /// <param name="rect"></param>
        public ImageElement(byte[] image, ImageTypes imageFileType, PdfRectangle rect)
        {
            Image = image;
            ImageFileType = imageFileType;
            Rect = rect;
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

    public enum ImageTypes { Jpeg, Png }
}