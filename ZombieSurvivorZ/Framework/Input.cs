using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZombieSurvivorZ
{
    public class Input
    {

        public static MouseState LastMouseState { get; private set; }
        public static MouseState MouseState { get; private set; }
        public static Vector2 MousePos => MouseState.Position.ToVector2();
        public static Vector2 LastMousePos => LastMouseState.Position.ToVector2();
        public static Vector2 MouseChangeDelta => MousePos - LastMousePos;

        public static KeyboardState LastKeyboardState { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }

        public static void Update(GameTime gameTime)
        {
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyUp(Keys key)
        {
            return KeyboardState.IsKeyUp(key);
        }
        public static bool IsKeyFirstDown(Keys key)
        {
            return IsKeyDown(key) && LastKeyboardState.IsKeyUp(key);
        }
        public static bool IsKeyFirstUp(Keys key)
        {
            return IsKeyUp(key) && LastKeyboardState.IsKeyDown(key);
        }
        public static float ConstructAxis(Keys positive, Keys negative)
        {
            float value = (IsKeyDown(positive) ? 1 : 0) + (IsKeyDown(negative) ? -1 : 0);
            return value;
        }
        public static Vector2 ConstructAxis2(Keys up, Keys down, Keys left, Keys right)
        {
            Vector2 axis2 = new(ConstructAxis(left, right), ConstructAxis(up, down));
            return axis2;
        }

        public static int GetNumberKeysDown(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                Keys key = GetNumKey(i);
                if (IsKeyDown(key))
                {
                    return i;
                }
            }
            return -1;
        }
        public static int GetNumberKeysFirstDown(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                Keys key = GetNumKey(i);
                if (IsKeyFirstDown(key))
                {
                    return i;
                }
            }
            return -1;
        }

        public static Keys GetNumKey(int val) => val switch
        {
            0 => Keys.D0,
            1 => Keys.D1,
            2 => Keys.D2,
            3 => Keys.D3,
            4 => Keys.D4,
            5 => Keys.D5,
            6 => Keys.D6,
            7 => Keys.D7,
            8 => Keys.D8,
            9 => Keys.D9,
            _ => Keys.None,
        };

        public static bool IsLMouseDown()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return MouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLMouseUp()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return MouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstDown()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return IsLMouseDown() && LastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstUp()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return IsLMouseUp() && LastMouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLMouseDownUI()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLMouseUpUI()
        {
            return MouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstDownUI()
        {
            return IsLMouseDownUI() && LastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstUpUI()
        {
            return IsLMouseUpUI() && LastMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsRMouseDown()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return MouseState.RightButton == ButtonState.Pressed;
        }
        public static bool IsRMouseUp()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return MouseState.RightButton == ButtonState.Released;
        }
        public static bool IsRMouseFirstDown()
        {
            if (Game1.UISuppressClick)
            {
                return false;
            }
            return IsRMouseDown() && LastMouseState.RightButton == ButtonState.Released;
        }
    }
}
