using System.Diagnostics;
using MinigameIdle.Engine.Mathematics;

namespace MinigameIdle.Engine
{
    public abstract class Game
    {
        public Platform Platform { get; init; }
        public GraphicsManager Graphics { get; init; }
        public KeyboardManager Keyboard { get; init; }

        public MouseManager Mouse { get; init; }

        private bool _lockCursor;
        public bool LockCursor
        {
            get => _lockCursor;
            set
            {
                _lockCursor = value;
                Platform.Form.SetCursorLocked(value);
            }
        }

        private Vector2 _resolution;
        public Vector2 Resolution
        {
            get => _resolution;
            set
            {
                _resolution = value;
                Platform.ResizeWindow((int)_resolution.X, (int)_resolution.Y);
            }
        }

        private readonly Stopwatch _globalTimer = new();
        public float DeltaTime { get; private set; }

        internal void SetResolutionInternal(Vector2 res)
            => _resolution = res;

        protected Game()
        {
            Platform = new(this);
            Graphics = new(this, Platform);
            Keyboard = new(this);
            Mouse = new(this);
        }

        public void Run()
        {
            Initialize();
            _globalTimer.Start();
            Platform.BeginRun();
        }

        public abstract void Initialize();

        public abstract void Update();

        public abstract void Draw();

        internal void ProcessFrame()
        {
            DeltaTime = (float)_globalTimer.Elapsed.TotalSeconds;
            _globalTimer.Restart();

            Keyboard.Update();
            Mouse.Update();
            Update();

            Graphics.Begin();
            Draw();
            Graphics.End();
        }
    }
}
