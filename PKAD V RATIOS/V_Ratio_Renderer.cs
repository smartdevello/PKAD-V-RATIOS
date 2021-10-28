using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace PKAD_V_RATIOS
{
    public class V_Ratio_Renderer
    {
        private int width = 0, height = 0;
        private double totHeight = 1300;
        private Bitmap bmp = null;
        private Graphics gfx = null;

        private List<V_Ratio_Model> data = null;
        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));

        public V_Ratio_Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;

        }
        public int getDataCount()
        {
            if (this.data == null) return 0;
            else return this.data.Count;
        }
        public List<V_Ratio_Model> getData()
        {
            return this.data;
        }
        public void setData(List<V_Ratio_Model> data)
        {
            this.data = data;
        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)(a.X * px);
            res.Y = (int)((totHeight - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)(p.X * px);
            res.Y = (int)((totHeight - p.Y) * px);
            return res;
        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }

        public void drawFilledCircle(Brush brush, Point o, Size size)
        {
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);

            gfx.FillEllipse(brush, rect);
        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }

        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }

        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }
        public void drawCenteredImg_withBorder(Image img, Rectangle rect, Brush brush, Font font, Color borderColor)
        {
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            //gfx.DrawString(content, font, brush, rect, stringFormat);
            //drawImg(logoImg, new Point(20, 60), new Size(150, 50));
            gfx.DrawImage(img, rect);
            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }
        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        private void fillPolygon(Brush brush, PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = convertCoord(points[i]);
            }
            gfx.FillPolygon(brush, points);
        }
        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawString(Font font, Color brushColor, string content, Point o)
        {
            o = convertCoord(o);
            SolidBrush drawBrush = new SolidBrush(brushColor);
            gfx.DrawString(content, font, drawBrush, o.X, o.Y);
        }
        public void drawString(Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }

        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle)
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);
        }

        public void draw(int pageID = 1)
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }


            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }
            double px = height / totHeight;
            fillRectangle(Color.CornflowerBlue, new Rectangle(0, (int)totHeight, (int)(width / px), (int)totHeight));
            if (data == null) return;
            int recWidth = 400, recHeight = 400;
            int boxheaderHeight = 70, boxfooterWidth = 100, boxfooterHeight = 50, pieRadius = 120, pieInnerRadius = 80;
            Pen blackBorderPen2 = new Pen(Color.Black, 2);

            Font percentFont = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Point);
            Font headertitle = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            Font headerText = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont8 = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont7 = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont6 = new Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont5 = new Font("Arial", 5, FontStyle.Regular, GraphicsUnit.Point);
            
            int baseIndex = (pageID - 1) * 12;

            for (int row = 0; row <3; row ++)
                for (int col = 0; col < 4; col ++)
                {
                    int baseLeft = 20 + col * (recHeight + 10);
                    int baseTop = 1250 - row * (recWidth + 10);
                    int index = baseIndex + row * 4 + col;

                    if (index > data.Count - 1) break;

                    //Draw Box Headers
                    fillRectangle(Color.White, new Rectangle(baseLeft, baseTop, recWidth, recHeight));
                    drawRectangle(blackBorderPen2, new Rectangle(baseLeft, baseTop, boxheaderHeight, boxheaderHeight));
                    drawRectangle(blackBorderPen2, new Rectangle(baseLeft + boxheaderHeight, baseTop, recWidth - boxheaderHeight * 3, boxheaderHeight));
                    drawRectangle(blackBorderPen2, new Rectangle(baseLeft + recWidth - boxheaderHeight * 2 , baseTop, boxheaderHeight, boxheaderHeight));
                    drawRectangle(blackBorderPen2, new Rectangle(baseLeft + recWidth - boxheaderHeight * 1, baseTop, boxheaderHeight, boxheaderHeight));

                    fillRectangle(Color.Black, new Rectangle(baseLeft, baseTop, boxheaderHeight, 30));
                    drawCenteredString("P", new Rectangle(baseLeft, baseTop, boxheaderHeight, 30), Brushes.White, headertitle);
                    drawCenteredString(data[index].pallet.ToString(), new Rectangle(baseLeft, baseTop - 30, boxheaderHeight, boxheaderHeight - 30), Brushes.Black, headerText);


                    fillRectangle(Color.Black, new Rectangle(baseLeft + boxheaderHeight, baseTop - boxheaderHeight + 30, recWidth - boxheaderHeight * 3, 30));
                    drawCenteredString("BATCH", new Rectangle(baseLeft + boxheaderHeight, baseTop - boxheaderHeight + 30, recWidth - boxheaderHeight * 3, 30), Brushes.White, headertitle);
                    drawCenteredString(data[index].batch, new Rectangle(baseLeft + boxheaderHeight, baseTop, recWidth - boxheaderHeight * 3, boxheaderHeight - 30), Brushes.Black, headerText);


                    fillRectangle(Color.Black, new Rectangle(baseLeft + recWidth - 2 * boxheaderHeight, baseTop, boxheaderHeight, 30));
                    drawCenteredString("TB", new Rectangle(baseLeft + recWidth - 2 * boxheaderHeight, baseTop, boxheaderHeight, 30), Brushes.White, headertitle);
                    drawCenteredString(data[index].total_ballots.ToString(), new Rectangle(baseLeft + recWidth - 2 * boxheaderHeight, baseTop - 30, boxheaderHeight, boxheaderHeight - 30), Brushes.Black, headerText);


                    fillRectangle(Color.Black, new Rectangle(baseLeft + recWidth - boxheaderHeight, baseTop - boxheaderHeight + 30, boxheaderHeight, 30));
                    drawCenteredString("VB", new Rectangle(baseLeft + recWidth - boxheaderHeight, baseTop - boxheaderHeight + 30, boxheaderHeight, 30), Brushes.White, headertitle);
                    drawCenteredString(data[index].biden.ToString(), new Rectangle(baseLeft + recWidth - boxheaderHeight, baseTop , boxheaderHeight, boxheaderHeight - 30), Brushes.Black, headerText);

                    ///////////////////////////////////
                    ///s

                    ///////////////Draw Box Footers ////////
                    drawRectangle(blackBorderPen2, new Rectangle(baseLeft, baseTop - recHeight + boxfooterHeight, recWidth, boxfooterHeight));
                    fillRectangle(Color.Black, new Rectangle(baseLeft, baseTop - recHeight + boxfooterHeight, boxfooterWidth, boxfooterHeight));
                    drawCenteredString("BOX", new Rectangle(baseLeft, baseTop - recHeight + boxfooterHeight, boxfooterWidth, boxfooterHeight), Brushes.White, headertitle);

                    drawCenteredString(data[index].box, new Rectangle(baseLeft + boxfooterWidth, baseTop - recHeight + boxfooterHeight, recWidth - 2 * boxfooterWidth, boxfooterHeight), Brushes.Black, headerText);

                    fillRectangle(Color.Black, new Rectangle(baseLeft + recWidth - boxfooterWidth, baseTop - recHeight + boxfooterHeight, boxfooterWidth, boxfooterHeight));
                    drawCenteredString(data[index].jorgenson.ToString() + "/" + data[index].writeInOverUnder.ToString() , new Rectangle(baseLeft + recWidth - boxfooterWidth, baseTop - recHeight + boxfooterHeight, boxfooterWidth, boxfooterHeight), Brushes.White, headertitle);


                    //Draw PIE

                    Point leftop = new Point(baseLeft + recWidth / 2 - pieRadius, baseTop - recHeight / 2 + pieRadius);
                    Point innerLeftTop = new Point(baseLeft + recWidth / 2 - pieInnerRadius, baseTop - recHeight / 2 + pieInnerRadius);

                    float startAngle = 270, sweepAngle = 0;
                    sweepAngle = 360.0f * data[index].biden / (float)data[index].total_ballots;
                    drawPie(Color.CornflowerBlue, leftop, new Size(2 * pieRadius, 2 * pieRadius), startAngle, sweepAngle);

                    startAngle += sweepAngle;
                    sweepAngle = 360.0f - sweepAngle;
                    drawPie(Color.LightGray, leftop, new Size(2 * pieRadius, 2 * pieRadius), startAngle, sweepAngle);


                    drawFilledCircle(Brushes.White, innerLeftTop, new Size(2 * pieInnerRadius, 2 * pieInnerRadius));
                    int percent = data[index].biden * 100 / data[index].total_ballots;
                    drawCenteredString(percent.ToString() + "%", new Rectangle(innerLeftTop, new Size(2 * pieInnerRadius, 2 * pieInnerRadius)), Brushes.Black, percentFont);


                    //Draw Logo and Copyright
                    drawImg(logoImg, new Point(baseLeft, baseTop - recHeight + 100), new Size(100, 50));
                    string copyright = "©2021 Tesla Laboratories, llc & JHP";
                    drawCenteredString(copyright, new Rectangle(baseLeft + 150, baseTop - recHeight + 80, 250, 50), Brushes.Black, textFont6);
                }

            blackBorderPen2.Dispose();
            headertitle.Dispose();
            headerText.Dispose();
            textFont10.Dispose();
            textFont8.Dispose();
            textFont7.Dispose();
            textFont6.Dispose();
            textFont5.Dispose();
        }

    }
}
