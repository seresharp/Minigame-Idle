using Microsoft.Xna.Framework;

namespace MinigameIdle.Plinko
{
    public class PlinkoGame : MiniGame
    {
        public IdleGame MainGame { get; init; }

        public PlinkoUpgrades BoughtUpgrades { get; init; } = new();

        private readonly Peg?[,] Pegs;

        private readonly List<Ball> Balls = new();

        public PlinkoGame(IdleGame game)
        {
            MainGame = game;
            Pegs = new Peg[15, 8];
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

            // Setup teleports
            for (int x = 3; x < Pegs.GetLength(0); x += 4)
            {
                if (Pegs[x, Pegs.GetLength(1) - 1] is not Peg p)
                {
                    continue;
                }

                p.Top = Pegs[x + 1, 0];
            }

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

        private float _ballTimer;
        public override void Update(GameTime gameTime)
        {
            _ballTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_ballTimer >= 5f)
            {
                _ballTimer -= 5f;
                Balls.Add(new(this, Pegs[0, 0]));
            }

            for (int i = 0; i < Balls.Count; i++)
            {
                Balls[i].Progress += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Balls[i].Progress >= 1f)
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
                MainGame.ScaleY(400)),
                Color.LightBlue);

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
            MainGame.SpriteBatch.DrawRectangle(new(225, 416, 710, 9), Color.Black);
            for (int x = 0; x < Pegs.GetLength(0); x++)
            {
                MainGame.SpriteBatch.DrawRectangle(new(
                    MainGame.ScaleX(226 + (50 * x)),
                    MainGame.ScaleY(370),
                    MainGame.ScaleX(7),
                    MainGame.ScaleY(50)),
                    Color.Black);
            }


            // Point text and teleporter pads
            for (int x = 0; x < Pegs.GetLength(0) - 1; x++)
            {
                if (x is 3 or 7 or 11)
                {
                    Color c = x switch
                    {
                        3 => Color.Purple,
                        7 => Color.DarkOliveGreen,
                        _ => Color.MediumVioletRed
                    };

                    MainGame.SpriteBatch.DrawRectangle(new(
                        MainGame.ScaleX(235 + (50 * x)),
                        MainGame.ScaleY(416),
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
                    new(MainGame.ScaleX(235 + (50 * x)), MainGame.ScaleY(366)),
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
                if (Current.Top != null)
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
