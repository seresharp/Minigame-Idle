namespace MinigameIdle.Engine
{
    public class Button
    {
        public static readonly Font DefaultFont = new("Tahoma", 32);

        public Game Game { get; init; }

        public Rectangle ScreenPos { get; private set; }
        public Color Color { get; init; }
        public string Text { get; private set; }

        public Button(Game game, Rectangle screenPos, Color c, string text)
        {
            Game = game;
            ScreenPos = screenPos;
            Color = c;
            Text = text;
        }

        public bool WasClicked()
            => Game.Mouse.State.Left.WasClicked
            && Game.Mouse.State.IsInRect(ScreenPos);

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
            Game.Graphics.DrawRectangle(ScreenPos, Color);
            Game.Graphics.DrawRectangle(ScreenPos, Color.Black, false);

            Game.Graphics.DrawText(
                Text,
                DefaultFont,
                Color.Black,
                ScreenPos.Location,
                ScreenPos.Size);
        }
    }
}
