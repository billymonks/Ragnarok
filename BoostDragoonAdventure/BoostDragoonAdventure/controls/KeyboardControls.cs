﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace wickedcrush.controls
{
    public class KeyboardControls : Controls
    {
        private KeyboardState keyState, prevKeyState;
        private MouseState mouseState, prevMouseState;

        private Keys actionKey = Keys.Z, blockKey = Keys.X, boostKey = Keys.Space,
            strafeKey = Keys.LeftControl, downKey = Keys.S, upKey = Keys.W, leftKey = Keys.A, rightKey = Keys.D,
            altDownKey = Keys.Down, altUpKey = Keys.Up, altLeftKey = Keys.Left, altRightKey = Keys.Right,
            startKey = Keys.Enter, selectKey = Keys.Escape, walkKey = Keys.LeftShift;


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

        public override bool ActionHeld()
        {
            if (keyState.IsKeyDown(actionKey) || mouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public override bool ActionPressed()
        {
            if (
                (keyState.IsKeyDown(actionKey) && prevKeyState.IsKeyUp(actionKey))
                || 
                (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
               )
                return true;
            else
                return false;
        }
        public override bool ActionReleased()
        {
            if (keyState.IsKeyUp(actionKey) || mouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public override bool BlockHeld()
        {
            if (keyState.IsKeyDown(blockKey))
                return true;
            else
                return false;
        }

        public override bool BlockPressed()
        {
            if (keyState.IsKeyDown(blockKey) && prevKeyState.IsKeyUp(blockKey))
                return true;
            else
                return false;
        }

        public override bool BlockReleased()
        {
            if (keyState.IsKeyUp(blockKey))
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

        public override bool StrafeHeld()
        {
            if (keyState.IsKeyDown(strafeKey) || mouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

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
        
    }
}
