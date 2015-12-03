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
using wickedcrush.stats;

namespace wickedcrush.screen.menu
{
    public enum GearUpgradeMenuState
    {
        ConnectionSelect = 0,
        PartSelection = 1
    }

    public class GearUpgradeMenuScreen : MenuScreen
    {

        //int highlightedItem = -1, lastHighlightedIndex = -1;

        int gearSize = 3;

        private bool highlightChange = false, gearChange = false, partSelectHighlightChange = false;

        public GearUpgradeMenuState state = GearUpgradeMenuState.ConnectionSelect;

        protected List<Part> partList;

        protected Dictionary<Point, Rectangle> gearPanelBoxes;
        protected Dictionary<Point, SpriterPlayer> gearPanelSpriters;

        protected Dictionary<EquippedPart, Dictionary<Point, SpriterPlayer>> gearSlotSpriters;

        protected Dictionary<EquippedConnection, Rectangle> gearConnectionBoxes;
        protected Dictionary<EquippedConnection, SpriterPlayer> gearConnectionSpriters;

        protected Dictionary<Point, SpriterPlayer> previewSlotSpriters;

        protected SpriterPlayer descSpriter;
        protected Rectangle descBox;

        TextEntity compatiblePartsText;
        
        protected List<SpriterPlayer> partSpriters;

        protected int partSelectionInt = -1, prevPartSelectionInt = -1;
        protected List<Rectangle> partSelectionBoxes;
        protected List<TextEntity> partSelectionText;

        protected TextEntity pageTitle, partStatsPartSelectionText, itemDesc;

        protected EquippedPart activePart;
        protected EquippedConnection activeConnection;

        int height = 180, halfPageWidth = 720;
        Point targetPanelLeftLocation = new Point(330, 540);
        Point targetPanelRightLocation = new Point(980, 540);
        Point targetPanelLocation = new Point(980, 540);
        Point panelLocation = new Point(330, 540);

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

            previewSlotSpriters = new Dictionary<Point, SpriterPlayer>();

            gearSlotSpriters = new Dictionary<EquippedPart, Dictionary<Point, SpriterPlayer>>();

            gearConnectionBoxes = new Dictionary<EquippedConnection, Rectangle>();
            gearConnectionSpriters = new Dictionary<EquippedConnection, SpriterPlayer>();

            
            
            partList = p.getStats().inventory.GetPartList();
            partSelectionText = new List<TextEntity>();
            partSelectionBoxes = new List<Rectangle>();

            partSpriters = new List<SpriterPlayer>();

            pageTitle = new TextEntity("EQUIP", new Vector2(320, height / 2), _gm.factory._sm, game, -1, _gm.factory, Color.White, 2f, "Rubik Mono One", false);

            partStatsPartSelectionText = new TextEntity("", new Vector2(20, 840), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            partStatsPartSelectionText.center = false;

            itemDesc = new TextEntity("desc", new Vector2(780, height + 40), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
            itemDesc.center = false;

            descBox = new Rectangle(20, 20 + height, 680, 680);

            descSpriter = new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 5, _gm.factory._spriterManager.spriters["hud"].loader);
            descSpriter.setAnimation("unselected", 0, 0);

            compatiblePartsText = new TextEntity("Compatible Parts:", new Vector2(20, height), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);

            InitializeGear();

            AddText(pageTitle);
            //AddText(itemName);
            AddText(itemDesc);

        }

        private void UpdateGearPanelPos()
        {
            panelLocation = new Point((panelLocation.X + targetPanelLocation.X) / 2, (panelLocation.Y + targetPanelLocation.Y) / 2);

            for (int i = -gearSize; i <= gearSize; i++)
            {
                for (int j = -gearSize; j <= gearSize; j++)
                {
                    gearPanelBoxes[new Point(i, j)] = new Rectangle(panelLocation.X + 100 * i, panelLocation.Y + 100 * j, 100, 100);
                }
            }

            Dictionary<EquippedConnection, Rectangle> tempList = new Dictionary<EquippedConnection, Rectangle>();

            tempList.Clear();

            foreach (KeyValuePair<EquippedConnection, Rectangle> pair in gearConnectionBoxes)
            {
                tempList.Add(pair.Key, new Rectangle(panelLocation.X + pair.Key.translation.X * 100 + 25 + pair.Key.cRotation.X * 50, panelLocation.Y + pair.Key.translation.Y * 100 + 25 + pair.Key.cRotation.Y * 50, 50, 50));
            }

            gearConnectionBoxes.Clear();

            foreach (KeyValuePair<EquippedConnection, Rectangle> pair in tempList)
            {
                gearConnectionBoxes.Add(pair.Key, pair.Value);
            }

            tempList.Clear();
        }

