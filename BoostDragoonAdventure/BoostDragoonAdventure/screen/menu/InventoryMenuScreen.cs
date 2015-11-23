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
    public enum ItemType
    {
        Consumable = 0,
        Part = 1
    }

    public class InventoryMenuScreen : MenuScreen
    {

        int highlightedItem = -1, lastHighlightedIndex = -1;

        private bool highlightChange = false, listChange = false;

        protected ItemType displayType;

        protected List<Consumable> consumableList;

        protected Dictionary<int, SpriterPlayer> inventorySlotSpriters;
        protected Dictionary<int, Rectangle> inventoryBoxes;
        

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        protected TextEntity pageTitle, itemName, itemDesc;

        int height = 180, halfPageWidth = 720;

        //protected Rectangle weaponsBox, itemsBox, statusBox;

        public InventoryMenuScreen(GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            
            //Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            backgroundColor = new Color(0.4f, 0.4f, 0f, 1f);

            displayType = ItemType.Consumable;

            inventorySlotSpriters = new Dictionary<int, SpriterPlayer>();
            inventoryBoxes = new Dictionary<int, Rectangle>();

            consumableList = p.getStats().inventory.GetConsumableList();

            pageTitle = new TextEntity("ITEMS", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            itemName = new TextEntity("", new Vector2(1080, height + 10), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);

            itemDesc = new TextEntity("", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemDesc.center = false;

            inventoryBoxes.Add(0, new Rectangle(20, 20 + height, 200, 200));
            inventoryBoxes.Add(1, new Rectangle(260, 20 + height, 200, 200));
            inventoryBoxes.Add(2, new Rectangle(500, 20 + height, 200, 200));
            inventoryBoxes.Add(3, new Rectangle(20, 260 + height, 200, 200));
            inventoryBoxes.Add(4, new Rectangle(260, 260 + height, 200, 200));
            inventoryBoxes.Add(5, new Rectangle(500, 260 + height, 200, 200));
            inventoryBoxes.Add(6, new Rectangle(20, 500 + height, 200, 200));
            inventoryBoxes.Add(7, new Rectangle(260, 500 + height, 200, 200));
            inventoryBoxes.Add(8, new Rectangle(500, 500 + height, 200, 200));

            descBox = new Rectangle(halfPageWidth + 20, 20 + height, 680, 680);

            inventorySlotSpriters.Add(0, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(1, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(2, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(3, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(4, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(5, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(6, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(7, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));
            inventorySlotSpriters.Add(8, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));

            inventorySlotSpriters[0].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[1].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[2].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[3].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[4].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[5].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[6].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[7].setAnimation("unselected", 0, 0);
            inventorySlotSpriters[8].setAnimation("unselected", 0, 0);

            descSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);
            descSpriter.setAnimation("unselected", 0, 0);

            foreach (KeyValuePair<int, SpriterPlayer> pair in inventorySlotSpriters)
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

            highlightedItem = -1;

            if (listChange)
            {
                consumableList = p.getStats().inventory.GetConsumableList();
            }

            foreach (KeyValuePair<int, SpriterPlayer> pair in inventorySlotSpriters)
            {
                pair.Value.setAnimation("blank", 0, 0);

                if(pair.Key < consumableList.Count)
                    pair.Value.setAnimation("unselected", 0, 0);

                if (inventoryBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)) && pair.Key < consumableList.Count)
                {
                    highlightedItem = pair.Key;
                    if (highlightedItem == lastHighlightedIndex)
                    {
                        lastHighlightedIndex = highlightedItem;
                        highlightChange = false;
                    }
                    else
                    {
                        lastHighlightedIndex = highlightedItem;
                        highlightChange = true;
                    }

                    pair.Value.setAnimation("selected", 0, 0);
                }

                pair.Value.setScale(2f);
                pair.Value.update(inventoryBoxes[pair.Key].Center.X, -inventoryBoxes[pair.Key].Center.Y);
                pair.Value.SetDepth(0.06f);
            }

            

            //for (int i = 0; i < weaponList.Count; i++)
            //{
                //if (p.getStats().inventory.equippedWeapon == weaponList[i])
                //{
                    //equippedWeapon = i;
                //}
            //}

            /*foreach (KeyValuePair<int, SpriterPlayer> pair in weaponSlotSpriters)
            {
                pair.Value.setAnimation("unselected", 0, 0);

                if (weaponBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)))
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

            }*/

            descSpriter.setScale(6.8f);
            descSpriter.update(descBox.Center.X, -descBox.Center.Y);
            descSpriter.SetDepth(0.06f);


            UpdateItemDesc();


            if (p.c is KeyboardControls)
                UpdateCursorPosition((KeyboardControls)p.c);

            if (p.c.LaunchMenuPressed())
            {
                Dispose();
            }

            HandleClick();
            
        }

        private void UpdateItemDesc()
        {
            if (highlightChange && lastHighlightedIndex > -1 && lastHighlightedIndex < consumableList.Count)
            {
                itemName.text = consumableList[lastHighlightedIndex].name;
                itemDesc.text = consumableList[lastHighlightedIndex].desc;
                itemDesc.SetMaxWidth(600f);
            }

            highlightChange = false;
        }

        private void HandleClick()
        {
            if (p.c.InteractPressed())
            {
                if (displayType == ItemType.Consumable)
                {
                    //show consumables

                    if (highlightedItem > -1 && highlightedItem < consumableList.Count)
                    {
                        //p.getStats().inventory.equippedWeapon = weaponList[highlightedWeapon];
                        //equippedWeapon = highlightedWeapon;
                        consumableList[highlightedItem].Use(p.getAgent());

                        if (p.getStats().inventory.getItemCount(consumableList[highlightedItem]) <= 0)
                        {
                            highlightedItem = -1;
                            lastHighlightedIndex = -1;
                            listChange = true;

                            itemName.text = "";
                            itemDesc.text = "";
                        }
                    }

                }
            }
        }
    }
}
