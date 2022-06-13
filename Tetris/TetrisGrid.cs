namespace MinigameIdle.Tetris
{
    public class TetrisGrid
    {
        public TetrisGame Game { get; init; }

        public Rectangle ScreenPos { get; private set; }
        public int Width { get; init; }
        public int Height { get; init; }

        public TetrisGrid(TetrisGame game, Rectangle screenPos, int width, int height)
        {
            Game = game;
            ScreenPos = screenPos;
            Width = width;
            Height = height;
        }

        public void Resize(Rectangle rect)
        {
            ScreenPos = rect;
        }

        public void DrawBackground()
        {
            // Background
            Game.MainGame.Graphics.DrawRectangle(ScreenPos, Color.FromArgb(1, 50, 32));

            // "Game Over" area
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    ColorSquare(x, y, Color.LightBlue);
                }
            }
        }

        public void DrawForeground()
        {
            // Vertical lines
            for (int x = 0; x <= Width; x++)
            {
                int lineX = (int)(ScreenPos.X + x * (ScreenPos.Width / (float)Width));
                Game.MainGame.Graphics.DrawLine(lineX, ScreenPos.Top, lineX, ScreenPos.Bottom, Color.Black);
            }

            // Horizontal lines
            for (int y = 0; y <= Height; y++)
            {
                int lineY = (int)(ScreenPos.Y + y * (ScreenPos.Height / (float)Height));
                Game.MainGame.Graphics.DrawLine(ScreenPos.Left, lineY, ScreenPos.Right, lineY, Color.Black);
            }
        }

        public void ColorSquare(int x, int y, Color color)
        {
            int x1 = (int)(ScreenPos.X + x * (ScreenPos.Width / (float)Width));
            int x2 = (int)(ScreenPos.X + (x + 1) * (ScreenPos.Width / (float)Width));

            int y1 = (int)(ScreenPos.Y + y * (ScreenPos.Height / (float)Height));
            int y2 = (int)(ScreenPos.Y + (y + 1) * (ScreenPos.Height / (float)Height));

            Game.MainGame.Graphics.DrawRectangle(x1, y1, x2 - x1, y2 - y1, color);
        }
    }
}