        private void InitializeGear()
        {
            for (int i = -gearSize; i <= gearSize; i++)
            {
                for (int j = -gearSize; j <= gearSize; j++)
                {
                    gearPanelBoxes.Add(new Point(i, j), new Rectangle(panelLocation.X + 100 * i, panelLocation.Y + 100 * j, 100, 100));
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
            gearSlotSpriters.Add(p.getStats().inventory.gear.core, tempGearSpriters);

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
                gearSlotSpriters.Add(equippedPart, tempGearSpriters);

                foreach (EquippedConnection equipConnect in equippedPart.equippedConnections)
                {
                    InitializeConnectionSpriter(equipConnect);
                }


            }
            foreach (EquippedConnection equipConnect in p.getStats().inventory.gear.core.equippedConnections)
            {
                InitializeConnectionSpriter(equipConnect);
            }
        }

        private void ResetGear()
        {
            Dictionary<Point, SpriterPlayer> tempGearSpriters = new Dictionary<Point, SpriterPlayer>();

            foreach (KeyValuePair<EquippedPart, Dictionary<Point, SpriterPlayer>> gearSlotPair in gearSlotSpriters)
            {
                foreach (KeyValuePair<Point, SpriterPlayer> pair in gearSlotPair.Value)
                {
                    RemoveSpriter(pair.Value);
                }
            }

            foreach (KeyValuePair<EquippedConnection, SpriterPlayer> pair in gearConnectionSpriters)
            {
                RemoveSpriter(pair.Value);
            }

            gearSlotSpriters.Clear();
            gearConnectionBoxes.Clear();
            gearConnectionSpriters.Clear();

            foreach (Point point in p.getStats().inventory.gear.core.GetEquippedSlots())
            {
                tempGearSpriters.Add(point, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader));
                tempGearSpriters[point].setScale(2f);
                tempGearSpriters[point].setAnimation("grey", 0, 0);

                AddSpriter(tempGearSpriters[point]);
            }
            gearSlotSpriters.Add(p.getStats().inventory.gear.core, tempGearSpriters);

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
                gearSlotSpriters.Add(equippedPart, tempGearSpriters);

                foreach (EquippedConnection equipConnect in equippedPart.equippedConnections)
                {
                    InitializeConnectionSpriter(equipConnect);
                }


            }
            foreach (EquippedConnection equipConnect in p.getStats().inventory.gear.core.equippedConnections)
            {
                InitializeConnectionSpriter(equipConnect);
            }
        }



        private void InitializeConnectionSpriter(EquippedConnection equipConnect)
        {
            int connectType = 9;

            switch (equipConnect.connection.type)
            {
                case ConnectionType.Circle:
                    connectType = 9;
                    break;
                case ConnectionType.Square:
                    connectType = 10;
                    break;
                case ConnectionType.Triangle:
                    connectType = 11;
                    break;
            }

            if (equipConnect.connection.female)
            {

                gearConnectionBoxes.Add(equipConnect, new Rectangle(panelLocation.X + equipConnect.translation.X * 100 + 25 + equipConnect.cRotation.X * 50, panelLocation.Y + equipConnect.translation.Y * 100 + 25 + equipConnect.cRotation.Y * 50, 50, 50));
                gearConnectionSpriters.Add(equipConnect, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), connectType, _gm.factory._spriterManager.spriters["hud"].loader));

                gearConnectionSpriters[equipConnect].setScale(2f);

