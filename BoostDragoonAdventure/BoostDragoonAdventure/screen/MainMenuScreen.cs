using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.gameplay;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Input;
using wickedcrush.helper;
using wickedcrush.player;
using wickedcrush.utility.config;

namespace wickedcrush.screen.menu
{
    public class MainMenuScreen : GameScreen
    {

        int selectedOption = -1;

        protected SpriterPlayer newGameSpriter, loadGameSpriter, optionsSpriter;

        protected Rectangle newGameBox, loadGameBox, optionsBox;

        SpriterPlayer crosshairSpriter;

        public Vector2 cursorPos = new Vector2(720, 540);
        public Vector2 cursorPosition = Vector2.Zero;
        public Vector2 prevCursorPosition = Vector2.Zero;
        public Vector2 scaledCursorPosition = Vector2.Zero;

        protected Color backgroundColor = new Color(0f, 0f, 0f, 1f);

        protected float marginSize;

        public Controls c;

        public MainMenuScreen(GameBase game)
        {
            Initialize(game);
            //backgroundColor = new Color(0f, 0f, 0f, 1f);
            
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            marginSize = (g.screenManager.fov - 1.3333333f) * 540f;

            crosshairSpriter = new SpriterPlayer(g.spriterManager.spriters["cursor"].getSpriterData(), 0, g.spriterManager.spriters["cursor"].loader);
            crosshairSpriter.setAnimation("crosshair", 0, 0);

            AddSpriter(crosshairSpriter);

            newGameBox = new Rectangle(320, 440, 200, 200);
            loadGameBox = new Rectangle(620, 440, 200, 200);
            optionsBox = new Rectangle(920, 440, 200, 200);


            newGameSpriter = new SpriterPlayer(g.spriterManager.spriters["hud"].getSpriterData(), 2, g.spriterManager.spriters["hud"].loader);
            newGameSpriter.setAnimation("unselected", 0, 0);

            loadGameSpriter = new SpriterPlayer(g.spriterManager.spriters["hud"].getSpriterData(), 3, g.spriterManager.spriters["hud"].loader);
            loadGameSpriter.setAnimation("unselected", 0, 0);

            optionsSpriter = new SpriterPlayer(g.spriterManager.spriters["hud"].getSpriterData(), 4, g.spriterManager.spriters["hud"].loader);
            optionsSpriter.setAnimation("unselected", 0, 0);



            AddSpriter(newGameSpriter);
            AddSpriter(loadGameSpriter);
            AddSpriter(optionsSpriter);

            
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            crosshairSpriter.setScale(2f);
            crosshairSpriter.update(cursorPos.X, -cursorPos.Y);
            crosshairSpriter.SetDepth(0.03f);

            newGameSpriter.setScale(2f);
            newGameSpriter.update(420, -540);
            newGameSpriter.SetDepth(0.06f);
            
            selectedOption = -1;

            if (newGameBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                newGameSpriter.setAnimation("selected", 0, 0);
                selectedOption = 0;
            }
            else
            {
                newGameSpriter.setAnimation("unselected", 0, 0);
            }




            loadGameSpriter.setScale(2f);
            loadGameSpriter.update(720, -540);
            loadGameSpriter.SetDepth(0.06f);

            if (loadGameBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                loadGameSpriter.setAnimation("selected", 0, 0);
                selectedOption = 1;
            }
            else
            {
                loadGameSpriter.setAnimation("unselected", 0, 0);
            }



            optionsSpriter.setScale(2f);
            optionsSpriter.update(1020, -540);
            optionsSpriter.SetDepth(0.06f);

            if (optionsBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                optionsSpriter.setAnimation("selected", 0, 0);
                selectedOption = 2;
            }
            else
            {
                optionsSpriter.setAnimation("unselected", 0, 0);
            }



                //UpdateCursorPosition(p.c);

            //if (p.c.LaunchMenuPressed() || p.c.ItemBPressed())
            //{
                //Dispose();
            //}

            HandleClick();
            
        }

        public void UpdateCursorPosition(Controls c)
        {
            if (c is KeyboardControls && game.settings.controlMode == ControlMode.MouseAndKeyboard)
            {
                prevCursorPosition.X = game.GraphicsDevice.Viewport.Width / 2;
                prevCursorPosition.Y = game.GraphicsDevice.Viewport.Height / 2;

                cursorPosition.X = ((KeyboardControls)c).mousePosition().X;
                cursorPosition.Y = ((KeyboardControls)c).mousePosition().Y;
            }
            else
            {
                prevCursorPosition = Vector2.Zero;
                cursorPosition.X = c.LStickXAxis() * game.settings.mouseSensitivity * 3;
                cursorPosition.Y = c.LStickYAxis() * game.settings.mouseSensitivity * 3;
            }

            cursorPos += (cursorPosition - prevCursorPosition) * new Vector2(1f, (float)(2.0 / Math.Sqrt(2))) * game.settings.mouseSensitivity;

            ConstrainCursorPos();

            game.diag = "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "CursorPos: " + cursorPos.X + ", " + cursorPos.Y + "\n";
        }

        private void ConstrainCursorPos()
        {
            if (cursorPos.X < 0 - (game.screenManager.fov - 1.3333333f) * 540f)
            {
                cursorPos.X = 0 - (game.screenManager.fov - 1.3333333f) * 540f;
            }
            else if (cursorPos.X > 1440f + (game.screenManager.fov - 1.3333333f) * 540f)
            {
                cursorPos.X = 1440f + (game.screenManager.fov - 1.3333333f) * 540f;
            }

            if (cursorPos.Y < 0)
            {
                cursorPos.Y = 0;
            }
            else if (cursorPos.Y > 1080)
            {
                cursorPos.Y = 1080;
            }

            if (game.IsActive)
                Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);

        }

        private void HandleClick()
        {
            //if (p.c.BoostPressed())
            //{
                //game.screenManager.AddScreen(new GearUpgradeMenuScreen(game, _gm, p));
            //}
            //if (p.c.InteractPressed() || (game.settings.controlMode == utility.config.ControlMode.KeyboardOnly && p.c.WeaponPressed()))
            //{
                switch (selectedOption)
                {
                    case 0: //new
                        //game.screenManager.AddScreen(new WeaponSelectMenuScreen(game, _gm, p));
                        break;
                    case 1: //load
                        //game.screenManager.AddScreen(new InventoryMenuScreen(game, _gm, p));
                        break;
                    case 2: //options
                        //game.screenManager.AddScreen(new GearUpgradeMenuScreen(game, _gm, p));
                        break;
                }
            //}
        }
    }
}
