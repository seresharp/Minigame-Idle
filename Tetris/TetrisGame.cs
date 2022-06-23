using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MinigameIdle.Tetris
{
    public class TetrisGame : MiniGame
    {
        public IdleGame MainGame { get; init; }

        public float TickRate => MathF.Pow(.95f, BoughtUpgrades.TickUpgrades);
        private float _tickTimer;
        private float _startTimer;

        public Upgrades BoughtUpgrades { get; init; } = new();

        public TetrisGrid Grid { get; private set; } = null!;

        public Tetromino? CurrentTetromino { get; private set; }

        public Color?[,] DeadTetrominos { get; private set; } = null!;

        private Button StartGameButton = null!;
        private Button EndGameButton = null!;
        private Button AIButton = null!;
        private TetrisUpgradePanel UpgradeButtons = null!;

        private bool _gameRunning;
        public bool GameRunning
        {
            get => _gameRunning;
            set
            {
                // Game beginning
                if (value && !_gameRunning)
                {
                    CurrentTetromino = null;

                    for (int i = 0; i < Grid.Width; i++)
                    {
                        for (int j = 0; j < Grid.Height; j++)
                        {
                            DeadTetrominos[i, j] = null;
                        }
                    }
                }

                // Game ending
                if (!value && _gameRunning)
                {
                    MainGame.Points += GetGridPoints();
                }

                _gameRunning = value;
            }
        }

        public TetrisGame(IdleGame game)
        {
            MainGame = game;
        }

        public override void Initialize()
        {
            Grid = new(this, new Rectangle(), 10, 20);
            DeadTetrominos = new Color?[Grid.Width, Grid.Height];

            StartGameButton = new(MainGame, new Rectangle(), Color.DimGray, "Begin Game\n(inf)");
            EndGameButton = new(MainGame, new Rectangle(), Color.DimGray, "End Early\n+0 Points");
            AIButton = new(MainGame, new Rectangle(), Color.DimGray, "AI Enabled: false\n(Must be unlocked)");
            UpgradeButtons = new(this);
        }

        public void Resize()
        {
            Grid.Resize(new Rectangle(
                MainGame.ScaleX(225),
                MainGame.ScaleY(25),
                MainGame.ScaleX(430),
                MainGame.ScaleY(860)));

            StartGameButton.Resize(new Rectangle(
                MainGame.ScaleX(690),
                MainGame.ScaleY(25),
                MainGame.ScaleX(115),
                MainGame.ScaleY(80)));

            EndGameButton.Resize(new Rectangle(
                MainGame.ScaleX(820),
                MainGame.ScaleY(25),
                MainGame.ScaleX(115),
                MainGame.ScaleY(80)));

            AIButton.Resize(new Rectangle(
                MainGame.ScaleX(690),
                MainGame.ScaleY(120),
                MainGame.ScaleX(245),
                MainGame.ScaleY(80)));

            UpgradeButtons.Resize();
        }

        public override void DoInput(GameTime gameTime)
        {
            // Buttons
            if (StartGameButton.WasClicked())
            {
                GameRunning = true;
            }

            if (EndGameButton.WasClicked())
            {
                GameRunning = false;
                CurrentTetromino = null;
            }

            if (AIButton.WasClicked() && BoughtUpgrades.AIUnlocked)
            {
                BoughtUpgrades.AIEnabled = !BoughtUpgrades.AIEnabled;
            }

            // Upgrades panel button handling
            UpgradeButtons.Update();

            // Game input handling
            if (GameRunning && !BoughtUpgrades.AIEnabled && CurrentTetromino is { Alive: true })
            {
                if (BoughtUpgrades.BasicControls && MainGame.Input.WasPressed(Keys.Left))
                {
                    CurrentTetromino.TryMove(new Point(-1, 0));
                }

                if (BoughtUpgrades.BasicControls && MainGame.Input.WasPressed(Keys.Right))
                {
                    CurrentTetromino.TryMove(new Point(1, 0));
                }

                if (BoughtUpgrades.BasicControls && MainGame.Input.WasPressed(Keys.Up))
                {
                    CurrentTetromino.Rotate();
                }

                if (BoughtUpgrades.Teleport && MainGame.Input.WasPressed(Keys.Down))
                {
                    while (CurrentTetromino.Alive)
                    {
                        CurrentTetromino.Tick();
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Autostart
            if (GameRunning)
            {
                _startTimer = 0;
            }
            else if (BoughtUpgrades.AutoStartUpgrades > 0)
            {
                _startTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_startTimer >= Upgrades.AUTOSTART_TIME - BoughtUpgrades.AutoStartUpgrades)
                {
                    GameRunning = true;
                }
            }

            // Button text
            if (BoughtUpgrades.AutoStartUpgrades == 0)
            {
                StartGameButton.UpdateText("Begin Game\n(inf)");
            }
            else
            {
                StartGameButton.UpdateText($"Begin Game\n({(GameRunning ? 0 : (Upgrades.AUTOSTART_TIME - BoughtUpgrades.AutoStartUpgrades - _startTimer).ToString("0.0"))})");
            }

            EndGameButton.UpdateText($"End Early\n+{(GameRunning ? GetGridPoints().ToString("0.##") : 0)} Points");

            AIButton.UpdateText($"AI Enabled: {BoughtUpgrades.AIEnabled}{(BoughtUpgrades.AIUnlocked ? "" : "\n(Must be unlocked)")}");

            // Move tetrominos
            _tickTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_tickTimer >= TickRate)
            {
                _tickTimer -= TickRate;
                Tick();
            }
        }

        private void Tick()
        {
            if (!GameRunning)
            {
                CurrentTetromino = null;
                return;
            }

            // Move tetromino downward
            CurrentTetromino?.Tick();

            // No need to continue if the piece is still falling
            if (CurrentTetromino is { Alive: true })
            {
                return;
            }

            // Add old tetromino to dead square list
            if (CurrentTetromino != null)
            {
                foreach (Point p in CurrentTetromino.Squares)
                {
                    DeadTetrominos[p.X, p.Y] = CurrentTetromino.Color;
                }
            }

            // Check for filled lines
            for (int y = 0; y < Grid.Height; y++)
            {
                if (Enumerable.Range(0, Grid.Width)
                    .All(x => DeadTetrominos[x, y] != null))
                {
                    for (int x = 0; x < Grid.Width; x++)
                    {
                        DeadTetrominos[x, y] = null;
                    }

                    // Move squares above removed line down by one
                    for (int downY = y - 1; downY >= 0; downY--)
                    {
                        for (int x = 0; x < Grid.Width; x++)
                        {
                            if (DeadTetrominos[x, downY] != null)
                            {
                                DeadTetrominos[x, downY + 1] = DeadTetrominos[x, downY];
                                DeadTetrominos[x, downY] = null;
                            }
                        }
                    }

                    // Grant points for filled line
                    MainGame.Points += GetLinePoints();
                }
            }

            // Check for game end
            bool gameOver = false;
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (DeadTetrominos[x, y] != null)
                    {
                        gameOver = true;
                        break;
                    }
                }

                if (gameOver)
                {
                    GameRunning = false;
                    break;
                }
            }

            // Build new tetromino
            CurrentTetromino = GameRunning
                ? BuildTetromino()
                : null;
        }

        public override void Draw()
        {
            if (MainGame.ActiveGame != this)
            {
                return;
            }

            // Tetris game
            Grid.DrawBackground();

            CurrentTetromino?.Draw();
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    if (DeadTetrominos[x, y] != null)
                    {
                        Grid.ColorSquare(x, y, DeadTetrominos[x, y]!.Value);
                    }
                }
            }

            Grid.DrawForeground();

            // UI
            StartGameButton.Draw();
            EndGameButton.Draw();
            AIButton.Draw();
            UpgradeButtons.Draw();
        }

        private readonly List<int> TetBag = new();
        private Tetromino BuildTetromino()
        {
            Color c = new Random().Next(6) switch
            {
                0 => Color.Red,
                1 => Color.Blue,
                2 => Color.Green,
                3 => Color.Yellow,
                4 => Color.Purple,
                5 => Color.Cyan,
                _ => throw new InvalidOperationException()
            };

            int tetChoice;
            if (BoughtUpgrades.BagRNG)
            {
                if (TetBag.Count == 0)
                {
                    TetBag.AddRange(Enumerable.Range(0, 7));
                }

                tetChoice = TetBag[new Random().Next(TetBag.Count)];
                TetBag.Remove(tetChoice);
            }
            else
            {
                tetChoice = new Random().Next(7);
            }

            return tetChoice switch
            {
                // Square
                0 => new(this, c, new Point(4, 0), new Point(4, 1), new Point(5, 0), new Point(5, 1)),
                // L
                1 => new(this, c, new Point(4, 0), new Point(4, 1), new Point(4, 2), new Point(5, 2)),
                // Backwards L
                2 => new(this, c, new Point(5, 0), new Point(5, 1), new Point(5, 2), new Point(4, 2)),
                // -_
                3 => new(this, c, new Point(4, 0), new Point(5, 0), new Point(5, 1), new Point(6, 1)),
                // _-
                4 => new(this, c, new Point(4, 1), new Point(5, 1), new Point(5, 0), new Point(6, 0)),
                // --
                5 => new(this, c, new Point(3, 0), new Point(4, 0), new Point(5, 0), new Point(6, 0)),
                // T
                6 => new(this, c, new Point(4, 0), new Point(5, 0), new Point(6, 0), new Point(5, 1)),
                _ => throw new InvalidOperationException()
            };
        }

        private double GetGridPoints()
        {
            int tetCount = DeadTetrominos.Count(c => c != null);
            return Math.Pow(tetCount, 1.5) / 100;
        }

        private double GetLinePoints()
        {
            return Grid.Width / 4.0;
        }

        [Serializable]
        public class Upgrades
        {
            // Basic game functionality
            public bool BasicControls;
            public bool Teleport;

            // AI Upgrades
            public bool AIEnabled;
            public bool AIUnlocked;
            public bool StupidAI;
            public bool AdvancedAI;
            public bool AITeleport;

            // Autostart
            public const int AUTOSTART_TIME = 11;
            public int AutoStartUpgrades; // Max 11

            // Tickrate
            public int TickUpgrades; // Max 100

            // Misc
            public bool BagRNG;
        }
    }
}
