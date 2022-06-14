using Microsoft.Xna.Framework;

namespace MinigameIdle.Tetris
{
    public class Tetromino
    {
        public TetrisGame Game { get; init; }

        public Point[] Squares { get; init; }
        public Color Color { get; init; }

        public bool Alive { get; private set; } = true;

        public Tetromino(TetrisGame game, Color color, params Point[] squares)
        {
            Game = game;
            Color = color;
            Squares = squares;

            if (Game.BoughtUpgrades.AIUnlocked && Game.BoughtUpgrades.AIEnabled)
            {
                if (Game.BoughtUpgrades.AdvancedAI)
                {
                    AdvancedAI();
                }
                else if (Game.BoughtUpgrades.StupidAI)
                {
                    StupidAI();
                }
                else
                {
                    RandomAI();
                }

                while (Game.BoughtUpgrades.AITeleport && Alive)
                {
                    Tick();
                }
            }
        }

        public void Tick()
        {
            if (!Alive)
            {
                return;
            }

            if (!TryMove(new Point(0, 1)))
            {
                Alive = false;
            }
        }

        public void Draw()
        {
            foreach (Point p in Squares)
            {
                Game.Grid.ColorSquare(p.X, p.Y, Color);
            }
        }

        public bool TryMove(Point p)
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Point nextPos = new(Squares[i].X + p.X, Squares[i].Y + p.Y);
                if (nextPos.X < 0 || nextPos.X >= Game.Grid.Width
                    || nextPos.Y < 0 || nextPos.Y >= Game.Grid.Height
                    || IntersectsOther(nextPos))
                {
                    return false;
                }
            }

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].X += p.X;
                Squares[i].Y += p.Y;
            }

            return true;
        }

        public void Rotate()
        {
            Point tl = new(Squares.Min(p => p.X), Squares.Min(p => p.Y));

            // Rotate
            for (int i = 0; i < Squares.Length; i++)
            {
                int tempX = Squares[i].X;
                Squares[i].X = Squares[i].Y - tl.Y + tl.X;
                Squares[i].Y = -(tempX - tl.X) + tl.Y;
            }

            Point newTl = new(Squares.Min(p => p.X), Squares.Min(p => p.Y));

            // Restore old top left position
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].X += tl.X - newTl.X;
                Squares[i].Y += tl.Y - newTl.Y;
            }

            // Keep inbounds on the right side
            while (Squares.Any(p => p.X >= Game.Grid.Width))
            {
                for (int i = 0; i < Squares.Length; i++)
                {
                    Squares[i].X--;
                }
            }

            // Keep inbounds on the bottom and prevent intersection
            while (Squares.Any(p => p.Y >= Game.Grid.Height)
                || Squares.Any(IntersectsOther))
            {
                for (int i = 0; i < Squares.Length; i++)
                {
                    Squares[i].Y--;
                }
            }
        }

        private Point[] SimulateDown()
        {
            Point[] orig = new Point[Squares.Length];
            Squares.CopyTo(orig, 0);

            while (TryMove(new(0, 1))) ;

            Point[] simulated = new Point[Squares.Length];
            Squares.CopyTo(simulated, 0);
            orig.CopyTo(Squares, 0);

            return simulated;
        }

        private bool IntersectsOther(Point p)
            => p.Y >= 0 && Game.DeadTetrominos[p.X, p.Y] != null;

        private void RandomAI()
        {
            int left = Squares.Min(p => p.X);
            int width = Squares.Max(p => p.X) - left + 1;
            int goal = new Random().Next(0, Game.Grid.Width - width + 1);

            TryMove(new(goal - left, 0));
        }

        private void StupidAI()
        {
            int left = Squares.Min(p => p.X);
            int width = Squares.Max(p => p.X) - left + 1;

            int lowest = -int.MaxValue;
            int goal = -1;
            TryMove(new(-left, 0));
            do
            {
                Point[] points = SimulateDown();
                int top = points.Min(s => s.Y);
                if (top > lowest)
                {
                    lowest = top;
                    goal = points.Min(p => p.X);
                }
            }
            while (TryMove(new(1, 0)));

            left = Squares.Min(p => p.X);
            TryMove(new(goal - left, 0));
        }

        private void AdvancedAI()
        {
            double maxScore = double.NegativeInfinity;
            int maxR = 0;
            int maxL = 0;
            for (int r = 0; r < 4; r++)
            {
                // Calculate width of piece and move to the left side to begin iteration of moves
                int left = Squares.Min(p => p.X);
                int width = Squares.Max(p => p.X) - left + 1;
                TryMove(new(-left, 0));
                do
                {
                    // Simulate move and add it to dead tetrominos for simplicity
                    Point[] simulated = SimulateDown();
                    foreach (Point p in simulated)
                    {
                        Game.DeadTetrominos[p.X, p.Y] = Color;
                    }

                    // Calculate aggregate height
                    int[] heights = new int[Game.Grid.Width];
                    for (int x = 0; x < Game.Grid.Width; x++)
                    {
                        for (int y = 0; y < Game.Grid.Height; y++)
                        {
                            if (Game.DeadTetrominos[x, y] != null)
                            {
                                heights[x] = Game.Grid.Height - y;
                                break;
                            }
                        }
                    }

                    int heightSum = heights.Sum();

                    // Calculate completed lines
                    int lines = 0;
                    for (int y = 0; y < Game.Grid.Height; y++)
                    {
                        if (Enumerable.Range(0, Game.Grid.Width)
                            .All(x => Game.DeadTetrominos[x, y] != null))
                        {
                            lines++;
                        }
                    }

                    // Calculate holes
                    int holes = 0;
                    for (int x = 0; x < Game.Grid.Width; x++)
                    {
                        for (int y = 0; y < Game.Grid.Height; y++)
                        {
                            if (Game.DeadTetrominos[x, y] == null
                                && Enumerable.Range(0, y)
                                .Any(upY => Game.DeadTetrominos[x, upY] != null))
                            {
                                holes++;
                            }
                        }
                    }

                    // Calculate bumpiness
                    int bumpiness = 0;
                    for (int x = 0; x < Game.Grid.Width - 1; x++)
                    {
                        bumpiness += Math.Abs(heights[x] - heights[x + 1]);
                    }

                    // Final score
                    double score = (.760666 * lines) - (.510066 * heightSum) - (.35663 * holes) - (.184483 * bumpiness);
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxR = r;
                        maxL = simulated.Min(p => p.X);
                    }

                    // Remove simulated piece from dead tetrominos
                    foreach (Point p in simulated)
                    {
                        Game.DeadTetrominos[p.X, p.Y] = null;
                    }
                }
                while (TryMove(new(1, 0)));

                Rotate();
            }

            for (int i = 0; i < maxR; i++)
            {
                Rotate();
            }

            TryMove(new(maxL - Squares.Min(p => p.X), 0));
        }
    }
}
