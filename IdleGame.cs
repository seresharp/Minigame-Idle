using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinigameIdle.Plinko;
using MinigameIdle.Shooter;
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

        public PlinkoGame Plinko { get; init; }
        private Button PlinkoButton = null!;

        public ShooterGame Shooter { get; init; }
        private Button ShooterButton = null!;

        public InputManager Input { get; init; } = new();

        public GraphicsDeviceManager Graphics { get; init; }

        public SpriteBatch SpriteBatch { get; private set; } = null!;

        public SpriteFont Font { get; private set; } = null!;

        public IdleGame()
        {
            Graphics = new(this);

            Tetris = new TetrisGame(this);
            Plinko = new PlinkoGame(this);
            Shooter = new ShooterGame(this);

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

            Plinko.Initialize();
            PlinkoButton = new(this, new(), Color.DimGray, "Falling Balls");

            Shooter.Initialize();
            ShooterButton = new(this, new(), Color.DimGray, "Shooter");

            SpriteBatch = new(GraphicsDevice);
            SpriteBatch.InitPixel();

            Font = Content.Load<SpriteFont>("Content/Arial");

            // Ensure everything matches the default resolution
            ResizeAll();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            ActiveGame?.DoInput(gameTime);

            Tetris.Update(gameTime);
            Plinko.Update(gameTime);
            Shooter.Update(gameTime);

            if (TetrisButton.WasClicked())
            {
                ActiveGame = Tetris;
            }
            else if (PlinkoButton.WasClicked())
            {
                ActiveGame = Plinko;
            }
            else if (ShooterButton.WasClicked())
            {
                ActiveGame = Shooter;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);
            SpriteBatch.Begin();

            // Games
            Tetris.Draw();
            Plinko.Draw();
            Shooter.Draw();

            // Sidebar
            SpriteBatch.DrawRectangle(new(0, 0, ScaleX(200), ScaleY(910)), new Color(56, 56, 56));
            SpriteBatch.DrawRectangle(new(0, 0, ScaleX(200), ScaleY(100)), Color.ForestGreen);
            SpriteBatch.DrawText($"Points\n{Points.ToString(Points >= 1000000 ? "0.000E0" : "0.00")}", Font, Color.Black, new Vector2(0, 0), new Vector2(ScaleX(200), ScaleY(100)));

            TetrisButton.Draw();
            PlinkoButton.Draw();
            ShooterButton.Draw();

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

            Plinko.Resize();
            PlinkoButton.Resize(new(
                ScaleX(0),
                ScaleY(140),
                ScaleX(200),
                ScaleY(40)));

            ShooterButton.Resize(new(
                ScaleX(0),
                ScaleY(220),
                ScaleX(200),
                ScaleY(40)));
        }
    }
}
