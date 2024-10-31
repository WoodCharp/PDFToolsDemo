using PDFToolsDemo.Elements;
using UglyToad.PdfPig.Core;
using PDFToolsDemo;

namespace TestConsoleApp
{
    public static class Test
    {
        public static ElementBlock HeaderBlock(MyPDF p, double height, string imageFile, double imageWidth, double imageHeight)
        {
            PdfRectangle contentRect = p.ContentRect(p.PDFSettings.PageSize);
            ElementBlock b = new ElementBlock(contentRect.Width, height);

            LineElement line = new LineElement(b.Rect.BottomLeft, b.Rect.BottomRight, 0.2);
            b.Elements.Add(line);

            TextElement title = new TextElement("PDF TOOLS DEMO",
                new PdfRectangle(b.Rect.Left, b.Rect.Top - 18, b.Rect.Left + 100, b.Rect.Top), p.DefFont, 18);
            b.Elements.Add(title);

            TextElement subtitle = new TextElement("Example PDF",
                new PdfRectangle(b.Rect.Left, title.Rect.Bottom - 12, b.Rect.Left + 100, title.Rect.Bottom), p.DefFont, 12);
            b.Elements.Add(subtitle);

            TextElement dateText = new TextElement(DateTime.Now.Date.ToShortDateString(),
                new PdfRectangle(b.Rect.Right - 100, b.Rect.Top - 12, b.Rect.Right, b.Rect.Top), p.DefFont, 12)
            { HorizontalAlignment = EHorizontalAlignment.Right };
            b.Elements.Add(dateText);

            double imgX = b.Rect.Left + ((b.Rect.Width / 2) - (imageWidth / 2));
            double imgY = b.Rect.Bottom + ((b.Rect.Height / 2) - (imageHeight / 2));

            ImageElement image = new ImageElement(PDFTools.LoadFile(imageFile), ImageTypes.Png, new PdfRectangle(imgX, imgY, imgX + imageWidth, imgY + imageHeight));
            b.Elements.Add(image);

            return b;
        }

        public static ElementBlock FooterBlock(MyPDF p, double height)
        {
            PdfRectangle contentRect = p.ContentRect(p.PDFSettings.PageSize);
            ElementBlock b = new ElementBlock(contentRect.Width, height);

            LineElement line = new LineElement(b.Rect.TopLeft, b.Rect.TopRight, 0.2);
            b.Elements.Add(line);

            PdfRectangle r = new PdfRectangle(
                b.Rect.Left + (b.Rect.Width / 2) - 20, b.Rect.Bottom + ((b.Rect.Height / 2) - 6),
                b.Rect.Right - (b.Rect.Width / 2) + 20, b.Rect.Bottom + ((b.Rect.Height / 2) + 6));

            PageNumberElement number = new PageNumberElement("/", true, r, p.DefFont)
            { HorizontalAlignment = EHorizontalAlignment.Center };
            
            b.Elements.Add(number);

            return b;
        }
        

