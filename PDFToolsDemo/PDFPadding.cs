namespace PDFToolsDemo
{
    public struct PDFPadding
    {
        public double Left { get; }
        public double Right { get; }
        public double Top { get; }
        public double Bottom { get; }

        public PDFPadding(double left, double right, double top, double bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }
}