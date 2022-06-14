using Microsoft.Xna.Framework;

namespace MinigameIdle
{
    public class Button
    {
        public IdleGame Game { get; init; }

        public Rectangle ScreenPos { get; private set; }
        public Color Color { get; init; }
        public string Text { get; private set; }

        public Button(IdleGame game, Rectangle screenPos, Color c, string text)
        {
            Game = game;
            ScreenPos = screenPos;
            Color = c;
            Text = text;
        }

        public bool WasClicked()
            => Game.Input.GetMouseState().Left.WasClicked
            && Game.Input.GetMouseState().IsInRect(ScreenPos);

        public void Resize(Rectangle screenPos)
        {
            ScreenPos = screenPos;
        }

        public void UpdateText(string text)
        {
            Text = text;
        }

        public void Draw()
        {
            Game.SpriteBatch.DrawRectangle(ScreenPos, Color);
            Game.SpriteBatch.DrawRectangle(ScreenPos, Color.Black, false);

            Game.SpriteBatch.DrawText(
                Text,
                Game.Font,
                Color.Black,
                new(ScreenPos.Location.X + .02f * ScreenPos.Size.X, ScreenPos.Location.Y + .02f * ScreenPos.Size.Y),
                new(ScreenPos.Size.X * .96f, ScreenPos.Size.Y * .96f));
        }
    }
}