                AddSpriter(gearConnectionSpriters[equipConnect]);

            }
        }

        public override void Dispose()
        {
            ClearText();
            game.screenManager.RemoveScreen(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool clearHighlight = true;
            bool clearConnectionHighlight = true;

            if (gearChange)
            {
                ResetGear();
                p.getStats().ApplyStats();
                gearChange = false;
            }

            UpdateGearPanelPos();

            //highlightedItem = -1;

            //if (listChange)
            //{
                //partList = p.getStats().inventory.GetPartList();
            //}

            foreach (KeyValuePair<Point, SpriterPlayer> panel in gearPanelSpriters)
            {
                panel.Value.SetDepth(0.2f);
                panel.Value.update(gearPanelBoxes[panel.Key].Center.X, -gearPanelBoxes[panel.Key].Center.Y);
            }

            if (state == GearUpgradeMenuState.ConnectionSelect)
            {
                targetPanelLocation = targetPanelLeftLocation;
                foreach (KeyValuePair<EquippedPart, Dictionary<Point, SpriterPlayer>> partPair in gearSlotSpriters)
                {
                    bool highlightPart = false;
                
                    if (partPair.Key == activePart)
                        highlightPart = true;

                

                    foreach (KeyValuePair<Point, SpriterPlayer> spritePair in partPair.Value)
                    {
                        spritePair.Value.SetDepth(0.1f);
                        spritePair.Value.update(gearPanelBoxes[spritePair.Key].Center.X, -gearPanelBoxes[spritePair.Key].Center.Y);

                        if (gearPanelBoxes[spritePair.Key].Contains(Helper.CastToPoint(cursorPos)))
                        {
                            activePart = partPair.Key;
                            clearHighlight = false;
                        }

                        if (highlightPart)
                        {
                            spritePair.Value.setAnimation("light", 0, 0);
                        }
                        else
                        {
                            spritePair.Value.setAnimation("grey", 0, 0);
                        }
                    }
                }

                foreach (KeyValuePair<EquippedConnection, SpriterPlayer> connectPair in gearConnectionSpriters)
                {
                    connectPair.Value.SetDepth(0.05f);
                    connectPair.Value.update(gearConnectionBoxes[connectPair.Key].Center.X, -gearConnectionBoxes[connectPair.Key].Center.Y);

                    

                    if (connectPair.Key.child == null)
                    {

                        if (gearConnectionBoxes[connectPair.Key].Contains(Helper.CastToPoint(cursorPos)))
                        {
                            activePart = connectPair.Key.parent;
                            activeConnection = connectPair.Key;
                            clearHighlight = false;
                            clearConnectionHighlight = false;
                            connectPair.Value.setAnimation("inside_grey", 0, 0);
                        }
                        else
                        {
                            connectPair.Value.setAnimation("inside", 0, 0);
                        }

                    }
                    else
                    {
                        connectPair.Value.setAnimation("filled", 0, 0);
                    }
                }

                if (clearHighlight)
                {
                    activeConnection = null;
                    activePart = null;
                }

                if (clearConnectionHighlight)
                {
                    activeConnection = null;
                }
            } else if (state == GearUpgradeMenuState.PartSelection)
            {
                targetPanelLocation = targetPanelRightLocation;
                foreach (KeyValuePair<EquippedPart, Dictionary<Point, SpriterPlayer>> partPair in gearSlotSpriters)
                {
                    foreach (KeyValuePair<Point, SpriterPlayer> spritePair in partPair.Value)
                    {
                        spritePair.Value.SetDepth(0.1f);
                        spritePair.Value.update(gearPanelBoxes[spritePair.Key].Center.X, -gearPanelBoxes[spritePair.Key].Center.Y);
                    }

                    foreach (KeyValuePair<EquippedConnection, SpriterPlayer> connectPair in gearConnectionSpriters)
                    {
                        connectPair.Value.SetDepth(0.05f);
                        connectPair.Value.update(gearConnectionBoxes[connectPair.Key].Center.X, -gearConnectionBoxes[connectPair.Key].Center.Y);
                    }
                }

                foreach (KeyValuePair<Point, SpriterPlayer> previewPair in previewSlotSpriters)
                {
                    previewPair.Value.SetDepth(0.1f);
                    previewPair.Value.update(gearPanelBoxes[previewPair.Key].Center.X, -gearPanelBoxes[previewPair.Key].Center.Y);
                }

                prevPartSelectionInt = partSelectionInt;
                partSelectionInt = -1;
                for (int i = 0; i < partSelectionBoxes.Count; i++)
                {
                    if (partSelectionBoxes[i].Contains(Helper.CastToPoint(cursorPos)))
                    {
                        partSelectionInt = i;
                        partSelectionText[i].textColor = Color.LightGoldenrodYellow;
                    }
                    else
                    {
                        partSelectionText[i].textColor = Color.Black;
                    }
                }

                if (partSelectionInt != prevPartSelectionInt)
                {
                    partSelectHighlightChange = true;
                }
            }


            UpdateItemDesc();

            HandleInput();
            
        }

        private void UpdateItemDesc()
        {
            
            if (partSelectHighlightChange && state == GearUpgradeMenuState.PartSelection)
            {
                foreach (KeyValuePair<Point, SpriterPlayer> pair in previewSlotSpriters)
                {
                    RemoveSpriter(pair.Value);
                }
                previewSlotSpriters.Clear();

                if (partSelectionInt != -1)
                {
                    EquippedPart previewPart = p.getStats().inventory.gear.GetPreviewPart(partList[partSelectionInt], activeConnection);
                    foreach (Point point in previewPart.GetEquippedSlots())
                    {
                        previewSlotSpriters.Add(point, new SpriterPlayer(_gm.factory._spriterManager.spriters["hud"].getSpriterData(), 7, _gm.factory._spriterManager.spriters["hud"].loader));
                        previewSlotSpriters[point].setScale(2f);
                        previewSlotSpriters[point].setAnimation("light", 0, 0);

                        AddSpriter(previewSlotSpriters[point]);
                    }

                    partStatsPartSelectionText.text = "";
                    foreach (KeyValuePair<GearStat, int> statPair in previewPart.part.partStruct.stats)
                    {
                        partStatsPartSelectionText.text += statPair.Key + " +" + statPair.Value + "\n";
                    }
                }

            }
            else if (state == GearUpgradeMenuState.ConnectionSelect)
            {
                if (activePart == null)
                {
                    itemDesc.text = "";
                    foreach (KeyValuePair<GearStat, int> pair in p.getStats().inventory.gear.GetEquippedStats())
                    {
                        itemDesc.text = itemDesc.text + pair.Key + " +" + pair.Value + "\n";
                    }
                }
                else
                {
                    itemDesc.text = activePart.part.name + "\n"; ;
                    foreach (KeyValuePair<GearStat, int> pair in activePart.part.partStruct.stats)
                    {
                        itemDesc.text = itemDesc.text + pair.Key + " +" + pair.Value + "\n";
                    }
                }
            }
        }

        private void LaunchPartSelectionMenu()
        {
            state = GearUpgradeMenuState.PartSelection;

            partList = p.getStats().inventory.GetCompatibleParts(activeConnection);

            
            compatiblePartsText.center = false;
            //partSelectionText.Add(compatiblePartsText);
            AddText(compatiblePartsText);

            for (int i = 0; i < partList.Count; i++)
            {
                partSelectionBoxes.Add(new Rectangle(20, height + 60 + i * 60, 500, 60));
                TextEntity tempText = new TextEntity(partList[i].name + " x" + p.getStats().inventory.getItemCount(partList[i]), new Vector2(partSelectionBoxes[i].X, partSelectionBoxes[i].Top), _gm.factory._sm, game, -1, _gm.factory, Color.White, 1f, "Khula", false);
                tempText.center = false;
                partSelectionText.Add(tempText);

                AddText(tempText);
            }

            partStatsPartSelectionText.text = "";

            AddText(partStatsPartSelectionText);
        }

        private void HandleInput()
        {
            if (state == GearUpgradeMenuState.ConnectionSelect)
            {
                if (p.c.InteractPressed())
                {
                    if (activeConnection != null)
                    {
                        LaunchPartSelectionMenu();
                        RemoveText(itemDesc);

                    } else if (activePart != null)
                    {
                        if (activePart != p.getStats().inventory.gear.core)
                        {
                            p.getStats().inventory.UnequipPart(activePart);
                            gearChange = true;
                        }
                    }
                }

                if (p.c.LaunchMenuPressed())
                {
                    Dispose();
                }
            }
            else if (state == GearUpgradeMenuState.PartSelection)
            {
                if (p.c.InteractPressed())
                {
                    if (partSelectionInt > -1 && partSelectionInt < partList.Count)
                    {
                        
                        p.getStats().inventory.EquipPart(partList[partSelectionInt], activeConnection);
                        gearChange = true;
                        ClosePartSelectionMenu();
                    } 
                }

                if (p.c.LaunchMenuPressed())
                {
                    ClosePartSelectionMenu();
                }
            }

            if (p.c is KeyboardControls)
                UpdateCursorPosition((KeyboardControls)p.c);
        }

        private void ClosePartSelectionMenu()
        {
            foreach (KeyValuePair<Point, SpriterPlayer> pair in previewSlotSpriters)
            {
                RemoveSpriter(pair.Value);
            }
            previewSlotSpriters.Clear();

            foreach (TextEntity t in partSelectionText)
            {
                RemoveText(t);
            }

            RemoveText(compatiblePartsText);
            RemoveText(partStatsPartSelectionText);

            partSelectionText.Clear();
            partSelectionBoxes.Clear();
            state = GearUpgradeMenuState.ConnectionSelect;
            AddText(itemDesc);
        }
    }
}
