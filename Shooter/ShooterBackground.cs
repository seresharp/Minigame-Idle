using Microsoft.Xna.Framework;

namespace MinigameIdle.Shooter
{
    public class ShooterBackground
    {
        public IdleGame MainGame { get; init; }

        public ShooterGame Shooter { get; init; }

        private readonly List<Particle> Particles = new();

        public ShooterBackground(IdleGame mainGame, ShooterGame shooterGame)
        {
            MainGame = mainGame;
            Shooter = shooterGame;
        }

        private float _flameTime;
        private float _starTime;
        public void Update(GameTime gameTime)
        {
            // Add stars
            _starTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_starTime <= 0)
            {
                Random rnd = new();
                _starTime = ((float)rnd.NextDouble() * 0.25f) + 0.1f;

                Particles.Add(new(
                    MainGame.ScaleX(rnd.Next(225, 650)),
                    MainGame.ScaleY(25),
                    rnd.Next(1, 5),
                    MainGame.ScaleY(rnd.Next(20, 500)),
                    Color.White));
            }

            // Move stars downward
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Y += Particles[i].Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Particles[i].Y + Particles[i].Size >= MainGame.ScaleY(885))
                {
                    Particles.RemoveAt(i--);
                }
            }

            // Ship flames
            _flameTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_flameTime <= 0)
            {
                Random rnd = new();
                _flameTime = ((float)rnd.NextDouble() * 0.05f) + 0.01f;

                int x = rnd.Next(10, 22);
                if (rnd.Next(2) == 0)
                {
                    x += 31;
                }

                Particles.Add(new(
                MainGame.ScaleX((int)Shooter.ShipX + x),
                MainGame.ScaleY((int)Shooter.ShipY + 64),
                MainGame.ScaleX(3),
                MainGame.ScaleY(50),
                Color.Red));
            }
        }

        public void Draw()
        {
            MainGame.SpriteBatch.DrawRectangle(new(
                MainGame.ScaleX(225),
                MainGame.ScaleY(25),
                MainGame.ScaleX(430),
                MainGame.ScaleY(860)),
                Color.Black);

            foreach (Particle p in Particles)
            {
                MainGame.SpriteBatch.DrawRectangle(new(p.X, (int)p.Y, p.Size, p.Size), p.Color);
            }
        }

        private class Particle
        {
            public int X { get; init; }

            public float Y { get; set; }

            public int Size { get; init; }

            public int Speed { get; init; }

            public Color Color { get; init; }

            public Particle(int x, float y, int size, int speed, Color c)
            {
                X = x;
                Y = y;
                Size = size;
                Speed = speed;
                Color = c;
            }
        }
    }
}
