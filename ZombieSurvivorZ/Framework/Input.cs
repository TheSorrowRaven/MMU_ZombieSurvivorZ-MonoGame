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

        public static KeyboardState LastKeyboardState { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }

        public static void Update(GameTime gameTime)
        {
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
        }

        


    }
}
