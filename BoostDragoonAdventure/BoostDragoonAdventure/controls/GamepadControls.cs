using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace wickedcrush.controls
{
    public class GamepadControls : Controls
    {
        public PlayerIndex playerIndex;

        private GamePadState padState, prevPadState;

        //classic controls
        private Buttons interactButton = Buttons.A, itemAButton = Buttons.X, itemBButton = Buttons.Y, itemCButton = Buttons.B, boostButton = Buttons.RightTrigger, reverseBoostButton = Buttons.LeftTrigger, lockOnButton = Buttons.RightStick, itemScrollUp = Buttons.RightShoulder, itemScrollDown = Buttons.LeftShoulder;

        //twinstick controls
        //private Buttons interactButton = Buttons.LeftShoulder, itemAButton = Buttons.RightShoulder, itemBButton = Buttons.Y, itemCButton = Buttons.B, boostButton = Buttons.RightTrigger, reverseBoostButton = Buttons.LeftTrigger, lockOnButton = Buttons.RightStick, itemScrollUp = Buttons.A, itemScrollDown = Buttons.X;
        
        public GamepadControls(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

            keyState = new KeyboardState();
            prevKeyState = keyState;

            padState = new GamePadState();
            prevPadState = padState;
        }

        public override void Update()
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            prevPadState = padState;
            padState = GamePad.GetState(playerIndex);
        }

        public override bool InteractHeld()
        {
            if (padState.IsButtonDown(interactButton))
                return true;
            else
                return false;
        }

        public override bool InteractPressed()
        {
            if (padState.IsButtonDown(interactButton) && prevPadState.IsButtonUp(interactButton))
                return true;
            else
                return false;
        }
        public override bool InteractReleased()
        {
            if (padState.IsButtonUp(interactButton))
                return true;
            else
                return false;
        }

        public override bool WeaponHeld()
        {
            if (padState.IsButtonDown(itemAButton))
                return true;
            else
                return false;
        }

        public override bool WeaponPressed()
        {
            if (padState.IsButtonDown(itemAButton) && prevPadState.IsButtonUp(itemAButton))
                return true;
            else
                return false;
        }

        public override bool WeaponReleased()
        {
            if (padState.IsButtonUp(itemAButton))
                return true;
            else
                return false;
        }

        public override bool ItemBHeld()
        {
            if (padState.IsButtonDown(itemBButton))
                return true;
            else
                return false;
        }

        public override bool ItemBPressed()
        {
            if (padState.IsButtonDown(itemBButton) && prevPadState.IsButtonUp(itemBButton))
                return true;
            else
                return false;
        }

        public override bool ItemBReleased()
        {
            if (padState.IsButtonUp(itemBButton))
                return true;
            else
                return false;
        }

        public override bool ItemCHeld()
        {
            if (padState.IsButtonDown(itemCButton))
                return true;
            else
                return false;
        }

        public override bool ItemCPressed()
        {
            if (padState.IsButtonDown(itemCButton) && prevPadState.IsButtonUp(itemCButton))
                return true;
            else
                return false;
        }

        public override bool ItemCReleased()
        {
            if (padState.IsButtonUp(itemCButton))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostHeld()
        {
            if (padState.IsButtonDown(reverseBoostButton))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostPressed()
        {
            if (padState.IsButtonDown(reverseBoostButton) && prevPadState.IsButtonUp(reverseBoostButton))
                return true;
            else
                return false;
        }

        public override bool ReverseBoostReleased()
        {
            if (padState.IsButtonUp(reverseBoostButton))
                return true;
            else
                return false;
        }

        public override bool BoostHeld()
        {
            if (padState.IsButtonDown(boostButton))
                return true;
            else
                return false;
        }

        public override bool BoostPressed()
        {
            if (padState.IsButtonDown(boostButton) && prevPadState.IsButtonUp(boostButton))
                return true;
            else
                return false;
        }

        public override bool BoostReleased()
        {
            if (padState.IsButtonUp(boostButton))
                return true;
            else
                return false;
        }

        /*public override bool StrafeHeld()
        {
            if (padState.IsButtonDown(strafeButton))
                return true;
            else
                return false;
        }

        public override bool StrafePressed()
        {
            if (padState.IsButtonDown(strafeButton) && prevPadState.IsButtonUp(strafeButton))
                return true;
            else
                return false;
        }*/

        public override bool DownPressed()
        {
            //if (padState.ThumbSticks.Left.Y < -0.2f && !(prevPadState.ThumbSticks.Left.Y < -0.2f))
            if(padState.IsButtonDown(Buttons.DPadDown) && prevPadState.IsButtonUp(Buttons.DPadDown))
                return true;
            else
                return false;
        }

        public override bool LeftPressed()
        {
            //if (padState.ThumbSticks.Left.X < 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
            if (padState.IsButtonDown(Buttons.DPadLeft) && prevPadState.IsButtonUp(Buttons.DPadLeft))
                return true;
            else
                return false;
        }

        public override bool RightPressed()
        {
            //if (padState.ThumbSticks.Left.X > 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
            if (padState.IsButtonDown(Buttons.DPadRight) && prevPadState.IsButtonUp(Buttons.DPadRight))
                return true;
            else
                return false;
        }

        public override bool UpPressed()
        {
            //if (padState.ThumbSticks.Left.Y > 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
            if (padState.IsButtonDown(Buttons.DPadUp) && prevPadState.IsButtonUp(Buttons.DPadUp))
                return true;
            else
                return false;
        }

        public override float LStickXAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Left.X) < 0.2f)
                return 0f;
            else if (padState.ThumbSticks.Left.X < -0.75f)
                return -1f;
            else if (padState.ThumbSticks.Left.X > 0.75f)
                return 1f;
            else
                return padState.ThumbSticks.Left.X;
        }

        public override float LStickYAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Left.Y) < 0.2f)
                return 0f;
            else if (padState.ThumbSticks.Left.Y < -0.75f)
                return 1f;
            else if (padState.ThumbSticks.Left.Y > 0.75f)
                return -1f;
            else
                return -padState.ThumbSticks.Left.Y;
        }

        public override float RStickXAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Right.X) < 0.2f)
                return 0f;
            else if (padState.ThumbSticks.Right.X < -0.75f)
                return -1f;
            else if (padState.ThumbSticks.Right.X > 0.75f)
                return 1f;
            else
                return padState.ThumbSticks.Right.X;
        }

        public override float RStickYAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Right.Y) < 0.2f)
                return 0f;
            else if (padState.ThumbSticks.Right.Y < -0.75f)
                return -1f;
            else if (padState.ThumbSticks.Right.Y > 0.75f)
                return 1f;
            else
                return padState.ThumbSticks.Right.Y;
        }

        public override bool StartPressed()
        {
            if (padState.IsButtonDown(Buttons.Start) && prevPadState.IsButtonUp(Buttons.Start))
                return true;
            else
                return false;
        }

        public override bool SelectPressed()
        {
            if (padState.IsButtonDown(Buttons.Back) && prevPadState.IsButtonUp(Buttons.Back) || (keyState.IsKeyDown(Keys.Escape) && prevKeyState.IsKeyUp(Keys.Escape)))
                return true;
            else
                return false;
        }

        public override bool LockOnPressed()
        {
            if (padState.IsButtonDown(lockOnButton) && prevPadState.IsButtonUp(lockOnButton))
            {
                return true;
            }

            return false;
        }

        public override bool WeaponScrollUp()
        {
            if (padState.IsButtonDown(itemScrollUp) && prevPadState.IsButtonUp(itemScrollUp))
            {
                return true;
            }

            return false;
        }

        public override bool WeaponScrollDown()
        {
            if (padState.IsButtonDown(itemScrollDown) && prevPadState.IsButtonUp(itemScrollDown))
            {
                return true;
            }

            return false;
        }
        
    }
}
