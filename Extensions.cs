using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinigameIdle
{
    public static class Extensions
    {
        public static int Count<T>(this T[,] self, Func<T, bool> predicate)
        {
            int num = 0;
            for (int i = 0; i < self.GetLength(0); i++)
            {
                for (int j = 0; j < self.GetLength(1); j++)
                {
                    if (predicate(self[i, j]))
                    {
                        num++;
                    }
                }
            }

            return num;
        }

        private static Texture2D WhitePixel = null!;

        public static void InitPixel(this SpriteBatch spr)
        {
            WhitePixel = new Texture2D(spr.GraphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });
        }

        public static void DrawLine(this SpriteBatch spr, Vector2 start, Vector2 end, Color c, int width = 1)
        {
            Rectangle r = new((int)start.X, (int)start.Y, (int)(end - start).Length() + width, width);
            Vector2 v = Vector2.Normalize(start - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (start.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spr.Draw(WhitePixel, r, null, c, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(this SpriteBatch spr, Rectangle rect, Color c, bool fill = true)
        {
            if (fill)
            {
                spr.Draw(WhitePixel, rect, c);
                return;
            }

            spr.DrawLine(new(rect.Left, rect.Top), new(rect.Right, rect.Top), c);
            spr.DrawLine(new(rect.Right, rect.Top), new(rect.Right, rect.Bottom), c);
            spr.DrawLine(new(rect.Right, rect.Bottom), new(rect.Left, rect.Bottom), c);
            spr.DrawLine(new(rect.Left, rect.Bottom), new(rect.Left, rect.Top), c);
        }

        public static void DrawText(this SpriteBatch spr, string text, SpriteFont font, Color c, Vector2 pos, Vector2 size)
        {
            // Split multiline text
            if (text.Contains('\n'))
            {
                string[] lines = text.Split('\n');
                size = new(size.X, size.Y / lines.Length);
                foreach (string line in lines)
                {
                    spr.DrawText(line, font, c, pos, size);
                    pos = new(pos.X, pos.Y + size.Y);
                }

                return;
            }

            // Calculate actual render size
            Vector2 textSize = font.MeasureString(text);
            float scale = MathF.Min(size.X / textSize.X, size.Y / textSize.Y);
            Vector2 newSize = textSize * scale;

            // Render text
            spr.DrawString(font, text, new(pos.X + (size.X - newSize.X) / 2, pos.Y), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public static bool IsInRect(this MouseState mouse, Rectangle rect)
            => mouse.X >= rect.Left && mouse.X <= rect.Right
            && mouse.Y >= rect.Top && mouse.Y <= rect.Bottom;
    }
}
