using Microsoft.Xna.Framework;

namespace MinigameIdle.Plinko
{
    public class PlinkoGame : MiniGame
    {
        public IdleGame MainGame { get; init; }

        public PlinkoUpgrades BoughtUpgrades { get; init; } = new();

        private readonly Peg?[,] Pegs;

        private readonly List<Ball> Balls = new();

        private readonly Button SpawnButton;
        private readonly Button AutoSpawnButton;
        private readonly Button ImprovedRngButton;
        private readonly Button TeleButton;

        public PlinkoGame(IdleGame game)
        {
            MainGame = game;
            Pegs = new Peg[15, 14];
            SpawnButton = new(MainGame, new(), Color.DimGray, "Spawn Ball");
            AutoSpawnButton = new(MainGame, new(), Color.DimGray, "");
            ImprovedRngButton = new(MainGame, new(), Color.DimGray, "");
            TeleButton = new(MainGame, new(), Color.DimGray, "");
        }

        public override void Initialize()
        {
            // Create pegs
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                for (int y = 0; y < Pegs.GetLength(1); y++)
                {
                    if (y % 2 != 0 && x == Pegs.GetLength(0) - 1)
                    {
                        continue;
                    }

                    int pegX = 230 + (x * 50);
                    if (y % 2 != 0)
                    {
                        pegX += 25;
                    }

                    int pegY = 70 + (y * 50);

                    Pegs[x, y] = new(MainGame, pegX, pegY);
                }
            }

            // Setup left and right bounces
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                for (int y = 0; y < Pegs.GetLength(1) - 1; y++)
                {
                    if (Pegs[x, y] is not Peg p)
                    {
                        continue;
                    }

                    int lx = y % 2 == 0 ? x - 1 : x;
                    int rx = y % 2 == 0 ? x : x + 1;

                    if (lx >= 0)
                    {
                        p.Left = Pegs[lx, y + 1];
                    }

                    if (rx < Pegs.GetLength(0))
                    {
                        p.Right = Pegs[rx, y + 1];
                    }
                }
            }

            // Setup teleport
            if (Pegs[6, Pegs.GetLength(1) - 1] is not Peg telePeg)
            {
                throw new InvalidDataException($"Peg at 6,{Pegs.GetLength(1) - 1} must be non-null");
            }

            telePeg.Top = Pegs[7, 0];

            // Setup point rewards
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                if (Pegs[x, Pegs.GetLength(1) - 1] is not Peg p)
                {
                    continue;
                }

