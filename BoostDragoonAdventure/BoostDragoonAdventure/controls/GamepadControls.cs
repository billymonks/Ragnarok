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

        private Buttons actionButton = Buttons.X, blockButton = Buttons.B, boostButton = Buttons.A;

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

        public override bool DownPressed()
        {
            if (padState.ThumbSticks.Left.Y < -0.2f && !(prevPadState.ThumbSticks.Left.Y < -0.2f))
                return true;
            else
                return false;
        }

        public override bool LeftPressed()
        {
            if (padState.ThumbSticks.Left.X < 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
                return true;
            else
                return false;
        }

        public override bool RightPressed()
        {
            if (padState.ThumbSticks.Left.X > 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
                return true;
            else
                return false;
        }

        public override bool UpPressed()
        {
            if (padState.ThumbSticks.Left.Y > 0.2f && !(prevPadState.ThumbSticks.Left.Y > 0.2f))
                return true;
            else
                return false;
        }

        public override float LStickXAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Left.X) < 0.2f)
                return 0f;
            else
                return padState.ThumbSticks.Left.X;
        }

        public override float LStickYAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Left.Y) < 0.2f)
                return 0f;
            else
                return -padState.ThumbSticks.Left.Y;
        }

        public override float RStickXAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Right.X) < 0.2f)
                return 0f;
            else
                return padState.ThumbSticks.Right.X;
        }

        public override float RStickYAxis()
        {
            if (Math.Abs(padState.ThumbSticks.Right.Y) < 0.2f)
                return 0f;
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
