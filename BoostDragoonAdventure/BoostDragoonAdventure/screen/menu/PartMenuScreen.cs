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

    public class PartMenuScreen : MenuScreen
    {

        int highlightedItem = -1, lastHighlightedIndex = -1;

        private bool highlightChange = false, listChange = false;

        protected ItemType displayType;

        protected List<Part> partList;
        protected List<Consumable> consumableList;

        protected List<Item> itemList;

        protected Dictionary<int, SpriterPlayer> inventorySlotSpriters;
        protected Dictionary<int, Rectangle> inventoryBoxes;
        

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        protected List<SpriterPlayer> partSpriters;

        protected TextEntity pageTitle, itemName, itemDesc;

        int height = 180, halfPageWidth = 720;

        //protected Rectangle weaponsBox, itemsBox, statusBox;

        public PartMenuScreen(GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            
            //Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            backgroundColor = new Color(0.4f, 0.4f, 0f, 1f);

            displayType = ItemType.Part;

            inventorySlotSpriters = new Dictionary<int, SpriterPlayer>();
            inventoryBoxes = new Dictionary<int, Rectangle>();

            partList = p.getStats().inventory.GetPartList();
            consumableList = p.getStats().inventory.GetConsumableList();

            itemList = new List<Item>();
            foreach (Consumable consumable in consumableList)
            {
                itemList.Add(consumable);
            }
            foreach (Part part in partList)
            {
                itemList.Add(part);
            }
            


            partSpriters = new List<SpriterPlayer>();

            pageTitle = new TextEntity("PARTS", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            itemName = new TextEntity("", new Vector2(1080, height + 10), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);

            itemDesc = new TextEntity("", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemDesc.center = false;

            for (int i = 0; i < 36; i++)
            {
                int x = i % 6;
                int y = i / 6;

                inventoryBoxes.Add(i, new Rectangle(20 + x * 120, 20 + height + y * 120, 100, 100));

                inventorySlotSpriters.Add(i, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader));

                inventorySlotSpriters[i].setAnimation("unselected", 0, 0);


            }

            descBox = new Rectangle(halfPageWidth + 20, 20 + height, 680, 680);

            /*inventoryBoxes.Add(0, new Rectangle(20, 20 + height, 200, 200));
            inventoryBoxes.Add(1, new Rectangle(260, 20 + height, 200, 200));
            inventoryBoxes.Add(2, new Rectangle(500, 20 + height, 200, 200));
            inventoryBoxes.Add(3, new Rectangle(20, 260 + height, 200, 200));
            inventoryBoxes.Add(4, new Rectangle(260, 260 + height, 200, 200));
            inventoryBoxes.Add(5, new Rectangle(500, 260 + height, 200, 200));
            inventoryBoxes.Add(6, new Rectangle(20, 500 + height, 200, 200));
            inventoryBoxes.Add(7, new Rectangle(260, 500 + height, 200, 200));
            inventoryBoxes.Add(8, new Rectangle(500, 500 + height, 200, 200));

            

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
            inventorySlotSpriters[8].setAnimation("unselected", 0, 0);*/

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
                partList = p.getStats().inventory.GetPartList();
                consumableList = p.getStats().inventory.GetConsumableList();

                itemList.Clear();
                foreach (Consumable consumable in consumableList)
                {
                    itemList.Add(consumable);
                }
                foreach (Part part in partList)
                {
                    itemList.Add(part);
                }
            }

            foreach (KeyValuePair<int, SpriterPlayer> pair in inventorySlotSpriters)
            {
                pair.Value.setAnimation("blank", 0, 0);

                if (pair.Key < itemList.Count)
                    pair.Value.setAnimation("unselected", 0, 0);

                if (inventoryBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)) && pair.Key < itemList.Count)
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

                pair.Value.setScale(1f);
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
            descSpriter.SetDepth(0.3f);


            UpdateItemDesc();


                UpdateCursorPosition(p.c);

                if (p.c.LaunchMenuPressed() || p.c.ItemBPressed())
            {
                Dispose();
            }

            HandleClick();
            
        }

        private void UpdateItemDesc()
        {
            if (highlightChange && lastHighlightedIndex > -1 && lastHighlightedIndex < itemList.Count)
            {
                itemName.text = itemList[lastHighlightedIndex].name;
                itemDesc.text = itemList[lastHighlightedIndex].desc;
                itemDesc.SetMaxWidth(600f);

                if (itemList[lastHighlightedIndex] is Part)
                {
                    UpdatePartDisplay();
                }
                else
                {
                    ClearPartDisplay();
                }
            }

            highlightChange = false;
        }

        private void ClearPartDisplay()
        {
            foreach (SpriterPlayer p in partSpriters)
            {
                RemoveSpriter(p);
            }

            partSpriters.Clear();
        }

        private void UpdatePartDisplay()
        {
            Part selectedPart = (Part)itemList[lastHighlightedIndex];
            SpriterPlayer tempSpriter;
            Point partDisplayPos = new Point(1155, 540);

            ClearPartDisplay();

            foreach (Point p in selectedPart.partStruct.slots)
            {
                tempSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader);
                tempSpriter.setAnimation("light", 0, 0);
                tempSpriter.update(partDisplayPos.X + (p.X * 50), -partDisplayPos.Y - (p.Y * 50));
                tempSpriter.SetDepth(0.2f);
                partSpriters.Add(tempSpriter);
                AddSpriter(tempSpriter);
            }

            foreach (PartConnection c in selectedPart.partStruct.connections)
            {
                int entNum = 9;

                Point cRotation = new Point((int) Math.Round(Math.Cos(MathHelper.ToRadians(c.direction))), (int)Math.Round(Math.Sin(MathHelper.ToRadians(c.direction))));

                switch(c.type)
                {
                    case ConnectionType.Circle:
                        entNum = 9;
                        break;

                    case ConnectionType.Square:
                        entNum = 10;
                        break;

                    case ConnectionType.Triangle:
                        entNum = 11;
                        break;
                }
                
                tempSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), entNum, _gm.factory._spriterManager.spriters["hud"].loader);

                if(c.female)
                {
                    tempSpriter.setAnimation("inside", 0, 0);
                } else {
                    tempSpriter.setAnimation("outside", 0, 0);
                }

                tempSpriter.SetDepth(0.1f);

                tempSpriter.update(partDisplayPos.X + (c.slot.X * 50) + (cRotation.X * 25), -partDisplayPos.Y - (c.slot.Y * 50) - (cRotation.Y * 25));

                partSpriters.Add(tempSpriter);
                AddSpriter(tempSpriter);
                
            }
        }

        private void HandleClick()
        {
            if (p.c.InteractPressed() || (game.settings.controlMode == utility.config.ControlMode.KeyboardOnly && p.c.WeaponPressed()))
            {
                    //show consumables

                    if (highlightedItem > -1 && highlightedItem < itemList.Count)
                    {
                        //p.getStats().inventory.equippedWeapon = weaponList[highlightedWeapon];
                        //equippedWeapon = highlightedWeapon;
                        //partList[highlightedItem].Use(p.getAgent());

                        if (itemList[highlightedItem] is Consumable)
                        {
                            ((Consumable)itemList[highlightedItem]).Use(p.getAgent());
                        }

                        if (p.getStats().inventory.getItemCount(itemList[highlightedItem]) <= 0)
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
