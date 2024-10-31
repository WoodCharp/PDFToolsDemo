using PDFToolsDemo.Elements;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace PDFToolsDemo
{
    public class MyPDF
    {
        public ElementBlock? HeaderBlock { get; set; }
        public ElementBlock? FooterBlock { get; set; }
        public List<ElementBlock> Blocks { get; set; }

        public readonly MyPDFSettings PDFSettings;
        private readonly PdfDocumentBuilder m_builder;
        private PdfPageBuilder m_page;
        public readonly PdfDocumentBuilder.AddedFont DefFont;

        private double m_drawY;

        #region Debug

        private bool m_debug = true;

        private void DebugSetColors(PdfPageBuilder page)
        {
            page.SetStrokeColor(255, 0, 0);
        }

        private void DebugDrawRect(PdfRectangle rect, PdfPageBuilder page)
        {
            DebugSetColors(page);
            page.DrawRectangle(rect.BottomLeft, rect.Width, rect.Height, 0.2);
        }

        private void DebugDrawPagePaddings()
        {
            foreach(var page in m_builder.Pages.Values)
            {
                DebugDrawRect(ContentRect(page), page);
            }
        }

        #endregion

        private PdfRectangle ContentRect (PdfPageBuilder page)
        {
            return new PdfRectangle(PDFSettings.Padding.Left, PDFSettings.Padding.Bottom,
                page.PageSize.Width - PDFSettings.Padding.Right, page.PageSize.Height - PDFSettings.Padding.Top);
        }

        public PdfRectangle ContentRect(PageSize size)
        {
            PdfPoint pageSize = PDFTools.GetPageSize(size);
            return new PdfRectangle(PDFSettings.Padding.Left, PDFSettings.Padding.Bottom,
                pageSize.X - PDFSettings.Padding.Right, pageSize.Y - PDFSettings.Padding.Top);
        }


        public MyPDF(MyPDFSettings settings, byte[] defTTF)
        {
            PDFSettings = settings;
            Blocks = new List<ElementBlock>();
            m_builder = new PdfDocumentBuilder() { ArchiveStandard = settings.PDFAStandard };
            DefFont = m_builder.AddTrueTypeFont(defTTF);

            m_page = m_builder.AddPage(PDFSettings.PageSize, PDFSettings.IsPortrait);
        }

        public void SaveDocument(string file)
        {
            File.WriteAllBytes(file, m_builder.Build());
        }


        /// <summary>
        /// Resets draw point to top of the page.
        /// </summary>
        private void ResetDrawY()
        {
            PdfRectangle contentRect = ContentRect(m_page);
            double y = contentRect.Top;
            if (HeaderBlock != null) y -= HeaderBlock.Rect.Height;
            m_drawY = y;
        }

        /// <summary>
        /// Check available in current page. Creates new page if there is no more room. Also calls <see cref="ResetDrawY()"/>
        /// if new page is created
        /// </summary>
        /// <param name="nextDrawHeight"></param>
        /// <returns></returns>
        private bool CheckAvailablePageSpace(double nextDrawHeight = 0)
        {
            PdfRectangle contentRect = ContentRect(m_page);

            double stopY = PDFSettings.Padding.Bottom;
            if (FooterBlock != null) stopY += FooterBlock.Rect.Height;
            stopY += nextDrawHeight;

            if(m_drawY < stopY)
            {
                m_page = m_builder.AddPage(PDFSettings.PageSize, PDFSettings.IsPortrait);
                ResetDrawY();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Build the PDF document
        /// </summary>
        public void BuildDocument()
        {
            //Get page content area and reset draw point
            PdfRectangle rect = ContentRect(m_page);
            ResetDrawY();
            
            double moveX = PDFSettings.Padding.Left;

            foreach(var b in Blocks)
            {
                CheckAvailablePageSpace(b.Rect.Height);

                foreach(var e in b.Elements)
                    DrawElements(e, null, moveX, m_drawY - b.Rect.Height);

                m_drawY -= b.Rect.Height;
            }

            DrawHeaderBlocks();
            DrawFooterBlocks();

            if (m_debug) DebugDrawPagePaddings();
        }

        private void DrawHeaderBlocks()
        {
            if (HeaderBlock == null) return;

            double moveX = PDFSettings.Padding.Left;
            double moveY = ContentRect(m_page).Top - HeaderBlock.Rect.Height;

            foreach(var page in m_builder.Pages.Values)
            {
                if (m_debug) DebugDrawRect(HeaderBlock.Move(moveX, moveY), page);

                foreach(var e in HeaderBlock.Elements)
                    DrawElements(e, page, moveX, moveY);
            }
        }

        private void DrawFooterBlocks()
        {
            if (FooterBlock == null) return;

            double moveX = PDFSettings.Padding.Left;
            double moveY = ContentRect(m_page).Bottom;

            foreach (var page in m_builder.Pages.Values)
            {
                if (m_debug) DebugDrawRect(FooterBlock.Move(moveX, moveY), page);

                foreach (var e in FooterBlock.Elements)
                    DrawElements(e, page, moveX, moveY);
            }
        }


        private void DrawElements(IElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            if (e.GetType() == typeof(LineElement)) DrawLineElement((LineElement)e, p, moveX, moveY);
            else if (e.GetType() == typeof(TextElement)) DrawTextElement((TextElement)e, p, moveX, moveY);
            else if (e.GetType() == typeof(ImageElement)) DrawImageElement((ImageElement)e, p, moveX, moveY);
            else if (e.GetType() == typeof(PageNumberElement)) DrawPageNumberElement((PageNumberElement)e, p, moveX, moveY);
            else if (e.GetType() == typeof(GridElement)) DrawGridElement((GridElement)e, p, moveX, moveY);

        }

        private void DrawLineElement(LineElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            PdfPageBuilder page = p != null ? p : m_page;

            if (m_debug) DebugDrawRect(e.Move(moveX, moveY), page);

            page.SetStrokeColor(e.Color.R, e.Color.G, e.Color.B);
            page.DrawLine(e.MoveFrom(moveX, moveY), e.MoveTo(moveX, moveY), e.LineWidth);
        }

        private void DrawTextElement(TextElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            PdfPageBuilder page = p != null ? p : m_page;
            PdfRectangle tRect = e.Move(moveX, moveY);

            if (m_debug) DebugDrawRect(tRect, page);

            e.CalculateTextRect(page);

            double x = tRect.Left, y = tRect.Bottom;

            if (e.HorizontalAlignment == EHorizontalAlignment.Right)
                x = tRect.Right - e.TextRect.Width;
            else if (e.HorizontalAlignment == EHorizontalAlignment.Center)
                x = tRect.Left + ((tRect.Width / 2) - (e.TextRect.Width / 2));

            if (e.VerticalAlignment == EVerticalAlignment.Top)
                y = tRect.Top - e.TextRect.Height;
            else if (e.VerticalAlignment == EVerticalAlignment.Center)
                y = tRect.Bottom + ((tRect.Height / 2) - (e.TextRect.Height / 2));

            page.SetTextAndFillColor(e.TextColor.R, e.TextColor.G, e.TextColor.B);
            page.AddText(e.Text, e.FontSize, new PdfPoint(x, y), e.Font);

            foreach (var l in e.Border.Lines)
                DrawBorderLine(l, tRect, page);
        }
        
        private void DrawImageElement(ImageElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            PdfPageBuilder page = p != null ? p : m_page;
            PdfRectangle tRect = e.Move(moveX, moveY);

            if (m_debug) DebugDrawRect(tRect, page);

            if (e.ImageFileType == ImageTypes.Jpeg)
                page.AddJpeg(e.Image, tRect);
            else
                page.AddPng(e.Image, tRect);

            foreach (var l in e.Border.Lines)
                DrawBorderLine(l, tRect, page);
        }

        private void DrawPageNumberElement(PageNumberElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            PdfPageBuilder page = p != null ? p : m_page;
            PdfRectangle tRect = e.Move(moveX, moveY);

            if (m_debug) DebugDrawRect(tRect, page);

            string text = page.PageNumber.ToString();
            if (e.ShowTotalPagesNumber)
                text = $"{page.PageNumber}{e.NumberSeparator}{m_builder.Pages.Count}";
            e.Text = text;

            e.CalculateTextRect(page);

            double x = tRect.Left, y = tRect.Bottom;

            if (e.HorizontalAlignment == EHorizontalAlignment.Right)
                x = tRect.Right - e.TextRect.Width;
            else if (e.HorizontalAlignment == EHorizontalAlignment.Center)
                x = tRect.Left + ((tRect.Width / 2) - (e.TextRect.Width / 2));

            if (e.VerticalAlignment == EVerticalAlignment.Top)
                y = tRect.Top - e.TextRect.Height;
            else if (e.VerticalAlignment == EVerticalAlignment.Center)
                y = tRect.Bottom + ((tRect.Height / 2) - (e.TextRect.Height / 2));

            page.SetTextAndFillColor(e.TextColor.R, e.TextColor.G, e.TextColor.B);
            page.AddText(e.Text, e.FontSize, new PdfPoint(x, y), e.Font);

            foreach (var l in e.Border.Lines)
                DrawBorderLine(l, tRect, page);
        }


        private void DrawBorderLine(ElementBorderLine l, PdfRectangle rect, PdfPageBuilder p)
        {
            p.SetStrokeColor(l.Color.R, l.Color.G, l.Color.B);

            if (l.Type == LineTypes.Solid)
            {
                if(l.Border == Borders.Bottom)
                    p.DrawLine(rect.BottomLeft, rect.BottomRight, l.LineWidth);
                else if (l.Border == Borders.Top)
                    p.DrawLine(rect.TopLeft, rect.TopRight, l.LineWidth);
                else if(l.Border == Borders.Right)
                    p.DrawLine(rect.BottomRight, rect.TopRight, l.LineWidth);
                else if(l.Border == Borders.Left)
                    p.DrawLine(rect.BottomLeft, rect.TopLeft, l.LineWidth);
                else if(l.Border == Borders.Full)
                    p.DrawRectangle(rect.BottomLeft, rect.Width, rect.Height, l.LineWidth);
            }
            else if(l.Type == LineTypes.Dashed ||
                l.Type == LineTypes.Dots)
            {
                double dashLenght = l.Type == LineTypes.Dashed ? 4 : 1;

                if(l.Border == Borders.Bottom)
                {
                    double dashStartX = rect.Left;

                    while(dashStartX <= rect.Right - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(dashStartX, rect.Bottom),
                            new PdfPoint(dashStartX + dashLenght, rect.Bottom), l.LineWidth);

                        dashStartX += dashLenght * 2;
                    }
                }
                else if (l.Border == Borders.Top)
                {
                    double dashStartX = rect.Left;

                    while (dashStartX <= rect.Right - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(dashStartX, rect.Top),
                            new PdfPoint(dashStartX + dashLenght, rect.Top), l.LineWidth);

                        dashStartX += dashLenght * 2;
                    }
                }
                else if (l.Border == Borders.Right)
                {
                    double dashStartY = rect.Bottom;

                    while (dashStartY <= rect.Top - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(rect.Right, dashStartY),
                            new PdfPoint(rect.Right, dashStartY + dashLenght), l.LineWidth);

                        dashStartY += dashLenght * 2;
                    }
                }
                else if (l.Border == Borders.Left)
                {
                    double dashStartY = rect.Bottom;

                    while (dashStartY <= rect.Top - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(rect.Left, dashStartY),
                            new PdfPoint(rect.Left, dashStartY + dashLenght), l.LineWidth);

                        dashStartY += dashLenght * 2;
                    }
                }
                else if(l.Border == Borders.Full)
                {
                    double dashStartX = rect.Left;

                    while (dashStartX <= rect.Right - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(dashStartX, rect.Bottom),
                            new PdfPoint(dashStartX + dashLenght, rect.Bottom), l.LineWidth);
                        p.DrawLine(new PdfPoint(dashStartX, rect.Top),
                            new PdfPoint(dashStartX + dashLenght, rect.Top), l.LineWidth);

                        dashStartX += dashLenght * 2;
                    }

                    double dashStartY = rect.Bottom;

                    while (dashStartY <= rect.Top - dashLenght)
                    {
                        p.DrawLine(new PdfPoint(rect.Left, dashStartY),
                            new PdfPoint(rect.Left, dashStartY + dashLenght), l.LineWidth);
                        p.DrawLine(new PdfPoint(rect.Right, dashStartY),
                            new PdfPoint(rect.Right, dashStartY + dashLenght), l.LineWidth);

                        dashStartY += dashLenght * 2;
                    }
                }
            }
        }


        private void DrawGridElement(GridElement e, PdfPageBuilder? p = null, double moveX = 0, double moveY = 0)
        {
            PdfPageBuilder page = p != null ? p : m_page;

            double x = moveX;

            if (e.HeaderRow != null)
            {
                m_drawY -= e.HeaderRow.RowHeight + PDFSettings.VerticalMargin;

                foreach (var c in e.HeaderRow.Cells)
                {
                    PdfRectangle rect = new PdfRectangle(x, m_drawY, x + c.Width, m_drawY + c.Height);
                    if (m_debug) DebugDrawRect(rect, page);

                    if (c.Content.GetType() == typeof(GridTextCell))
                    {
                        DrawGridCellText((GridTextCell)c.Content, rect, page);
                    }

                    foreach (var l in c.Border.Lines)
                        DrawBorderLine(l, rect, page);

                    x += c.Width;
                }
            }

            foreach (var row in e.Rows)
            {
                x = PDFSettings.Padding.Left;
                m_drawY -= row.RowHeight;

                foreach (var c in row.Cells)
                {
                    PdfRectangle rect = new PdfRectangle(x, m_drawY, x + c.Width, m_drawY + c.Height);
                    if (m_debug) DebugDrawRect(rect, page);

                    if (c.Content.GetType() == typeof(GridTextCell))
                    {
                        DrawGridCellText((GridTextCell)c.Content, rect, page);
                    }
                    else if (c.Content.GetType() == typeof(GridImageCell))
                    {
                        DrawGridCellImage((GridImageCell)c.Content, rect, page);
                    }

                    foreach (var l in c.Border.Lines)
                        DrawBorderLine(l, rect, page);

                    x += c.Width;
                }

                if (CheckAvailablePageSpace(PDFSettings.VerticalMargin + row.RowHeight))
                {
                    page = m_page;
                    m_drawY -= PDFSettings.VerticalMargin;
                }
            }
        }
    
        private void DrawGridCellText(GridTextCell textCell, PdfRectangle rect, PdfPageBuilder page)
        {
            if (string.IsNullOrEmpty(textCell.Text)) return;

            textCell.CalculateTextRect(page);

            page.SetTextAndFillColor(textCell.Foreground.R, textCell.Foreground.G, textCell.Foreground.B);

            double x = rect.Left, y = rect.Bottom;

            if (textCell.HorizontalAlignment == EHorizontalAlignment.Right)
                x = rect.Right - textCell.TextRect.Width;
            else if (textCell.HorizontalAlignment == EHorizontalAlignment.Center)
                x = rect.Left + ((rect.Width / 2) - (textCell.TextRect.Width / 2));

            if (textCell.VerticalAlignment == EVerticalAlignment.Top)
                y = rect.Top - textCell.TextRect.Height;
            else if (textCell.VerticalAlignment == EVerticalAlignment.Center)
                y = rect.Bottom + ((rect.Height / 2) - (textCell.TextRect.Height / 2));

            page.AddText(textCell.Text, textCell.FontSize, new PdfPoint(x, y), textCell.Font);
        }
    
        private void DrawGridCellImage(GridImageCell imageCell, PdfRectangle rect, PdfPageBuilder page)
        {
            double x = rect.Left, y = rect.Bottom;

            if (imageCell.HorizontalAlignment == EHorizontalAlignment.Right)
                x = rect.Right - imageCell.Width;
            else if (imageCell.HorizontalAlignment == EHorizontalAlignment.Center)
                x = rect.Left + ((rect.Width / 2) - (imageCell.Width / 2));

            if (imageCell.VerticalAlignment == EVerticalAlignment.Top)
                y = rect.Top - imageCell.Height;
            else if (imageCell.VerticalAlignment == EVerticalAlignment.Center)
                y = rect.Bottom + ((rect.Height / 2) - (imageCell.Height / 2));

            PdfRectangle imgRect = new PdfRectangle(x, y, x + imageCell.Width, y + imageCell.Height);

            if (imageCell.ImageType == ImageTypes.Jpeg)
                page.AddJpeg(imageCell.Image, imgRect);
            else
                page.AddPng(imageCell.Image, imgRect);
        }
    }
}