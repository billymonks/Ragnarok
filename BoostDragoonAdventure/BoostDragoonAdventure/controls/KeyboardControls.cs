using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace wickedcrush.controls
{
    public class KeyboardControls : Controls
    {

        private Keys 
            interactKey = Keys.E,
            itemAKey = Keys.J, 
            itemBKey = Keys.K, 
            itemCKey = Keys.L, 
            boostKey = Keys.Space,
            reverseBoostKey = Keys.LeftShift, 
            downKey = Keys.S, 
            upKey = Keys.W, 
            leftKey = Keys.A, 
            rightKey = Keys.D,
            altDownKey = Keys.Down, 
            altUpKey = Keys.Up, 
            altLeftKey = Keys.Left, 
            altRightKey = Keys.Right,
            startKey = Keys.Enter, 
            selectKey = Keys.Escape, 
            walkKey = Keys.LeftControl;

        

        public KeyboardControls()
        {
            keyState = new KeyboardState();
            prevKeyState = keyState;

            mouseState = new MouseState();
            prevMouseState = mouseState;
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
        }

        public override bool InteractHeld()
        {
            if (keyState.IsKeyDown(interactKey) || mouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public override bool InteractPressed()
        {
            if (
                (keyState.IsKeyDown(interactKey) && prevKeyState.IsKeyUp(interactKey))
                ||
                (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
               )
                return true;
            else
                return false;
        }
        public override bool InteractReleased()
        {
            if (keyState.IsKeyUp(interactKey) && mouseState.LeftButton == ButtonState.Released)
                return true;
            else
                return false;
        }

        public override bool WeaponHeld()
        {
            if (keyState.IsKeyDown(itemAKey))
                return true;
            else
                return false;
        }

        public override bool WeaponPressed()
        {
            if (keyState.IsKeyDown(itemAKey) && prevKeyState.IsKeyUp(itemAKey))
                return true;
            else
                return false;
        }

        public override bool WeaponReleased()
        {
            if (keyState.IsKeyUp(itemAKey))
                return true;
            else
                return false;
        }

        public override bool ItemBHeld()
        {
            if (keyState.IsKeyDown(itemBKey))
                return true;
            else
                return false;
        }

        public override bool ItemBPressed()
        {
            if (keyState.IsKeyDown(itemBKey) && prevKeyState.IsKeyUp(itemBKey))
                return true;
            else
                return false;
        }

        public override bool ItemBReleased()
        {
            if (keyState.IsKeyUp(itemBKey))
                return true;
            else
                return false;
        }

        public override bool ItemCHeld()
        {
            if (keyState.IsKeyDown(itemCKey))
                return true;
            else
                return false;
        }

        public override bool ItemCPressed()
        {
            if (keyState.IsKeyDown(itemCKey) && prevKeyState.IsKeyUp(itemCKey))
                return true;
            else
                return false;
        }

        public override bool ItemCReleased()
        {
            if (keyState.IsKeyUp(itemCKey))
                return true;
            else
                return false;
        }

        public override bool BoostHeld()
        {
            if (keyState.IsKeyDown(boostKey))
                return true;
            else
                return false;
        }

        public override bool BoostPressed()
        {
            if(keyState.IsKeyDown(boostKey) && prevKeyState.IsKeyUp(boostKey))
                return true;
            else
                return false;
        }

        public override bool BoostReleased()
        {
            if (keyState.IsKeyUp(boostKey))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostHeld()
        {
            if (keyState.IsKeyDown(reverseBoostKey))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostPressed()
        {
            if (keyState.IsKeyDown(reverseBoostKey) && prevKeyState.IsKeyUp(reverseBoostKey))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostReleased()
        {
            if (keyState.IsKeyUp(reverseBoostKey))
                return true;
            else
                return false;
        }

        public bool LeftMousePress()
        {
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                return true;

            return false;
        }

        public bool LeftMouseHold()
        {
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Pressed)
                return true;

            return false;
        }

        public bool LeftMouseRelease()
        {
            if (mouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
                return true;

            return false;
        }

        public bool RightMousePress()
        {
            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
                return true;

            return false;
        }

        public bool RightMouseHold()
        {
            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Pressed)
                return true;

            return false;
        }

        public bool RightMouseRelease()
        {
            if (mouseState.RightButton == ButtonState.Released && prevMouseState.RightButton == ButtonState.Pressed)
                return true;

            return false;
        }

        public override bool LockOnPressed()
        {
            return RightMousePress();
        }

        /*public override bool StrafeHeld()
        {
            if (keyState.IsKeyDown(strafeKey) || mouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public override bool StrafePressed()
        {
            if (
                (keyState.IsKeyDown(strafeKey) && prevKeyState.IsKeyUp(strafeKey))
                ||
                (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released)
               )
                return true;
            else
                return false;
        }*/

        public override bool DownPressed()
        {
            if (keyState.IsKeyDown(altDownKey) && prevKeyState.IsKeyUp(altDownKey))
                return true;
            else
                return false;
        }

        public override bool LeftPressed()
        {
            if (keyState.IsKeyDown(altLeftKey) && prevKeyState.IsKeyUp(altLeftKey))
                return true;
            else
                return false;
        }

        public override bool RightPressed()
        {
            if (keyState.IsKeyDown(altRightKey) && prevKeyState.IsKeyUp(altRightKey))
                return true;
            else
                return false;
        }

        public override bool UpPressed()
        {
            if (keyState.IsKeyDown(altUpKey) && prevKeyState.IsKeyUp(altUpKey))
                return true;
            else
                return false;
        }

        public override float LStickXAxis()
        {
            float value = 0f;
            
            if (keyState.IsKeyDown(leftKey))
            {
                value -= 1f;
            }

            if (keyState.IsKeyDown(rightKey))
            {
                value += 1f;
            }

            if (keyState.IsKeyDown(walkKey))
            {
                value *= 0.3f;
            }

            return value;
        }

        public override float LStickYAxis()
        {
            float value = 0f;

            if (keyState.IsKeyDown(upKey))
            {
                value -= 1f;
            }

            if (keyState.IsKeyDown(downKey))
            {
                value += 1f;
            }

            if (keyState.IsKeyDown(walkKey))
            {
                value *= 0.3f;
            }

            return value;
        }

        public override float RStickXAxis()
        {
            float value = 0f;

            if (keyState.IsKeyDown(altLeftKey))
            {
                value -= 1f;
            }

            if (keyState.IsKeyDown(altRightKey))
            {
                value += 1f;
            }

            return value;
        }

        public override float RStickYAxis()
        {
            float value = 0f;

            if (keyState.IsKeyDown(altUpKey))
            {
                value -= 1f;
            }

            if (keyState.IsKeyDown(altDownKey))
            {
                value += 1f;
            }

            return value;
        }

        public override bool StartPressed()
        {
            if (keyState.IsKeyDown(startKey) && prevKeyState.IsKeyUp(startKey))
                return true;
            else
                return false;
        }

        public override bool SelectPressed()
        {
            if (keyState.IsKeyDown(selectKey) && prevKeyState.IsKeyUp(selectKey))
                return true;
            else
                return false;
        }

        public Point mousePosition()
        {
            return new Point(mouseState.X, mouseState.Y);
        }

        public bool DeletePressed()
        {
            if (keyState.IsKeyDown(Keys.Delete) && prevKeyState.IsKeyUp(Keys.Delete))
                return true;
            else
                return false;
        }

        public override bool WeaponScrollUp()
        {
            if (mouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue)
                return true;

            return false;
        }

        public override bool WeaponScrollDown()
        {
            if (mouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue)
                return true;

            return false;
        }

        
        
    }
}
