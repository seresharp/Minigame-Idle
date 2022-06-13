using MinigameIdle.Engine.Mathematics;

namespace MinigameIdle.Engine
{
    public class MouseManager : IDisposable
    {
        public Game Game { get; init; }

        private MouseState asyncMouseState;
        private MouseState oldMouseState;
        private MouseState currentMouseState;

        internal MouseManager(Game game)
        {
            Game = game;
            HookForm();
        }

        public MouseState State { get; private set; }

        internal void Update()
        {
            oldMouseState = currentMouseState;
            currentMouseState = asyncMouseState;

            int x = currentMouseState.X;
            int y = currentMouseState.Y;

            Click l = new(currentMouseState.Left.IsClicked, !oldMouseState.Left.IsClicked && currentMouseState.Left.IsClicked);
            Click r = new(currentMouseState.Right.IsClicked, !oldMouseState.Right.IsClicked && currentMouseState.Right.IsClicked);
            Click c = new(currentMouseState.Center.IsClicked, !oldMouseState.Center.IsClicked && currentMouseState.Center.IsClicked);

            State = new MouseState(x, y, l, r, c);
        }

        private void HookForm()
        {
            UnhookForm();

            Game.Platform.Form.MouseDown += MouseDown;
            Game.Platform.Form.MouseUp += MouseUp;
            Game.Platform.Form.MouseMove += MouseMove;
        }

        private void UnhookForm()
        {
            Game.Platform.Form.MouseDown -= MouseDown;
            Game.Platform.Form.MouseUp -= MouseUp;
            Game.Platform.Form.MouseMove -= MouseMove;
        }

        private void MouseDown(object? sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    asyncMouseState.Left = new Click(true);
                    break;
                case MouseButtons.Right:
                    asyncMouseState.Right = new Click(true);
                    break;
                case MouseButtons.Middle:
                    asyncMouseState.Center = new Click(true);
                    break;
                default:
                    break;
            }
        }

        private void MouseUp(object? sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    asyncMouseState.Left = new Click(false);
                    break;
                case MouseButtons.Right:
                    asyncMouseState.Right = new Click(false);
                    break;
                case MouseButtons.Middle:
                    asyncMouseState.Center = new Click(false);
                    break;
                default:
                    break;
            }
        }

        private void MouseMove(object? sender, MouseEventArgs e)
        {
            asyncMouseState.X = e.X;
            asyncMouseState.Y = e.Y;
            asyncMouseState.Location = e.Location;
        }

        void IDisposable.Dispose()
        {
            UnhookForm();
            GC.SuppressFinalize(this);
        }

        public struct MouseState
        {
            public int X;
            public int Y;
            public Click Left;
            public Click Right;
            public Click Center;
            public Vector2 Location;

            internal MouseState(int x, int y, Click l, Click r, Click c)
            {
                X = x;
                Y = y;
                Left = l;
                Right = r;
                Center = c;
                Location = new Vector2(x, y);
            }

            public bool IsInRect(Rectangle rect)
                => X >= rect.Left && X <= rect.Right
                && Y >= rect.Top && Y <= rect.Bottom;
        }

        public struct Click
        {
            internal Click(bool clicked)
            {
                IsClicked = clicked;
                WasClicked = false;
            }

            internal Click(bool clicked, bool wasClicked)
            {
                IsClicked = clicked;
                WasClicked = wasClicked;
            }

            public bool IsClicked { get; init; }

            public bool WasClicked { get; init; }
        }
    }
}
