namespace MinigameIdle.Engine
{
    public class Platform
    {
        public GameForm Form { get; init; }

        public Game Game { get; init; }

        private IAsyncResult? _result;
        private readonly MethodInvoker _invoker;

        public Platform(Game game)
        {
            Game = game;
            Form = new GameForm(Game)
            {
                Text = "Minigame Idle",
                ClientSize = new Size(1280, 720),
                FormBorderStyle = FormBorderStyle.FixedSingle,
                BackColor = Color.HotPink,
                MaximizeBox = false,
                MinimizeBox = false
            };

            _invoker = new(PlatformIdle);

            Form.Hook();
            Form.Show();
        }

        public void BeginRun()
        {
            Application.Idle += PlatformIdle;
            Application.Run();

            Application.Idle -= PlatformIdle;
        }

        public void ResizeWindow(int x, int y)
        {
            Form.ClientSize = new Size(x, y);
            Form.Center();
            Game.Graphics.RecreateGraphics();
        }

        private void PlatformIdle(object? _, EventArgs __)
            => PlatformIdle();

        private void PlatformIdle()
        {
            if (_result == null)
            {
                _result = Form.BeginInvoke(_invoker);
            }
            else
            {
                Form.EndInvoke(_result);
                _result = null;
            }

            Game.ProcessFrame();
        }
    }
}
