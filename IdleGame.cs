using MinigameIdle.Engine;
using MinigameIdle.Engine.Mathematics;
using MinigameIdle.Tetris;
using Button = MinigameIdle.Engine.Button;

namespace MinigameIdle
{
    public class IdleGame : Game
    {
        public const int DEFAULT_WIDTH = 960;
        public const int DEFAULT_HEIGHT = 910;

        public MiniGame? ActiveGame { get; private set; }

        public double Points { get; set; } = 0;

        public TetrisGame Tetris { get; init; }
        private Button TetrisButton = null!;

        public IdleGame()
        {
            Tetris = new TetrisGame(this);
            ActiveGame = Tetris;
        }

        public override void Initialize()
        {
            Resolution = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            Graphics.BackColor = Color.DarkSlateGray;

            Tetris.Initialize();
            TetrisButton = new(this, new(), Color.DimGray, "Falling Blocks");

            // Ensure everything matches the default resolution
            ResizeAll();
        }

        public override void Update()
        {
            Tetris.Update();

            if (TetrisButton.WasClicked())
            {
                ActiveGame = Tetris;
            }
        }

        public override void Draw()
        {
            // Tetris
            Tetris.Draw();

            // Sidebar
            Graphics.DrawRectangle(0, 0, ScaleX(200), ScaleY(910), Color.FromArgb(56, 56, 56));
            Graphics.DrawRectangle(0, 0, ScaleX(200), ScaleY(100), Color.ForestGreen);
            Graphics.DrawText($"Points\n{Points.ToString(Points >= 1000000 ? "0.000E0" : "0.0")}", Button.DefaultFont, Color.Black, new Vector2(0, 0), new Vector2(ScaleX(200), ScaleY(100)));

            TetrisButton.Draw();
        }

        public int ScaleX(int x)
            => (int)((float)x / DEFAULT_WIDTH * Resolution.X);

        public int ScaleY(int y)
            => (int)((float)y / DEFAULT_HEIGHT * Resolution.Y);

        private void ResizeAll()
        {
            Tetris.Resize();
            TetrisButton.Resize(new(
                ScaleX(0),
                ScaleY(100),
                ScaleX(200),
                ScaleY(40)));
        }
    }
}
