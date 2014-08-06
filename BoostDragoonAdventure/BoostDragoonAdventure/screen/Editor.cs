using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity;
using wickedcrush.map.layer;
using wickedcrush.map;
using wickedcrush.editor;
using wickedcrush.player;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.editor.tool;
using wickedcrush.menu.editor;
using wickedcrush.display.sprite;
using wickedcrush.factory.sprite;
using wickedcrush.display.primitives;
using wickedcrush.factory.editor;

namespace wickedcrush.screen
{
    public class Editor : GameScreen
    {
        SpriteFactory sf;

        public EditorMap map;
        public Point mapOffset;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        private Texture2D cursorTexture;

        private EditorTool tool;

        private EditorMenu menu;

        private bool toolReady = false;

        private Dictionary<String, BaseSprite> hud = new Dictionary<String, BaseSprite>();

        private EditorEntityFactory factory;

        private KeyboardControls keyboardControls;

        public Editor(Game game)
        {
            this.game = game;

            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            map = new EditorMap(game.mapName);
            mapOffset = new Point(0, 0);

            factory = new EditorEntityFactory(map, map.manager);

            cursorPosition = new Vector2();
            scaledCursorPosition = new Vector2();

            tool = new TerrainTool(LayerType.WALL);

            InitializeEditorMenu();

            cursorTexture = game.Content.Load<Texture2D>("debugcontent/img/nice_cursor");


        }

        private void InitializeEditorMenu()
        {
            Dictionary<string, MenuNode> nodes = new Dictionary<string, MenuNode>();

            MenuElement wallNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Wall", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new TerrainTool(LayerType.WALL));

            MenuElement deathSoupNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Death Soup", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new TerrainTool(LayerType.DEATHSOUP));


            MenuElement wiringNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Wiring", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new TerrainTool(LayerType.WIRING));

            

            wallNode.next = deathSoupNode;
            deathSoupNode.prev = wallNode;
            deathSoupNode.next = wiringNode;
            wiringNode.prev = deathSoupNode;

            MenuElement selectorNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Selector", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new SelectorTool(factory, map.manager));

            SubMenu terrainMenuNode = new SubMenu(
                sf.createText(new Vector2(0f, 0f), "Terrain", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                wallNode);

            wallNode.parent = terrainMenuNode;
            deathSoupNode.parent = terrainMenuNode;
            wiringNode.parent = terrainMenuNode;

            MenuElement chestNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Chest", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new EntityTool(factory.LoadEntity("CHEST", Vector2.Zero, Direction.East), factory));

            MenuElement turretNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Turret", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new EntityTool(factory.LoadEntity("TURRET", Vector2.Zero, Direction.East), factory));

            MenuElement potNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Pot", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new EntityTool(factory.LoadEntity("POT", Vector2.Zero, Direction.East), factory));

            MenuElement floorSwitchNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Floor Switch", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new EntityTool(factory.LoadEntity("FLOOR_SWITCH", Vector2.Zero, Direction.East), factory));

            MenuElement timerNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Timer", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                new EntityTool(factory.LoadEntity("TIMER", Vector2.Zero, Direction.East), factory));


            SubMenu entityMenuNode = new SubMenu(
                sf.createText(new Vector2(0f, 0f), "Entities", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                chestNode);

            chestNode.parent = entityMenuNode;
            turretNode.parent = entityMenuNode;
            potNode.parent = entityMenuNode;
            floorSwitchNode.parent = entityMenuNode;
            timerNode.parent = entityMenuNode;

            selectorNode.next = terrainMenuNode;
            terrainMenuNode.prev = selectorNode;
            terrainMenuNode.next = entityMenuNode;
            entityMenuNode.prev = terrainMenuNode;

            chestNode.next = turretNode;
            turretNode.prev = chestNode;
            turretNode.next = potNode;
            potNode.prev = turretNode;
            potNode.next = floorSwitchNode;
            floorSwitchNode.prev = potNode;
            floorSwitchNode.next = timerNode;
            timerNode.prev = floorSwitchNode;

            nodes.Add("Wall", wallNode);
            nodes.Add("Death Soup", deathSoupNode);
            nodes.Add("Wiring", wiringNode);
            
            nodes.Add("Chest", chestNode);
            nodes.Add("Turret", turretNode);
            nodes.Add("Pot", potNode);
            nodes.Add("Floor Switch", floorSwitchNode);
            nodes.Add("Timer", timerNode);

            nodes.Add("Selector", selectorNode);
            nodes.Add("Terrain", terrainMenuNode);
            nodes.Add("Entities", entityMenuNode);

            menu = new EditorMenu(nodes);

            //menu.current = (node);

        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
            menu.Update(gameTime, cursorPosition);
            DebugControls(gameTime);
            map.Update(gameTime);
        }

        public override void DebugDraw()
        {
            Direction d;
            if (factory.preview != null)
                d = factory.preview.angle;
            else
                d = Direction.East;

            map.DebugDraw(game.whiteTexture, game.arrowTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, mapOffset);

            if (menu.currentTool() != null && menu.currentTool().getMode() == EditorMode.Entity)
            {
                EditorEntity tempEntity = ((EntityTool)menu.currentTool()).getEntity(scaledCursorPosition, d);
                Color temp;

                if (factory.CanPlace(((EntityTool)menu.currentTool()).getEntity(scaledCursorPosition, d).code,
                    scaledCursorPosition,
                    d))
                    temp = Color.Purple;
                else
                    temp = Color.Red;

                tempEntity.DebugDraw(game.whiteTexture, game.arrowTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, temp);
            }

            if (tool != null)
                tool.Draw(game.whiteTexture, game.arrowTexture, game.GraphicsDevice, game.spriteBatch, game.testFont);
        }

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);
            
            foreach (KeyValuePair<String, BaseSprite> s in hud)
                s.Value.Draw(game.spriteBatch);

            DrawMenu();
            DrawDiag();
            DrawCursor();

            game.spriteBatch.End();
        }