        public static ElementBlock TestTable(MyPDF p, string imageFile)
        {
            PdfRectangle contentRect = p.ContentRect(p.PDFSettings.PageSize);

            double rowHeight = 12, headerRowHeight = 20;
            double cellWidth = (contentRect.Width - rowHeight) / 4;
            double cellW0 = rowHeight;

            ElementBlock b = new ElementBlock(contentRect.Width, rowHeight);
            GridElement table = new GridElement();
            
            GridRowElement headerRow = new GridRowElement(headerRowHeight);
            headerRow.Cells.Add(new GridCellElement(new GridTextCell("", 16, p.DefFont), cellW0, headerRowHeight) { Border = HeaderBorders() });
            headerRow.Cells.Add(new GridCellElement(new GridTextCell("Product", 16, p.DefFont), cellWidth, headerRowHeight) { Border = HeaderBorders() });
            headerRow.Cells.Add(new GridCellElement(new GridTextCell("Price", 16, p.DefFont), cellWidth, headerRowHeight) { Border = HeaderBorders() });
            headerRow.Cells.Add(new GridCellElement(new GridTextCell("In stock", 16, p.DefFont), cellWidth, headerRowHeight) { Border = HeaderBorders() });
            headerRow.Cells.Add(new GridCellElement(new GridTextCell("Total value", 16, p.DefFont), cellWidth, headerRowHeight) { Border = HeaderBorders() });
            table.HeaderRow = headerRow;

            Random r = new Random();
            decimal totalValue = 0;

            byte[] image = PDFTools.LoadFile(imageFile);

            for (int i = 0; i < 20; i++)
            {
                string productTxt = $"Product {i}";
                decimal price = (decimal)r.NextDouble() * 10;
                price = decimal.Round(price, 2);

                decimal inStock = (decimal)r.NextDouble() * 10;
                inStock = decimal.Round(inStock, 2);

                string priceTxt = $"{price}€";
                string inStockTxt = $"{inStock}m";

                decimal value = price * inStock;

                string valueTxt = $"{value}€";

                totalValue += value;

                GridRowElement row = new GridRowElement(rowHeight);
                row.Cells.Add(new GridCellElement(new GridImageCell(image, ImageTypes.Png, 8, 8), cellW0, rowHeight) { Border = RowBorders() });
                row.Cells.Add(new GridCellElement(new GridTextCell($"Product {i}", 10, p.DefFont), cellWidth, rowHeight) { Border = RowBorders() });
                row.Cells.Add(new GridCellElement(new GridTextCell(priceTxt, 10, p.DefFont), cellWidth, rowHeight) { Border = RowBorders() });
                row.Cells.Add(new GridCellElement(new GridTextCell(inStockTxt, 10, p.DefFont), cellWidth, rowHeight) { Border = RowBorders() });
                row.Cells.Add(new GridCellElement(new GridTextCell(valueTxt, 10, p.DefFont), cellWidth, rowHeight) { Border = RowBorders() });
                table.Rows.Add(row);
            }

            totalValue = decimal.Round(totalValue, 2);

            GridRowElement row2 = new GridRowElement(rowHeight);
            row2.Cells.Add(new GridCellElement(new GridTextCell("", 10, p.DefFont), cellW0, rowHeight));
            row2.Cells.Add(new GridCellElement(new GridTextCell("", 10, p.DefFont), cellWidth, rowHeight));
            row2.Cells.Add(new GridCellElement(new GridTextCell("", 10, p.DefFont), cellWidth, rowHeight));
            row2.Cells.Add(new GridCellElement(new GridTextCell("Total value", 10, p.DefFont), cellWidth, rowHeight) { Border = TotalRowBorders() });
            row2.Cells.Add(new GridCellElement(new GridTextCell($"{totalValue}€", 10, p.DefFont), cellWidth, rowHeight) { Border = TotalRowBorders() });
            table.Rows.Add(row2);

            b.Elements.Add(table);
            return b;
        }

        private static ElementBorder HeaderBorders()
        {
            return new ElementBorder() { Lines = new List<ElementBorderLine>() {
            new ElementBorderLine(LineTypes.Solid, Borders.Bottom, 1)} };
        }

        private static ElementBorder RowBorders()
        {
            return new ElementBorder()
            {
                Lines = new List<ElementBorderLine>() {
            new ElementBorderLine(LineTypes.Dots, Borders.Bottom, 0.2)}
            };
        }

        private static ElementBorder TotalRowBorders()
        {
            return new ElementBorder()
            {
                Lines = new List<ElementBorderLine>() {
            new ElementBorderLine(LineTypes.Solid, Borders.Top, 1)}
            };
        }

        private static ElementBorder FullDashedBorders()
        {
            return new ElementBorder()
            {
                Lines = new List<ElementBorderLine>() {
                    new ElementBorderLine(LineTypes.Dashed, Borders.Full, 0.2)}
            };
        }

        public static ElementBlock TitleBlock(MyPDF p, string txt)
        {
            PdfRectangle contentRect = p.ContentRect(p.PDFSettings.PageSize);

            ElementBlock b = new ElementBlock(contentRect.Width, 20);
            b.Elements.Add(new TextElement(txt, new PdfRectangle(b.Rect.BottomLeft, b.Rect.TopRight), p.DefFont)
            {
                HorizontalAlignment = EHorizontalAlignment.Center, VerticalAlignment = EVerticalAlignment.Center
            });

            return b;
        }

        public static ElementBlock TitleBlock2(MyPDF p, string txt)
        {
            PdfRectangle contentRect = p.ContentRect(p.PDFSettings.PageSize);

            ElementBlock b = new ElementBlock(contentRect.Width, 20);
            b.Elements.Add(new TextElement(txt, new PdfRectangle(b.Rect.BottomLeft, b.Rect.TopRight), p.DefFont)
            {
                HorizontalAlignment = EHorizontalAlignment.Center,
                VerticalAlignment = EVerticalAlignment.Center,
                Border = FullDashedBorders()
            });

            return b;
        }
    }
}
