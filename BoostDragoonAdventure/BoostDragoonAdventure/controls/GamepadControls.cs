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

        private Buttons interactButton = Buttons.A, actionButton = Buttons.X, blockButton = Buttons.B, boostButton = Buttons.RightTrigger, strafeButton = Buttons.LeftTrigger;

        public GamepadControls(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

            padState = new GamePadState();
            prevPadState = padState;
        }

        public override void Update()
        {
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

        public override bool ActionHeld()
        {
            if (padState.IsButtonDown(actionButton))
                return true;
            else
                return false;
        }

        public override bool ActionPressed()
        {
            if (padState.IsButtonDown(actionButton) && prevPadState.IsButtonUp(actionButton))
                return true;
            else
                return false;
        }
        public override bool ActionReleased()
        {
            if (padState.IsButtonUp(actionButton))
                return true;
            else
                return false;
        }

        public override bool BlockHeld()
        {
            if (padState.IsButtonDown(blockButton))
                return true;
            else
                return false;
        }

        public override bool BlockPressed()
        {
            if (padState.IsButtonDown(blockButton) && prevPadState.IsButtonUp(blockButton))
                return true;
            else
                return false;
        }

        public override bool BlockReleased()
        {
            if (padState.IsButtonUp(blockButton))
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

        public override bool StrafeHeld()
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
        }

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
            if (padState.IsButtonDown(Buttons.Back) && prevPadState.IsButtonUp(Buttons.Back))
                return true;
            else
                return false;
        }
        
    }
}
