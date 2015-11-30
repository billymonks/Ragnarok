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

namespace wickedcrush.screen.menu
{
    public class GameplayMenuScreen : MenuScreen
    {

        int selectedOption = -1;

        protected SpriterPlayer weaponsSpriter, itemsSpriter, statusSpriter;

        protected Rectangle weaponsBox, itemsBox, statusBox;

        public GameplayMenuScreen(GameBase game, GameplayManager gm, Player p) : base(game, gm, p)
        {
            backgroundColor = new Color(0f, 0f, 0f, 0.5f);
            
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            weaponsBox = new Rectangle(320, 440, 200, 200);
            itemsBox = new Rectangle(620, 440, 200, 200);
            statusBox = new Rectangle(920, 440, 200, 200);


            weaponsSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 2, _gm.factory._spriterManager.spriters["hud"].loader);
            weaponsSpriter.setAnimation("unselected", 0, 0);

            itemsSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 3, _gm.factory._spriterManager.spriters["hud"].loader);
            itemsSpriter.setAnimation("unselected", 0, 0);

            statusSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 4, _gm.factory._spriterManager.spriters["hud"].loader);
            statusSpriter.setAnimation("unselected", 0, 0);

            

            AddSpriter(weaponsSpriter);
            AddSpriter(itemsSpriter);
            AddSpriter(statusSpriter);

            
        }

        public override void Dispose()
        {
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            weaponsSpriter.setScale(2f);
            weaponsSpriter.update(420,-540);
            weaponsSpriter.SetDepth(0.06f);
            
            selectedOption = -1;

            if (weaponsBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                weaponsSpriter.setAnimation("selected", 0, 0);
                selectedOption = 0;
            }
            else
            {
                weaponsSpriter.setAnimation("unselected", 0, 0);
            }

            
            

            itemsSpriter.setScale(2f);
            itemsSpriter.update(720, -540);
            itemsSpriter.SetDepth(0.06f);

            if (itemsBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                itemsSpriter.setAnimation("selected", 0, 0);
                selectedOption = 1;
            }
            else
            {
                itemsSpriter.setAnimation("unselected", 0, 0);
            }

            

            statusSpriter.setScale(2f);
            statusSpriter.update(1020, -540);
            statusSpriter.SetDepth(0.06f);

            if (statusBox.Contains(Helper.CastToPoint(cursorPos)))
            {
                statusSpriter.setAnimation("selected", 0, 0);
                selectedOption = 2;
            }
            else
            {
                statusSpriter.setAnimation("unselected", 0, 0);
            }



            if (p.c is KeyboardControls)
                UpdateCursorPosition((KeyboardControls)p.c);

            if (p.c.LaunchMenuPressed())
            {
                Dispose();
            }

            HandleClick();
            
        }

        private void HandleClick()
        {
            //if (p.c.BoostPressed())
            //{
                //game.screenManager.AddScreen(new GearUpgradeMenuScreen(game, _gm, p));
            //}
            if (p.c.InteractPressed())
            {
                switch (selectedOption)
                {
                    case 0: //weapons
                        game.screenManager.AddScreen(new WeaponSelectMenuScreen(game, _gm, p));
                        break;
                    case 1: //items
                        game.screenManager.AddScreen(new InventoryMenuScreen(game, _gm, p));
                        break;
                    case 2: //parts
                        game.screenManager.AddScreen(new GearUpgradeMenuScreen(game, _gm, p));
                        break;
                }
            }
        }
    }
}