                p.Points = MathF.Pow(x + 1, 2.5f) / 10;
            }
        }

        public void Resize()
        {
            SpawnButton.Resize(new(
                MainGame.ScaleX(230),
                MainGame.ScaleY(30),
                MainGame.ScaleX(115),
                MainGame.ScaleY(25)));

            AutoSpawnButton.Resize(new(225, 805, 230, 80));
            ImprovedRngButton.Resize(new(225 + 240, 805, 230, 80));
            TeleButton.Resize(new(225 + 480, 805, 230, 80));
        }

        private float _manualBallTimer;
        public override void DoInput(GameTime gameTime)
        {
            // Spawn button
            _manualBallTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (SpawnButton.WasClicked() && _manualBallTimer >= 0.5f)
            {
                _manualBallTimer = 0f;
                Balls.Add(new(this, Pegs[0, 0] ?? throw new InvalidDataException("Top left peg is null!")));
            }

            // Upgrades UI
            if (AutoSpawnButton.WasClicked())
            {
                if (!BoughtUpgrades.AutoSpawn && MainGame.Points >= 100)
                {
                    MainGame.Points -= 100;
                    BoughtUpgrades.AutoSpawn = true;
                }
                else if (BoughtUpgrades.AutoSpawn && BoughtUpgrades.SpawnRate < 100
                    && MainGame.Points >= BoughtUpgrades.GetSpawnCost())
                {
                    MainGame.Points -= BoughtUpgrades.GetSpawnCost();
                    BoughtUpgrades.SpawnRate++;
                }
            }

            if (ImprovedRngButton.WasClicked() && BoughtUpgrades.ImprovedRNG < 20
                && MainGame.Points >= BoughtUpgrades.GetRngCost())
            {
                MainGame.Points -= BoughtUpgrades.GetRngCost();
                BoughtUpgrades.ImprovedRNG++;
            }

            if (TeleButton.WasClicked() && !BoughtUpgrades.Teleports
                && MainGame.Points >= BoughtUpgrades.GetTeleCost())
            {
                MainGame.Points -= BoughtUpgrades.GetTeleCost();
                BoughtUpgrades.Teleports = true;
            }
        }

        private float _ballTimer;
        public override void Update(GameTime gameTime)
        {
            // Button text TODO: Stop showing cost at max upgrades
            if (!BoughtUpgrades.AutoSpawn)
            {
                AutoSpawnButton.UpdateText("Unlock Auto Spawn\n100 Points");
            }
            else
            {
                AutoSpawnButton.UpdateText($"Spawn Time ({BoughtUpgrades.SpawnRate}/100)\n{BoughtUpgrades.GetSpawnCost()} Points");
            }

            ImprovedRngButton.UpdateText($"Improve RNG ({BoughtUpgrades.ImprovedRNG}/20)\n{BoughtUpgrades.GetRngCost()} Points");
            TeleButton.UpdateText($"Teleporter\n{BoughtUpgrades.GetTeleCost()} Points");

            // Auto spawn
            if (BoughtUpgrades.AutoSpawn)
            {
                _ballTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (_ballTimer >= BoughtUpgrades.GetSpawnTime())
                {
                    _ballTimer -= BoughtUpgrades.GetSpawnTime();
                    Balls.Add(new(this, Pegs[0, 0] ?? throw new InvalidDataException("Top left peg is null!")));
                }
            }

            for (int i = 0; i < Balls.Count; i++)
            {
                Balls[i].Progress += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Balls[i].Progress >= 1)
                {
                    if (Balls[i].Current == Balls[i].Next)
                    {
                        MainGame.Points += Balls[i].Current.Points;
                        Balls.RemoveAt(i--);
                    }
                    else
                    {
                        Balls[i] = new(this, Balls[i].Next);
                    }
                }
            }
        }

        public override void Draw()
        {
            if (MainGame.ActiveGame != this)
            {
                return;
            }

            // Background
            MainGame.SpriteBatch.DrawRectangle(new(
                MainGame.ScaleX(225),
                MainGame.ScaleY(25),
                MainGame.ScaleX(710),
                MainGame.ScaleY(700)),
                Color.LightBlue);

            // UI
            SpawnButton.Draw();
            AutoSpawnButton.Draw();
            ImprovedRngButton.Draw();

            if (!BoughtUpgrades.Teleports)
            {
                TeleButton.Draw();
            }

            // Pegs
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                // Bottom row of pegs is invis, these are the reward buckets
                for (int y = 0; y < Pegs.GetLength(1) - 1; y++)
                {
                    Pegs[x, y]?.Draw();
                }
            }

            // Bottom slots
            MainGame.SpriteBatch.DrawRectangle(new(
                MainGame.ScaleX(225),
                MainGame.ScaleY(716),
                MainGame.ScaleX(710),
                MainGame.ScaleY(9)),
                Color.Black);
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                MainGame.SpriteBatch.DrawRectangle(new(
                    MainGame.ScaleX(226 + (50 * x)),
                    MainGame.ScaleY(670),
                    MainGame.ScaleX(7),
                    MainGame.ScaleY(50)),
                    Color.Black);
            }

            // Point text and teleporter pads
            for (int x = 0; x < Pegs.GetLength(0) - 1; x++)
            {
                if (x == 6 && BoughtUpgrades.Teleports)
                {
                    Color c = x switch
                    {
                        3 => Color.Purple,
                        7 => Color.DarkOliveGreen,
                        _ => Color.MediumVioletRed
                    };

                    MainGame.SpriteBatch.DrawRectangle(new(
                        MainGame.ScaleX(235 + (50 * x)),
                        MainGame.ScaleY(716),
                        MainGame.ScaleX(39),
                        MainGame.ScaleY(9)),
                        c);

                    MainGame.SpriteBatch.DrawRectangle(new(
                        MainGame.ScaleX(260 + (50 * x)),
                        MainGame.ScaleY(50),
                        MainGame.ScaleX(39),
                        MainGame.ScaleY(9)),
                        c);

                    continue;
                }

                int y = Pegs.GetLength(1) - 1;
                if (Pegs[x, y] is not { Points: double p })
                {
                    continue;
                }

                MainGame.SpriteBatch.DrawText(
                    p.ToString("0.0"),
                    MainGame.Font,
                    Color.DarkGreen,
                    new(MainGame.ScaleX(235 + (50 * x)), MainGame.ScaleY(666)),
                    new(MainGame.ScaleX(39), MainGame.ScaleY(50)));
            }

            // Balls
            foreach (Ball b in Balls)
            {
                int cx = b.Current.X;
                int nx = b.Next.X;
                int cy = b.Current.Y;
                int ny = b.Next.Y;
                float yfunc = MathF.Pow(2, 6 * (b.Progress - 1));

                MainGame.SpriteBatch.DrawRectangle(new(
                    MainGame.ScaleX(cx + (int)(b.Progress * (nx - cx)) - 4),
                    MainGame.ScaleY(cy + (int)(yfunc * (ny - cy)) - 11),
                    MainGame.ScaleX(7),
                    MainGame.ScaleY(7)),
                    Color.Red);
            }
        }

        public class PlinkoUpgrades
        {
            // Max 20
            public int ImprovedRNG;

            public bool AutoSpawn;

            // Max 100
            public int SpawnRate;

            public bool Teleports;

            public float GetSpawnTime()
                => 5f * MathF.Pow(.975f, SpawnRate);

            public float GetSpawnCost()
                => MathF.Pow(SpawnRate + 1, 3) / 50;

            public float GetRngCost()
                => 2500 * (ImprovedRNG + 1);

            public float GetTeleCost()
                => Teleports ? 0 : 1000;
        }

        private class Peg
        {
            public IdleGame Game { get; init; }

            public int X { get; init; }

            public int Y { get; init; }

            public Peg? Left { get; set; }

            public Peg? Right { get; set; }

            public Peg? Top { get; set; }

            public double Points { get; set; }

            public Peg(IdleGame game, int x, int y)
            {
                Game = game;
                X = x;
                Y = y;
            }

            public void Draw()
            {
                Game.SpriteBatch.DrawRectangle(new(
                    Game.ScaleX(X - 4),
                    Game.ScaleY(Y - 4),
                    Game.ScaleX(7),
                    Game.ScaleY(7)),
                    Color.Black);
            }
        }

        private class Ball
        {
            private readonly PlinkoGame Game;

            public Peg Current;
            public Peg Next;
            public float Progress;

            public Ball(PlinkoGame game, Peg current)
            {
                Game = game;

                // Check for ball teleports
                Current = current;
                if (Current.Top != null && Game.BoughtUpgrades.Teleports)
                {
                    Current = Current.Top;
                }

                // Left/right logic
                if (Current.Left == null && Current.Right != null)
                {
                    Next = Current.Right;
                }
                else if (Current.Left != null && Current.Right == null)
                {
                    Next = Current.Left;
                }
                else if (Current.Left != null && Current.Right != null)
                {
                    Next = new Random().Next(2 + Game.BoughtUpgrades.ImprovedRNG) == 0
                        ? Current.Left
                        : Current.Right;
                }
                else
                {
                    // Stay in place for a cycle after reaching the bottom
                    Next = Current;
                }
            }
        }
    }
}
