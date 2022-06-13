using System.Drawing.Drawing2D;
using MinigameIdle.Engine.Mathematics;

namespace MinigameIdle.Engine
{
    public class GraphicsManager
    {
        public Game Game { get; init; }
        public Platform Platform { get; init; }

        private readonly SolidBrush brush;
        private Pen pen;
        private Graphics formGraphics;

        private Bitmap buffer = null!;
        private Graphics bitmapGraphics = null!;

        internal GraphicsManager(Game game, Platform platform)
        {
            Game = game;
            Platform = platform;

            brush = new(Color.Red);
            pen = new(brush);

            formGraphics = platform.Form.CreateGraphics();
        }

        public Color BackColor
        {
            get => Platform.Form.BackColor;
            set => Platform.Form.BackColor = value;
        }

        public void DrawRectangle(Vector2 pos, Vector2 size, Color color, bool fill = true, float angle = 0, double xAnchor = 0, double yAnchor = 0)
            => DrawRectangle(bitmapGraphics, (int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, color, fill, angle, xAnchor, yAnchor);

        public void DrawRectangle(float x, float y, float w, float h, Color color, bool fill = true, float angle = 0, double xAnchor = 0, double yAnchor = 0)
            => DrawRectangle(new(x, y), new(w, h), color, fill, angle, xAnchor, yAnchor);

        public void DrawRectangle(RectangleF rect, Color color, bool fill = true, float angle = 0, double xAnchor = 0, double yAnchor = 0)
            => DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color, fill, angle, xAnchor, yAnchor);

        public void DrawLine(Vector2 v1, Vector2 v2, Color color)
            => DrawLine(bitmapGraphics, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, color);

        public void DrawLine(float x1, float y1, float x2, float y2, Color color)
            => DrawLine(new(x1, y1), new(x2, y2), color);

