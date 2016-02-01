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
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.player;
using wickedcrush.inventory;

namespace wickedcrush.screen.menu
{
    public class WeaponSelectMenuScreen : MenuScreen
    {

        int highlightedWeapon = -1, equippedWeapon = -1, lastHighlightedIndex = -1;

        private bool highlightChange = false;

        protected List<Weapon> weaponList;

        protected Dictionary<int, SpriterPlayer> weaponSlotSpriters;
        protected Dictionary<int, Rectangle> weaponBoxes;
        

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        protected TextEntity pageTitle, weaponName, weaponDesc;

        int height = 180, halfPageWidth = 720;

        //protected Rectangle weaponsBox, itemsBox, statusBox;

        public WeaponSelectMenuScreen(GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            
            //Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            backgroundColor = new Color(0.4f, 0.4f, 0f, 1f);

            weaponSlotSpriters = new Dictionary<int, SpriterPlayer>();
            weaponBoxes = new Dictionary<int, Rectangle>();

            weaponList = p.getStats().inventory.getWeaponList();

            pageTitle = new TextEntity("WEAPONS", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            weaponName = new TextEntity("", new Vector2(1080, height + 10), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);

            weaponDesc = new TextEntity("", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            weaponDesc.alignment = TextAlignment.Left;

            weaponBoxes.Add(0, new Rectangle(20, 20 + height, 200, 200));
            weaponBoxes.Add(1, new Rectangle(260, 20 + height, 200, 200));
            weaponBoxes.Add(2, new Rectangle(500, 20 + height, 200, 200));
            weaponBoxes.Add(3, new Rectangle(20, 260 + height, 200, 200));
            weaponBoxes.Add(4, new Rectangle(260, 260 + height, 200, 200));
            weaponBoxes.Add(5, new Rectangle(500, 260 + height, 200, 200));
            weaponBoxes.Add(6, new Rectangle(20, 500 + height, 200, 200));
            weaponBoxes.Add(7, new Rectangle(260, 500 + height, 200, 200));
            weaponBoxes.Add(8, new Rectangle(500, 500 + height, 200, 200));

            descBox = new Rectangle(halfPageWidth + 20, 20 + height, 680, 680);

            weaponSlotSpriters.Add(0, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(1, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(2, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(3, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(4, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(5, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(6, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(7, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            weaponSlotSpriters.Add(8, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));

            weaponSlotSpriters[0].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[1].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[2].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[3].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[4].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[5].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[6].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[7].setAnimation("unselected", 0, 0);
            weaponSlotSpriters[8].setAnimation("unselected", 0, 0);

            descSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);
            descSpriter.setAnimation("unselected", 0, 0);

            foreach (KeyValuePair<int, SpriterPlayer> pair in weaponSlotSpriters)
            {
                AddSpriter(pair.Value);
            }

            AddSpriter(descSpriter);

            AddText(pageTitle);
            AddText(weaponName);
            AddText(weaponDesc);
            
        }

        public override void Dispose()
        {
            ClearText();
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            highlightedWeapon = -1;
            equippedWeapon = -1;

            for (int i = 0; i < weaponList.Count; i++)
            {
                if (p.getStats().inventory.equippedWeapon == weaponList[i])
                {
                    equippedWeapon = i;
                }
            }

            foreach (KeyValuePair<int, SpriterPlayer> pair in weaponSlotSpriters)
            {
                //pair.Value.setAnimation("unselected", 0, 0);

                pair.Value.setAnimation("blank", 0, 0);

                if (pair.Key < weaponList.Count)
                    pair.Value.setAnimation("unselected", 0, 0);

                if (weaponBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)) && pair.Key < weaponList.Count)
                {
                    highlightedWeapon = pair.Key;
                    lastHighlightedIndex = highlightedWeapon;
                    highlightChange = true;

                    pair.Value.setAnimation("selected", 0, 0);
                }

                if (equippedWeapon == pair.Key)
                {
                    pair.Value.setAnimation("selected", 0, 0);
                }

                pair.Value.setScale(2f);
                pair.Value.update(weaponBoxes[pair.Key].Center.X, -weaponBoxes[pair.Key].Center.Y);
                pair.Value.SetDepth(0.06f);

            }

            descSpriter.setScale(6.8f);
            descSpriter.update(descBox.Center.X, -descBox.Center.Y);
            descSpriter.SetDepth(0.06f);

            
            UpdateWeaponDesc();


            UpdateCursorPosition(p.c);

            if (p.c.LaunchMenuPressed() || p.c.ItemBPressed())
            {
                Dispose();
            }

            HandleClick();
            
        }

        private void UpdateWeaponDesc()
        {
            if (highlightChange && lastHighlightedIndex > -1 && lastHighlightedIndex < weaponList.Count)
            {
                weaponName.text = weaponList[lastHighlightedIndex].name;
                weaponDesc.text = weaponList[lastHighlightedIndex].desc;
                weaponDesc.SetMaxWidth(600f);
            }

            highlightChange = false;
        }

        private void HandleClick()
        {
            if (p.c.InteractPressed() || p.c.WeaponPressed())
            {
                if (highlightedWeapon > -1 && highlightedWeapon < weaponList.Count)
                {
                    p.getStats().inventory.equippedWeapon = weaponList[highlightedWeapon];
                    equippedWeapon = highlightedWeapon;
                }
            }
        }
    }
}
