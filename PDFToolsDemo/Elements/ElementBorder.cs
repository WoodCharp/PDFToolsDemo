using System.Drawing;

namespace PDFToolsDemo.Elements
{
    public class ElementBorder
    {
        public List<ElementBorderLine> Lines { get; set; }

        /// <summary>
        /// Contains info about element border
        /// </summary>
        public ElementBorder()
        {
            Lines = new List<ElementBorderLine>();
        }
    }

    public class ElementBorderLine
    {
        public double LineWidth { get; set; }
        public Color Color { get; set; }
        public LineTypes Type { get; set; }
        public Borders Border { get; set; }

        /// <summary>
        /// Contains information about single border line
        /// </summary>
        /// <param name="type">Border line type</param>
        /// <param name="border">What border</param>
        /// <param name="lineWidth">Border line width</param>
        public ElementBorderLine(LineTypes type = LineTypes.Solid, Borders border = Borders.Full, double lineWidth = 1)
        {
            Type = type;
            LineWidth = lineWidth;
            Border = border;
        }
    }

    public enum Borders { Left, Right, Top, Bottom, Full }
    public enum LineTypes { Solid, Dashed, Dots }
}