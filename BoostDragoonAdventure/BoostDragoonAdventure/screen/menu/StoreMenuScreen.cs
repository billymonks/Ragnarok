﻿using System;
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
    public class StoreMenuScreen : MenuScreen
    {

        int highlightedWeapon = -1, equippedWeapon = -1, lastHighlightedIndex = -1;

        private bool highlightChange = false;

        protected List<Item> storeItems;

        protected Dictionary<int, SpriterPlayer> storeSlotSpriters;
        protected Dictionary<int, Rectangle> storeBoxes;
        

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        protected TextEntity pageTitle, itemName, itemDesc;

        protected float margin = 2f;

        int height = 180, halfPageWidth = 720;

        //protected Rectangle weaponsBox, itemsBox, statusBox;

        public StoreMenuScreen(GameBase game, GameplayManager gm, Player p, List<Item> storeItems)
            : base(game, gm, p)
        {
            this.storeItems = storeItems;
            //Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            //backgroundColor = new Color(0.4f, 0.4f, 0f, 1f);

            storeSlotSpriters = new Dictionary<int, SpriterPlayer>();
            storeBoxes = new Dictionary<int, Rectangle>();

            //weaponList = p.getStats().inventory.getWeaponList();

            pageTitle = new TextEntity("SHOP", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            itemName = new TextEntity("", new Vector2(1080, height + 10), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemName.shadow = true;

            itemDesc = new TextEntity("", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemDesc.alignment = TextAlignment.Left;
            itemDesc.cueName = "Jump7";

            storeBoxes.Add(0, new Rectangle(20, 20 + height, 200, 200));
            storeBoxes.Add(1, new Rectangle(260, 20 + height, 200, 200));
            storeBoxes.Add(2, new Rectangle(500, 20 + height, 200, 200));
            storeBoxes.Add(3, new Rectangle(20, 260 + height, 200, 200));
            storeBoxes.Add(4, new Rectangle(260, 260 + height, 200, 200));
            storeBoxes.Add(5, new Rectangle(500, 260 + height, 200, 200));
            storeBoxes.Add(6, new Rectangle(20, 500 + height, 200, 200));
            storeBoxes.Add(7, new Rectangle(260, 500 + height, 200, 200));
            storeBoxes.Add(8, new Rectangle(500, 500 + height, 200, 200));

            descBox = new Rectangle(halfPageWidth + 20, 20 + height, 680, 680);

            storeSlotSpriters.Add(0, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(1, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(2, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(3, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(4, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(5, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(6, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(7, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            storeSlotSpriters.Add(8, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));

            storeSlotSpriters[0].setAnimation("unselected", 0, 0);
            storeSlotSpriters[1].setAnimation("unselected", 0, 0);
            storeSlotSpriters[2].setAnimation("unselected", 0, 0);
            storeSlotSpriters[3].setAnimation("unselected", 0, 0);
            storeSlotSpriters[4].setAnimation("unselected", 0, 0);
            storeSlotSpriters[5].setAnimation("unselected", 0, 0);
            storeSlotSpriters[6].setAnimation("unselected", 0, 0);
            storeSlotSpriters[7].setAnimation("unselected", 0, 0);
            storeSlotSpriters[8].setAnimation("unselected", 0, 0);

            descSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);
            descSpriter.setAnimation("unselected", 0, 0);

            foreach (KeyValuePair<int, SpriterPlayer> pair in storeSlotSpriters)
            {
                AddSpriter(pair.Value);
            }

            AddSpriter(descSpriter);

            AddText(pageTitle);
            AddText(itemName);
            AddText(itemDesc);
            
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

            for (int i = 0; i < storeItems.Count; i++)
            {
                //if (p.getStats().inventory.equippedWeapon == weaponList[i])
                //{
                    //equippedWeapon = i;
                //}
            }

            foreach (KeyValuePair<int, SpriterPlayer> pair in storeSlotSpriters)
            {
                //pair.Value.setAnimation("unselected", 0, 0);

                pair.Value.setAnimation("blank", 0, 0);

                if (pair.Key < storeItems.Count)
                    pair.Value.setAnimation("unselected", 0, 0);

                if (storeBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)) && pair.Key < storeItems.Count)
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
                pair.Value.update(storeBoxes[pair.Key].Center.X, -storeBoxes[pair.Key].Center.Y);
                pair.Value.SetDepth(0.06f);

            }

            descSpriter.setScale(6.8f);
            descSpriter.update(descBox.Center.X, -descBox.Center.Y);
            descSpriter.SetDepth(0.06f);

            
            UpdateStoreDesc();


            UpdateCursorPosition(p.c);

            if (p.c.LaunchMenuPressed() || p.c.SelectPressed() || p.c.ItemBPressed())
            {
                Dispose();
            }

            HandleClick();
            
        }

        private void UpdateStoreDesc()
        {
            if (highlightChange && lastHighlightedIndex > -1 && lastHighlightedIndex < storeItems.Count)
            {
                itemName.ChangeText(storeItems[lastHighlightedIndex].name);
                itemDesc.ChangeText(storeItems[lastHighlightedIndex].desc + "\nCost: " + storeItems[lastHighlightedIndex].value * margin, 600f);
                //itemDesc.SetMaxWidth(600f);
            }

            highlightChange = false;
        }

        private void HandleClick()
        {
            if (p.c.InteractPressed() || p.c.WeaponPressed())
            {
                if (highlightedWeapon > -1 && highlightedWeapon < storeItems.Count)
                {
                    if(p.getStats().inventory.currency >= storeItems[lastHighlightedIndex].value * margin)
                    {
                        p.getStats().inventory.addCurrency((int)(-storeItems[lastHighlightedIndex].value * margin));
                        p.getStats().inventory.receiveItem(storeItems[highlightedWeapon]);
                        _gm.factory.DisplayMessage("Thank you!");
                    }
                    else
                    {
                        _gm.factory.DisplayMessage("You can't afford this.");
                    }
                    
                }
            }
        }
    }
}
