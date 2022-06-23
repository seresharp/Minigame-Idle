using Microsoft.Xna.Framework;

namespace MinigameIdle
{
    public abstract class MiniGame
    {
        public abstract void Initialize();

        public abstract void Update(GameTime gameTime);

        public abstract void DoInput(GameTime gameTime);

        public abstract void Draw();
    }
}
