using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MinigameIdle
{
    public class InputManager
    {
        // Caching this should improve performance a bit
        private static readonly Keys[] AllKeys = (Keys[])Enum.GetValues(typeof(Keys));

        private readonly Dictionary<Keys, bool> oldKeys;
        private readonly Dictionary<Keys, bool> currentKeys;

        private MouseState _oldMouse;
        private MouseState _currentMouse;

        public InputManager()
        {
            // Fill all dictionaries with false
            oldKeys = new Dictionary<Keys, bool>();
            foreach (Keys key in AllKeys)
            {
                // This check is required because the Keys enum contains duplicate values (ex. return/enter are both 13)
                if (!oldKeys.ContainsKey(key))
                {
                    oldKeys.Add(key, false);
                }
            }

            currentKeys = new Dictionary<Keys, bool>(oldKeys);
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

        public MouseState GetMouseState()
            => new(
                _currentMouse.X,
                _currentMouse.Y,
                new(_currentMouse.Left.IsClicked, _currentMouse.Left.IsClicked && !_oldMouse.Left.IsClicked),
                new(_currentMouse.Right.IsClicked, _currentMouse.Right.IsClicked && !_oldMouse.Right.IsClicked),
                new(_currentMouse.Center.IsClicked, _currentMouse.Center.IsClicked && !_oldMouse.Center.IsClicked));

        public void Update()
        {
            foreach (Keys key in AllKeys)
            {
                oldKeys[key] = currentKeys[key];
                currentKeys[key] = Keyboard.GetState().IsKeyDown(key);
            }

            _oldMouse = _currentMouse;
            _currentMouse = new(
                Mouse.GetState().X,
                Mouse.GetState().Y,
                new(Mouse.GetState().LeftButton == ButtonState.Pressed),
                new(Mouse.GetState().RightButton == ButtonState.Pressed),
                new(Mouse.GetState().MiddleButton == ButtonState.Pressed));
        }
    }

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