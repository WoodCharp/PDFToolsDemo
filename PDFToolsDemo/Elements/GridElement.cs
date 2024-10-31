using System.Drawing;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace PDFToolsDemo.Elements
{
    public class GridElement : IElement
    {
        public GridRowElement? HeaderRow { get; set; }
        public List<GridRowElement> Rows { get; set; }
        public EHorizontalAlignment HorizontalAlignment { get; set; } = EHorizontalAlignment.Center;
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
            set {  m_border = value; }
        }

        public GridElement()
        {
            Rows = new List<GridRowElement>();
        }
    }

    public class GridRowElement
    {
        /// <summary>
        /// Row cells
        /// </summary>
        public List<GridCellElement> Cells { get; set; }
        /// <summary>
        /// Row height
        /// </summary>
        public double RowHeight { get; set; }

        /// <summary>
        /// Row in grid element
        /// </summary>
        /// <param name="rowHeight">Row height</param>
        public GridRowElement(double rowHeight)
        {
            Cells = new List<GridCellElement>();
            RowHeight = rowHeight;
        }
    }

    public class GridCellElement
    {
        public GridCellContentBase Content { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        private ElementBorder? m_order;
        public ElementBorder Border
        {
            get
            {
                if (m_order == null)
                    m_order = new ElementBorder();

                return m_order;
            }
            set
            {
                m_order = value;
            }
        }

        /// <summary>
        /// Individual cell in the grid element row
        /// </summary>
        /// <param name="content">Content for the cell</param>
        /// <param name="w">Cell width</param>
        /// <param name="h">Cell height</param>
        public GridCellElement(GridCellContentBase content, double w, double h)
        {
            Content = content;
            Width = w;
            Height = h;
        }
    }

    public class GridCellContentBase
    {
        public EHorizontalAlignment HorizontalAlignment { get; set; } = EHorizontalAlignment.Center;
        public EVerticalAlignment VerticalAlignment { get; set; } = EVerticalAlignment.Center;
    }

    public class GridTextCell : GridCellContentBase
    {
        public string Text { get; set; }
        public double FontSize { get; set; }
        public PdfDocumentBuilder.AddedFont Font { get; set; }
        public Color Foreground { get; set; } = Color.Black;
        public PdfRectangle TextRect { get; private set; }

        /// <summary>
        /// Grid cell's text content type
        /// </summary>
        /// <param name="text">Content text</param>
        /// <param name="fontSize">Text font size</param>
        /// <param name="font">Font</param>
        public GridTextCell(string text, double fontSize, PdfDocumentBuilder.AddedFont font)
        {
            Text = text;
            FontSize = fontSize;
            Font = font;
        }

        /// <summary>
        /// Calculates text size and set's it to <see cref="TextRect"/>
        /// </summary>
        /// <param name="page"></param>
        public void CalculateTextRect(PdfPageBuilder page)
        {
            if (string.IsNullOrEmpty(Text))
            {
                TextRect = new PdfRectangle();
                return;
            }

            var letters = page.MeasureText(Text, FontSize, new PdfPoint(0, 0), Font);

            double h = 0;
            foreach (var letter in letters)
                if (letter.GlyphRectangle.Height > h) h = letter.GlyphRectangle.Height;

            TextRect = new PdfRectangle(0, 0, letters[letters.Count - 1].EndBaseLine.X, h);
        }
    }

    public class GridImageCell : GridCellContentBase
    {
        public byte[] Image { get; set; }
        public ImageTypes ImageType { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public GridImageCell(byte[] image, ImageTypes imageType, double width, double height)
        {
            Image = image;
            ImageType = imageType;
            Width = width;
            Height = height;
        }
    }
}
