using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool IsLMouseDown()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }
        public static bool IsLMouseUp()
        {
            return MouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstDown()
        {
            return IsLMouseDown() && LastMouseState.LeftButton == ButtonState.Released;
        }
        public static bool IsLMouseFirstUp()
        {
            return IsLMouseUp() && LastMouseState.LeftButton == ButtonState.Pressed;
        }

    }
}
