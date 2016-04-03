using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace wickedcrush.controls
{
    public abstract class Controls
    {
        public bool remove = false;

        protected KeyboardState keyState, prevKeyState;
        protected MouseState mouseState, prevMouseState;
        protected List<char> inputKeys = new List<char>();

        public abstract void Update();
        
        public abstract float LStickXAxis();
        public abstract float LStickYAxis();
        public abstract float RStickXAxis();
        public abstract float RStickYAxis();

        public abstract bool DownPressed();
        public abstract bool UpPressed();
        public abstract bool LeftPressed();
        public abstract bool RightPressed();

        public abstract bool InteractHeld();
        public abstract bool InteractPressed();
        public abstract bool InteractReleased();

        public abstract bool WeaponHeld();
        public abstract bool WeaponPressed();
        public abstract bool WeaponReleased();

        public abstract bool ItemBHeld();
        public abstract bool ItemBPressed();
        public abstract bool ItemBReleased();

        public abstract bool ReverseBoostHeld();
        public abstract bool ReverseBoostPressed();
        public abstract bool ReverseBoostReleased();

        public abstract bool BoostHeld();
        public abstract bool BoostPressed();
        public abstract bool BoostReleased();

        public abstract bool UnequipPartPress();

        /*public abstract bool StrafeHeld();
        public abstract bool StrafePressed();*/

        public abstract bool StartPressed();
        public abstract bool SelectPressed();

        public abstract bool LockOnPressed();

        public abstract bool LaunchMenuPressed();

        public virtual bool KeyPressed(Keys key)
        {
            if (keyState.IsKeyDown(key) && prevKeyState.IsKeyUp(key))
                return true;

            return false;
        }

        public virtual bool EnterPressed()
        {
            if (keyState.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
                return true;

            return false;
        }

        public virtual bool EscapePressed()
        {
            if (keyState.IsKeyDown(Keys.Escape) && prevKeyState.IsKeyUp(Keys.Escape))
                return true;

            return false;
        }

        public virtual String GetInput()
        {
            String output = "";

            List<Keys> keys = keyState.GetPressedKeys().ToList<Keys>();

            bool shiftPressed = (keys.Contains<Keys>(Keys.LeftShift) || keys.Contains<Keys>(Keys.RightShift));

            foreach (Keys k in prevKeyState.GetPressedKeys().ToList<Keys>())
                keys.Remove(k);

            foreach (Keys k in keys)
            {
                if (k >= Keys.A && k <= Keys.Z || k >= Keys.D0 && k <= Keys.D9)
                    output += k.ToString();

                if (k == Keys.Space)
                    output += " ";

                if (k == Keys.Back)
                    output += "\b";
            }

            if (shiftPressed)
                output = output.ToUpper();
            else
                output = output.ToLower();

            return output;
        }

        public abstract bool WeaponScrollUp();
        public abstract bool WeaponScrollDown();

    }
}
