namespace MinigameIdle.Engine
{
    public class GameForm : Form
    {
        public Game Game { get; init; }

        public GameForm(Game game)
        {
            Game = game;
        }

        ~GameForm()
        {
            SetCursorLocked(false);
        }

        public void Hook()
        {
            Resize -= RecreateGraphics;
            Resize += RecreateGraphics;

            GotFocus -= LockCursorCallback;
            GotFocus += LockCursorCallback;

            Resize -= LockCursorCallback;
            Resize += LockCursorCallback;

            ResizeEnd -= LockCursorCallback;
            ResizeEnd += LockCursorCallback;
        }

        public void SetCursorLocked(bool locked)
        {
            Cursor.Clip = locked && Game.LockCursor
                ? new Rectangle(
                    Location.X - PointToClient(Location).X,
                    Location.Y - PointToClient(Location).Y,
                    ClientSize.Width,
                    ClientSize.Height)
                : Rectangle.Empty;
        }

        public void Center()
        {
            Screen currentScreen = Screen.FromControl(this);
            Rectangle area = currentScreen.WorkingArea;

            Top = (area.Height - Height) / 2;
            Left = (area.Width - Width) / 2;

            SetCursorLocked(true);
        }

        private void RecreateGraphics(object? _, EventArgs __)
        {
            Game.Graphics.RecreateGraphics();
            Game.SetResolutionInternal(new(ClientSize.Width, ClientSize.Height));
        }

        private void LockCursorCallback(object? _, EventArgs __)
            => SetCursorLocked(true);
    }
}
