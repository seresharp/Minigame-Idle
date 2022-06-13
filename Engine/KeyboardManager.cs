using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MinigameIdle.Engine
{
    public struct KeyState
    {
        public KeyState(Keys key, bool isPressed, bool wasPressed)
        {
            Key = key;
            IsPressed = isPressed;
            WasPressed = wasPressed;
        }

        public bool IsPressed { get; init; }

        public bool WasPressed { get; init; }

        public Keys Key { get; init; }
    }

    public class KeyboardManager : IDisposable
    {
        // Caching this should improve performance a bit
        private static readonly Keys[] AllKeys = (Keys[])Enum.GetValues(typeof(Keys));

        private readonly Game game;

        private readonly Dictionary<Keys, bool> asyncKeys;
        private readonly Dictionary<Keys, bool> oldKeys;
        private readonly Dictionary<Keys, bool> currentKeys;

        internal KeyboardManager(Game game)
        {
            this.game = game;

            // Fill all dictionaries with false
            asyncKeys = new Dictionary<Keys, bool>();
            foreach (Keys key in AllKeys)
            {
                // This check is required because the Keys enum contains duplicate values (ex. return/enter are both 13)
                if (!asyncKeys.ContainsKey(key))
                {
                    asyncKeys.Add(key, false);
                }
            }

            oldKeys = new Dictionary<Keys, bool>(asyncKeys);
            currentKeys = new Dictionary<Keys, bool>(asyncKeys);

            // Begin looking for input
            HookForm();
        }

        public KeyState this[Keys key] => new(key, IsPressed(key), WasPressed(key));

        public bool IsPressed(Keys key)
        {
            currentKeys.TryGetValue(key, out bool ret);
            return ret;
        }

        public bool WasPressed(Keys key)
        {
            oldKeys.TryGetValue(key, out bool old);
            currentKeys.TryGetValue(key, out bool current);

            return current && !old;
        }

        public bool AlphaIsPressed()
        {
            foreach (KeyValuePair<Keys, bool> entry in currentKeys)
            {
                if (entry.Key <= Keys.Z && entry.Key >= Keys.A && entry.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Keys> ListAlphaPressed()
        {
            List<Keys> o = new();
            foreach (KeyValuePair<Keys, bool> entry in currentKeys)
            {
                if (entry.Key <= Keys.Z && entry.Key >= Keys.A && entry.Value)
                {
                    o.Add(entry.Key);
                }
            }

            return o;
        }

        public bool NumIsPressed()
        {
            foreach (KeyValuePair<Keys, bool> entry in currentKeys)
            {
                if (((entry.Key <= Keys.NumPad9 && entry.Key >= Keys.NumPad0)
                    || (entry.Key <= Keys.D9 && entry.Key >= Keys.D0)) && entry.Value)
                {
                    return true;
                }
            }

            return false;
        }

        public List<Keys> ListNumPressed()
        {
            List<Keys> o = new();
            foreach (KeyValuePair<Keys, bool> entry in currentKeys)
            {
                if ((entry.Key <= Keys.NumPad9 && entry.Key >= Keys.NumPad0)
                    || (entry.Key <= Keys.D9 && entry.Key >= Keys.D0))
                {
                    if (entry.Value)
                    {
                        o.Add(entry.Key);
                    }
                }
            }

            return o;
        }

        void IDisposable.Dispose()
        {
            UnhookForm();
            GC.SuppressFinalize(this);
        }

        internal void Update()
        {
            foreach (Keys key in AllKeys)
            {
                oldKeys[key] = currentKeys[key];
                currentKeys[key] = asyncKeys[key];
            }
        }

        private void HookForm()
        {
            // Prevent duplicate hooks
            UnhookForm();

            game.Platform.Form.KeyDown += RegisterKeyDown;
            game.Platform.Form.KeyUp += RegisterKeyUp;
        }

        private void UnhookForm()
        {
            game.Platform.Form.KeyDown -= RegisterKeyDown;
            game.Platform.Form.KeyUp -= RegisterKeyUp;
        }

        private void RegisterKeyDown(object? sender, KeyEventArgs args) => asyncKeys[args.KeyCode] = true;

        private void RegisterKeyUp(object? sender, KeyEventArgs args) => asyncKeys[args.KeyCode] = false;
    }
}