        public override void Dispose()
        {

        }

        private void DebugControls(GameTime gameTime)
        {
            foreach (Player p in game.playerManager.getPlayerList()) //move these foreach to playermanager, create methods that use all players
            {
                if (p.c.SelectPressed())
                {
                    Dispose();
                    game.screenStack.Pop();
                    return;
                }

                if (p.c is KeyboardControls)
                {
                    UpdateCursorPosition((KeyboardControls)p.c);

                    if (((KeyboardControls)p.c).ActionReleased())
                    {
                        toolReady = true;
                    }

                    if (menu.highlighted != null)
                    {
                        toolReady = false;
                    }

                    if(tool != null)
                        tool.Update(gameTime, (KeyboardControls)p.c, scaledCursorPosition, map, toolReady);

                    if (((KeyboardControls)p.c).ActionPressed())
                    {
                        menu.Click();
                        tool = menu.currentTool();
                    }
                }
            }
        }

        private void UpdateCursorPosition(KeyboardControls c)
        {
            cursorPosition.X = c.mousePosition().X;
            cursorPosition.Y = c.mousePosition().Y;

            scaledCursorPosition.X = c.mousePosition().X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
            scaledCursorPosition.Y = c.mousePosition().Y * (1 / game.debugyscale);

            game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
        }

        private void DrawDiag()
        {
            
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(2, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 3), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 1), Color.Black);
            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(4, 3), Color.Black);


            game.spriteBatch.DrawString(game.testFont, game.diag, new Vector2(3, 2), Color.White);
            
        }

        private void DrawCursor()
        {
            game.spriteBatch.Draw(cursorTexture, cursorPosition, Color.LightGreen);

            //game.spriteBatch.DrawCircle(cursorPosition, 90, Color.LightGreen, 1, 32); //cool
        }

        private void DrawMenu()
        {
            menu.Draw(game.spriteBatch);
        }

        private void DrawCircleTrail()
        {

        }

        private void DrawExpandingCircle()
        {

        }
    }
}
