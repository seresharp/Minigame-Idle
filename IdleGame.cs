using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinigameIdle.Tetris;

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

        public InputManager Input { get; init; } = new();

        public GraphicsDeviceManager Graphics { get; init; }

        public SpriteBatch SpriteBatch { get; private set; } = null!;

        public SpriteFont Font { get; private set; } = null!;

        public IdleGame()
        {
            Graphics = new(this);

            Tetris = new TetrisGame(this);
            ActiveGame = Tetris;
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = DEFAULT_WIDTH;
            Graphics.PreferredBackBufferHeight = DEFAULT_HEIGHT;
            Graphics.ApplyChanges();

            IsMouseVisible = true;

            Tetris.Initialize();
            TetrisButton = new(this, new(), Color.DimGray, "Falling Blocks");

            SpriteBatch = new(GraphicsDevice);
            SpriteBatch.InitPixel();

            Font = Content.Load<SpriteFont>("Content/Arial");

            // Ensure everything matches the default resolution
            ResizeAll();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            Tetris.Update(gameTime);

            if (TetrisButton.WasClicked())
            {
                ActiveGame = Tetris;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);
            SpriteBatch.Begin();

            // Tetris
            Tetris.Draw();

            // Sidebar
            SpriteBatch.DrawRectangle(new(0, 0, ScaleX(200), ScaleY(910)), new Color(56, 56, 56));
            SpriteBatch.DrawRectangle(new(0, 0, ScaleX(200), ScaleY(100)), Color.ForestGreen);
            SpriteBatch.DrawText($"Points\n{Points.ToString(Points >= 1000000 ? "0.000E0" : "0.00")}", Font, Color.Black, new Vector2(0, 0), new Vector2(ScaleX(200), ScaleY(100)));

            TetrisButton.Draw();

            //Graphics.DrawText((1/DeltaTime).ToString("00"), Button.DefaultFont, Color.Red, new(0, 800), new(200, 100));

            SpriteBatch.End();
        }

        public int ScaleX(int x)
            => (int)((float)x / DEFAULT_WIDTH * Graphics.PreferredBackBufferWidth);

        public int ScaleY(int y)
            => (int)((float)y / DEFAULT_HEIGHT * Graphics.PreferredBackBufferHeight);

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
