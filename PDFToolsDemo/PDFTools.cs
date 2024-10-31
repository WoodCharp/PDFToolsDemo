using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace PDFToolsDemo
{
    public static class PDFTools
    {
        public static byte[] LoadFile(string file)
        {
            return File.ReadAllBytes(file);
        }

        /// <summary>
        /// Get paper size.
        /// Sizes are from PdfPig's <see cref="PageSizeExtensions"/>. Executive size has been left out
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static PdfPoint GetPageSize(PageSize size)
        {
            switch (size)
            {
                case PageSize.A0:
                    return new PdfPoint(2384, 3370);
                case PageSize.A1:
                    return new PdfPoint(1684, 2384);
                case PageSize.A2:
                    return new PdfPoint(1191, 1684);
                case PageSize.A3:
                    return new PdfPoint(842, 1191);
                case PageSize.A4:
                    return new PdfPoint(595, 842);
                case PageSize.A5:
                    return new PdfPoint(420, 595);
                case PageSize.A6:
                    return new PdfPoint(298, 420);
                case PageSize.A7:
                    return new PdfPoint(210, 298);
                case PageSize.A8:
                    return new PdfPoint(147, 210);
                case PageSize.A9:
                    return new PdfPoint(105, 147);
                case PageSize.A10:
                    return new PdfPoint(74, 105);
                case PageSize.Letter:
                    return new PdfPoint(612, 792);
                case PageSize.Legal:
                    return new PdfPoint(612, 1008);
                case PageSize.Ledger:
                    return new PdfPoint(1224, 792);
                case PageSize.Tabloid:
                    return new PdfPoint(792, 1224);
                default:
                    return new PdfPoint(595, 842);
            }
        }

        public static PdfRectangle CalculateTwoPointBounds(PdfPoint p1, PdfPoint p2)
        {
            double minX = Math.Min(p1.X, p2.X);
            double minY = Math.Min(p1.Y, p2.Y);
            double maxX = Math.Max(p1.X, p2.X);
            double maxY = Math.Max(p1.Y, p2.Y);

            PdfPoint bottomLeft = new PdfPoint(minX, minY);
            PdfPoint topRight = new PdfPoint(maxX, maxY);

            return new PdfRectangle(bottomLeft, topRight);
        }

    }
}