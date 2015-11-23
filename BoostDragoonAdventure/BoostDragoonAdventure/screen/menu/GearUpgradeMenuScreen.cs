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
    public struct RectanglePart
    {

    }

    public class GearUpgradeMenuScreen : MenuScreen
    {

        //int highlightedItem = -1, lastHighlightedIndex = -1;

        int gearSize = 3;

        private bool highlightChange = false, listChange = false;

        protected List<Part> partList;

        protected Dictionary<Point, Rectangle> gearPanelBoxes;
        protected Dictionary<Point, SpriterPlayer> gearPanelSpriters;

        protected Dictionary<EquippedPart, Dictionary<Point, SpriterPlayer>> gearSpriters;

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        protected List<SpriterPlayer> partSpriters;

        protected TextEntity pageTitle, itemName, itemDesc;

        int height = 180, halfPageWidth = 720;

        //protected Rectangle weaponsBox, itemsBox, statusBox;

        public GearUpgradeMenuScreen(GameBase game, GameplayManager gm, Player p)
            : base(game, gm, p)
        {
            
            //Initialize(game);
        }

        public override void Initialize(wickedcrush.GameBase g)
        {
            base.Initialize(g);

            backgroundColor = new Color(0.4f, 0.4f, 0f, 1f);

            gearPanelBoxes = new Dictionary<Point, Rectangle>();
            gearPanelSpriters = new Dictionary<Point, SpriterPlayer>();

            gearSpriters = new Dictionary<EquippedPart, Dictionary<Point, SpriterPlayer>>();

            partList = p.getStats().inventory.GetPartList();

            partSpriters = new List<SpriterPlayer>();

            pageTitle = new TextEntity("UPGRADE", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            itemName = new TextEntity("", new Vector2(1080, height + 10), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);

            itemDesc = new TextEntity("", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemDesc.center = false;

            for (int i = -gearSize; i <= gearSize; i++)
            {
                for (int j = -gearSize; j <= gearSize; j++)
                {
                    gearPanelBoxes.Add(new Point(i, j), new Rectangle(980 + 100 * i, 540 + 100 * j, 100, 100));
                    gearPanelSpriters.Add(new Point(i, j), new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader));

                    gearPanelSpriters[new Point(i, j)].setScale(2f);

                    AddSpriter(gearPanelSpriters[new Point(i, j)]);
                }
            }

            Dictionary<Point, SpriterPlayer> tempGearSpriters = new Dictionary<Point, SpriterPlayer>();

            foreach (Point point in p.getStats().inventory.gear.core.GetEquippedSlots())
            {
                tempGearSpriters.Add(point, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader));
                tempGearSpriters[point].setScale(2f);
                tempGearSpriters[point].setAnimation("grey", 0, 0);

                AddSpriter(tempGearSpriters[point]);
            }
            gearSpriters.Add(p.getStats().inventory.gear.core, tempGearSpriters);

            foreach (EquippedPart equippedPart in p.getStats().inventory.gear.parts)
            {
                tempGearSpriters = new Dictionary<Point, SpriterPlayer>();

                foreach (Point point in equippedPart.GetEquippedSlots())
                {
                    tempGearSpriters.Add(point, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader));
                    tempGearSpriters[point].setScale(2f);
                    tempGearSpriters[point].setAnimation("grey", 0, 0);

                    AddSpriter(tempGearSpriters[point]);
                }
                gearSpriters.Add(equippedPart, tempGearSpriters);
            }



            descBox = new Rectangle(halfPageWidth + 20, 20 + height, 680, 680);

            descSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);
            descSpriter.setAnimation("unselected", 0, 0);

            //foreach (KeyValuePair<int, SpriterPlayer> pair in inventorySlotSpriters)
            //{
            //AddSpriter(pair.Value);
            //}

            //AddSpriter(descSpriter);

            AddText(pageTitle);
            //AddText(itemName);
            //AddText(itemDesc);

        }

        public override void Dispose()
        {
            ClearText();
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //highlightedItem = -1;

            if (listChange)
            {
                partList = p.getStats().inventory.GetPartList();
            }

            foreach (KeyValuePair<Point, SpriterPlayer> panel in gearPanelSpriters)
            {
                panel.Value.SetDepth(0.2f);
                panel.Value.update(gearPanelBoxes[panel.Key].Center.X, -gearPanelBoxes[panel.Key].Center.Y);
            }

            
            foreach (KeyValuePair<EquippedPart, Dictionary<Point, SpriterPlayer>> partPair in gearSpriters)
            {
                foreach (KeyValuePair<Point, SpriterPlayer> spritePair in partPair.Value)
                {
                    spritePair.Value.SetDepth(0.1f);
                    spritePair.Value.update(gearPanelBoxes[spritePair.Key].Center.X, -gearPanelBoxes[spritePair.Key].Center.Y);
                }
            }

            /*foreach (KeyValuePair<int, SpriterPlayer> pair in inventorySlotSpriters)
            {
                pair.Value.setAnimation("blank", 0, 0);

                if (pair.Key < partList.Count)
                    pair.Value.setAnimation("unselected", 0, 0);

                if (inventoryBoxes[pair.Key].Contains(Helper.CastToPoint(cursorPos)) && pair.Key < partList.Count)
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
            }*/

            

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

            //descSpriter.setScale(6.8f);
            //descSpriter.update(descBox.Center.X, -descBox.Center.Y);
            //descSpriter.SetDepth(0.3f);


            //UpdateItemDesc();


            if (p.c is KeyboardControls)
                UpdateCursorPosition((KeyboardControls)p.c);

            if (p.c.LaunchMenuPressed())
            {
                Dispose();
            }

            //HandleClick();
            
        }

        private void UpdateItemDesc()
        {
            //if (highlightChange && lastHighlightedIndex > -1 && lastHighlightedIndex < partList.Count)
            //{
                //itemName.text = partList[lastHighlightedIndex].name;
                //itemDesc.text = partList[lastHighlightedIndex].desc;
                //itemDesc.SetMaxWidth(600f);

                //UpdatePartDisplay();
            //}

            //highlightChange = false;
        }

        /*private void UpdatePartDisplay()
        {
            Part selectedPart = partList[lastHighlightedIndex];
            SpriterPlayer tempSpriter;
            Point partDisplayPos = new Point(1155, 540);

            foreach (SpriterPlayer p in partSpriters)
            {
                RemoveSpriter(p);
            }

            partSpriters.Clear();

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
        }*/

        private void HandleClick()
        {
            if (p.c.InteractPressed())
            {
                

                
            }
        }
    }
}
