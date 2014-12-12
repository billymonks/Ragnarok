using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using wickedcrush.controls;


namespace wickedcrush.manager.controls
{
    public class ControlsManager : Microsoft.Xna.Framework.GameComponent
    {
        private List<Controls> controlsList = new List<Controls>();
        private List<Controls> removeList = new List<Controls>();

        public Boolean joinAllowed;

        public DebugControls debugControls;

        public ControlsManager(Game game)
            : base(game)
        {
            Initialize();
        }
        
        public override void Initialize()
        {
            joinAllowed = true;

            debugControls = new DebugControls();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            updateControls();

            base.Update(gameTime);
        }

        private void updateControls()
        {
            foreach (Controls c in controlsList)
            {
                c.Update();

                if (c.remove)
                    removeList.Add(c);
            }

            debugControls.Update();
            
            performRemoval();
        }

        public bool NewGamepadPressed()
        {
            if (!joinAllowed)
                return false;

            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++) //thanks microsoft
            {
                if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed
                    && !padInList(index))
                {
                    return true;
                }
            }
            return false;
        }

        public bool NewKeyboardPressed()
        {
            if (!joinAllowed)
                return false;

            if (Keyboard.GetState().IsKeyDown(Keys.Enter)
                && !keyboardInList())
            {
                return true;
            }
            
            return false;
        }

        public Controls checkAndAddGamepads()
        {
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++) //thanks microsoft
            {
                if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed
                    && !padInList(index))
                {
                    return addGamepad(index);
                }
            }

            throw new NotImplementedException();
        }

        private bool padInList(PlayerIndex p)
        {
            foreach (Controls c in controlsList)
            {
                if (c is GamepadControls 
                    && ((GamepadControls)c).playerIndex.Equals(p))
                {
                    return true;
                }
            }
            return false;
        }

        private bool keyboardInList()
        {
            foreach (Controls c in controlsList)
            {
                if (c is KeyboardControls)
                {
                    return true;
                }
            }
            return false;
        }
        
        public Controls addGamepad(PlayerIndex i)
        {
            Controls c = new GamepadControls(i);
            
            controlsList.Add(c);

            return c;
        }

        public Controls addKeyboard()
        {
            Controls c = new KeyboardControls();

            controlsList.Add(c);

            return c;
        }

        public KeyboardControls getKeyboard()
        {
            foreach(Controls c in controlsList)
            {
                if (c is KeyboardControls)
                    return ((KeyboardControls)c);
            }
            return new KeyboardControls();
            //throw new NotImplementedException("getkeyboard no get keyboard...NO KEYBOARD D:");
        }

        public void removeGamepad(PlayerIndex index)
        {
            foreach (Controls c in controlsList)
            {
                if (c is GamepadControls 
                    && ((GamepadControls)c).playerIndex.Equals(index))
                {
                    removeList.Add(c);
                }
            }
        }

        public void removeKeyboard()
        {
            foreach (Controls c in controlsList)
            {
                if (c is KeyboardControls)
                {
                    removeList.Add(c);
                }
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Controls c in removeList)
                {
                    controlsList.Remove(c);
                }
                removeList.Clear();
            }
        }

        public String getDiag()
        {
            String diag = "Controller Order\n";
            int playerNumber = 1;

            foreach (Controls c in controlsList)
            {
                diag += playerNumber + " ";
                if (c is GamepadControls)
                {
                    diag += "Valiant Gamepad: " + ((GamepadControls)c).playerIndex + "\n";
                }
                if (c is KeyboardControls)
                {
                    diag += "Trusty Keyboard+Mouse\n";
                }
                playerNumber++;
            }

            return diag;
        }

        public bool SelectPressed()
        {
            foreach (Controls c in controlsList)
            {
                if (c.SelectPressed())
                    return true;
            }
            return false;
        }

        public bool StartPressed()
        {
            foreach (Controls c in controlsList)
            {
                if (c.StartPressed())
                    return true;
            }
            return false;
        }
    }
}
