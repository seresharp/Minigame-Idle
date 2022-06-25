using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MinigameIdle.Shooter
{
    public class ShooterGame : MiniGame
    {
        public IdleGame MainGame { get; init; }

        public float ShipX { get; private set; } = 408;

        public float ShipY { get; init; } = 775;

        private ShooterBackground Background { get; init; }
        private Texture2D ShipTex = null!;

        public ShooterGame(IdleGame game)
        {
            MainGame = game;
            Background = new(MainGame, this);
        }

        public override void Initialize()
        {
            ShipTex = MainGame.Content.Load<Texture2D>("Content/Ship");
        }

        public override void DoInput(GameTime gameTime)
        {
            if (MainGame.Input.IsPressed(Keys.Left))
            {
                ShipX -= 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (MainGame.Input.IsPressed(Keys.Right))
            {
                ShipX += 50 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (ShipX < 230)
            {
                ShipX = 230;
            }

            if (ShipX > 585)
            {
                ShipX = 585;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Background.Update(gameTime);
        }

        public override void Draw()
        {
            if (MainGame.ActiveGame != this)
            {
                return;
            }

            Background.Draw();
            MainGame.SpriteBatch.Draw(
                ShipTex,
                new Rectangle(MainGame.ScaleX((int)ShipX), MainGame.ScaleY((int)ShipY), MainGame.ScaleX(64), MainGame.ScaleY(64)),
                Color.CornflowerBlue);
        }
    }
}
