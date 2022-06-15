using Microsoft.Xna.Framework;

namespace MinigameIdle.Plinko
{
    public class PlinkoGame : MiniGame
    {
        public IdleGame MainGame { get; init; }

        private readonly Peg?[,] Pegs;

        private List<Ball> Balls = new();

        public PlinkoGame(IdleGame game)
        {
            MainGame = game;

            // Create pegs
            Pegs = new Peg[15, 8];
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

        public override void Initialize()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            if (Balls.Count == 0)
            {
                Balls.Add(new(Pegs[new Random().Next(Pegs.GetLength(0)), 0]));
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
                        Balls[i] = new(Balls[i].Next);
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
                MainGame.SpriteBatch.DrawRectangle(new(226 + (50 * x), 370, 7, 50), Color.Black);
            }

            // Balls
            foreach (Ball b in Balls)
            {
                int cx = b.Current.X;
                int nx = b.Next.X;
                int cy = b.Current.Y;
                int ny = b.Next.Y;
                //float yfunc = (MathF.Log10(b.Progress) + 2) / 2;
                float yfunc = MathF.Pow(2, 6 * (b.Progress - 1));

                MainGame.SpriteBatch.DrawRectangle(new(
                    MainGame.ScaleX(cx + (int)(b.Progress * (nx - cx)) - 4),
                    MainGame.ScaleY(cy + (int)(yfunc * (ny - cy)) - 11),
                    MainGame.ScaleX(7),
                    MainGame.ScaleY(7)),
                    Color.Red);
            }
        }

        private class Peg
        {
            public IdleGame Game { get; init; }

            public int X { get; init; }

            public int Y { get; init; }

            public Peg? Left { get; set; }

            public Peg? Right { get; set; }

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
            public Peg Current;
            public Peg Next;
            public float Progress;

            public Ball(Peg current)
            {
                Current = current;
                if (current.Left == null && current.Right != null)
                {
                    Next = current.Right;
                }
                else if (current.Left != null && current.Right == null)
                {
                    Next = current.Left;
                }
                else if (current.Left != null && current.Right != null)
                {
                    Next = new Random().Next(2) == 0
                        ? current.Left
                        : current.Right;
                }
                else
                {
                    Next = current;
                }
            }
        }
    }
}