        public void DrawEllipse(Vector2 pos, Vector2 size, Color color, bool fill = true, float r = 0, double xAnchor = 0f, double yAnchor = 0f)
            => DrawEllipse(bitmapGraphics, (int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, color, fill, r, xAnchor, yAnchor);

        public void DrawEllipse(float x, float y, float w, float h, Color color, bool fill = true, float r = 0, double xAnchor = 0f, double yAnchor = 0f)
            => DrawEllipse(new(x, y), new(w, h), color, fill, r, xAnchor, yAnchor);

        public void DrawCircle(Vector2 pos, float r, Color color, bool fill = true)
        {
            Vector2 size = new(r * 2, r * 2);
            pos = new(pos.X - r, pos.Y - r);

            DrawEllipse(pos, size, color, fill, r, 0.5f, 0.5f);
        }

        public void DrawCircle(float x, float y, float r, Color color, bool fill = true)
            => DrawCircle(new(x, y), r, color, fill);

        public void DrawBMP(Bitmap bmp, float x, float y, float w, float h, float r = 0)
            => DrawBMP(bitmapGraphics, bmp, (int)x, (int)y, (int)w, (int)h, r, 0, 0, bmp.Width, bmp.Height);

        public void DrawBMP(Bitmap bmp, Vector2 position, Vector2 size, float r = 0)
            => DrawBMP(bmp, position.X, position.Y, size.X, size.Y, r);

        public void DrawBMP(Bitmap bmp, RectangleF drawRect, float r = 0)
            => DrawBMP(bmp, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height, r);

        public void DrawBMP(Bitmap bmp, float x, float y, float w, float h, int ix, int iy, int iw, int ih, float r = 0)
            => DrawBMP(bitmapGraphics, bmp, (int)x, (int)y, (int)w, (int)h, r, ix, iy, iw, ih);

        public void DrawTri(Vector2 v1, Vector2 v2, Vector2 v3, Color color, bool fill = true, float r = 0)
            => DrawTri(bitmapGraphics, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, (int)v3.X, (int)v3.Y, color, fill, r);

        public void DrawText(string text, Font font, Color color, Vector2 pos, Vector2 size, float r = 0)
        {
            SetColor(color);

            Vector2 bmpSize = bitmapGraphics.MeasureString(text, font);

            using Bitmap textBmp = new((int)bmpSize.X, (int)bmpSize.Y);
            using Graphics textBmpGraphics = Graphics.FromImage(textBmp);

            textBmpGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            textBmpGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            textBmpGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            textBmpGraphics.DrawString(text, font, brush, new RectangleF(0, 0, bmpSize.X, bmpSize.Y),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

            // https://stackoverflow.com/questions/1940581/c-sharp-image-resizing-to-different-size-while-preserving-aspect-ratio
            double ratioX = (double)size.X / (double)bmpSize.X;
            double ratioY = (double)size.Y / (double)bmpSize.Y;

            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = (int)(bmpSize.Y * ratio);
            int newWidth = (int)(bmpSize.X * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            float posX = pos.X + (int)((size.X - (bmpSize.X * ratio)) / 2);
            float posY = pos.Y + (int)((size.Y - (bmpSize.Y * ratio)) / 2);

            // End stackoverflow code

            DrawBMP(textBmp, posX, posY, newWidth, newHeight, r);
        }

        public void DrawPolygon(Vector2[] vertices, Color color, bool fill = true, float r = 0)
        {
            Point[] points = new Point[vertices.Length];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = (Point)vertices[i];
            }

            DrawPolygon(bitmapGraphics, points, color, fill, r);
        }

        internal void RecreateGraphics()
        {
            formGraphics.Dispose();
            formGraphics = Platform.Form.CreateGraphics();
        }

        internal void End()
        {
            formGraphics.DrawImage(buffer, 0, 0);
            buffer.Dispose();
            bitmapGraphics.Dispose();
        }

        internal void SetColor(Color color)
        {
            brush.Color = color;
            pen?.Dispose();
            pen = new(brush);
        }

        internal void Rotate(PointF center, float angle)
        {
            Matrix m = new();
            m.RotateAt(angle, center);
            bitmapGraphics.Transform = m;
            m.Dispose();
        }

        internal void ResetRotation()
        {
            bitmapGraphics.ResetTransform();
        }

        internal void Begin()
        {
            buffer = new Bitmap(Platform.Form.ClientSize.Width, Platform.Form.ClientSize.Height, formGraphics);
            bitmapGraphics = Graphics.FromImage(buffer);

            SetColor(BackColor);
            bitmapGraphics.FillRectangle(brush, new Rectangle(0, 0, buffer.Width, buffer.Height));
        }

        // To draw rectangle at angle:
        // https://stackoverflow.com/questions/10210134/using-a-matrix-to-rotate-rectangles-individually
        private void DrawRectangle(Graphics g, int x, int y, int w, int h, Color color, bool fill = true, float angle = 0, double xAnchor = 0, double yAnchor = 0)
        {
            SetColor(color);
            Rotate(new PointF((float)(x + (w * xAnchor)), (float)(y + (h * yAnchor))), angle);

            if (fill)
            {
                g.FillRectangle(brush, new(x, y, w, h));
            }
            else
            {
                g.DrawRectangle(pen, new(x, y, w, h));
            }

            ResetRotation();
        }

        // TODO: tri and poly rotation
        private void DrawTri(Graphics g, int x1, int y1, int x2, int y2, int x3, int y3, Color color, bool fill = true, float r = 0)
        {
            SetColor(color);

            if (!fill)
            {
                g.DrawLine(pen, x1, y1, x2, y2);
                g.DrawLine(pen, x2, y2, x3, y3);
                g.DrawLine(pen, x3, y3, x1, y1);
            }
            else
            {
                g.FillPolygon(brush, new Point[] { new Point(x1, y1), new Point(x2, y2), new Point(x3, y3) });
            }
        }

        private void DrawPolygon(Graphics g, Point[] points, Color color, bool fill = true, float r = 0)
        {
            SetColor(color);

            if (fill)
            {
                g.FillPolygon(brush, points);
            }
            else
            {
                g.DrawPolygon(pen, points);
            }
        }

        private void DrawBMP(Graphics g, Bitmap bmp, int x, int y, int w, int h, float r, int ix, int iy, int iw, int ih)
        {
            Rotate(new PointF((x + w) / 2f, (y + h) / 2f), r);

            g.DrawImage(bmp, new Rectangle(x, y, w, h), new Rectangle(ix, iy, iw, ih), GraphicsUnit.Pixel);
            ResetRotation();
        }

        private void DrawEllipse(Graphics g, int x, int y, int w, int h, Color color, bool fill = true, float r = 0, double xAnchor = 0.5f, double yAnchor = 0.5f)
        {
            SetColor(color);
            Rotate(new PointF((float)(x + (w * xAnchor)), (float)(y + (h * yAnchor))), r);

            if (fill)
            {
                g.FillEllipse(brush, x, y, w, h);
            }
            else
            {
                g.DrawEllipse(pen, x, y, w, h);
            }

            ResetRotation();
        }

        private void DrawLine(Graphics g, int x1, int y1, int x2, int y2, Color color)
        {
            SetColor(color);
            g.DrawLine(pen, x1, y1, x2, y2);
        }
    }
}
