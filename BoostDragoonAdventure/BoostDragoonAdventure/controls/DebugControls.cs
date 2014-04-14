using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace wickedcrush.controls
{
    public class DebugControls
    {
        private KeyboardState keyState, prevKeyState;
        private MouseState mouseState, prevMouseState;

        public DebugControls()
        {
            keyState = new KeyboardState();
            prevKeyState = keyState;

            mouseState = new MouseState();
            prevMouseState = mouseState;
        }

        public void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
        }

        public bool KeyPressed(Keys k)
        {
            if (keyState.IsKeyDown(k) && prevKeyState.IsKeyUp(k))
                return true;
            return false;
        }
    }
}
