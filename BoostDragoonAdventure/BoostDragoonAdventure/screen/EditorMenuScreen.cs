using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.menu.editor;
using wickedcrush.factory.sprite;
using wickedcrush.editor.tool;
using Microsoft.Xna.Framework;
using wickedcrush.menu.editor.buttonlist;
using wickedcrush.factory.editor;
using wickedcrush.entity;
using wickedcrush.menu.input;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.screen
{
    public class EditorMenuScreen : GameScreen
    {
        private SpriteFactory sf;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        private EditorMenu menu;
        private EditorScreen _parent;

        public EditorMenuScreen(Game g, EditorScreen parent)
        {
            base.Initialize(g);

            sf = new SpriteFactory(g.Content);

            _parent = parent;

            exclusiveDraw = false;
            exclusiveUpdate = true;

            InitializeEditorMenu();

            cursorPosition = new Vector2();
            scaledCursorPosition = new Vector2();
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";


            DebugControls(gameTime);

            UpdateCursorPosition(game.controlsManager.getKeyboard());

            menu.Update(gameTime, cursorPosition);

            

            
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

        private void InitializeEditorMenu()
        {
            Dictionary<string, MenuNode> nodes = new Dictionary<string, MenuNode>();

            MenuElement wallNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Wall", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["wall"]);

            MenuElement deathSoupNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Death Soup", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["deathsoup"]);


            MenuElement wiringNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Wiring", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["wiring"]);



            wallNode.next = deathSoupNode;
            deathSoupNode.prev = wallNode;
            deathSoupNode.next = wiringNode;
            wiringNode.prev = deathSoupNode;

            MenuElement selectorNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Selector", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["selector"]);

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
                 _parent.toolbox.tools["chest"]);

            MenuElement turretNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Turret", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["turret"]);

            MenuElement potNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Pot", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["pot"]);

            MenuElement floorSwitchNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Floor Switch", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["floorswitch"]);

            MenuElement timerNode = new MenuElement(
                sf.createText(new Vector2(0f, 0f), "Timer", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                _parent.toolbox.tools["timer"]);


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

            menu = new EditorMenu(_parent, nodes);

            Button newButton = new Button(
                sf.createText(new Vector2(0f, 0f), "New", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => { _parent.NewRoom();
                Dispose();
                }
                );

            Button saveButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Save", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => { _parent.SaveRoom();
                game.screenManager.RemoveScreen(this);
                }
                );

            Button authorButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Author", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => { _parent.AuthorRoom();
                game.screenManager.RemoveScreen(this);
                }
                );

            Button renameButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Rename", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => { _parent.textInput = new TextInput(game.controlsManager.getKeyboard());
                game.screenManager.RemoveScreen(this);
                }
                );

            Button loadButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Load", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => { _parent.LoadRoom();
                game.screenManager.RemoveScreen(this);
                }
                );

            Button exitButton = new Button(
                sf.createText(new Vector2(0f, 0f), "Exit", "fonts/TestFont", new Vector2(1f, 1f), Vector2.Zero, Color.White, 0f),
                sf.createTexture("debugcontent/img/happy_cursor", new Vector2(0f, 0f), new Vector2(0.5f, 0.5f), new Vector2(50f, 50f), Color.White, 0f),
                e => {
                    game.screenManager.RemoveScreen(_parent);
                    game.screenManager.RemoveScreen(this);
                }
                );

            menu.controlBar.Add(newButton);
            menu.controlBar.Add(saveButton);
            menu.controlBar.Add(authorButton);
            menu.controlBar.Add(renameButton);
            menu.controlBar.Add(loadButton);
            menu.controlBar.Add(exitButton);

            //menu.current = (node);

            menu.SetCurrentToTool(_parent.tool);

        }

        private void DrawMenu()
        {
            menu.Draw(game.spriteBatch);
        }

        private void DrawCursor()
        {
            game.spriteBatch.Draw(_parent.cursorTexture, cursorPosition, Color.LightGreen);

            //game.spriteBatch.DrawCircle(cursorPosition, 90, Color.LightGreen, 1, 32); //cool
        }

        public override void DebugDraw()
        {

        }

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);

            DrawMenu();
            DrawCursor();

            game.spriteBatch.End();
        }


        public override void Dispose()
        {
            //throw new NotImplementedException();
        }

        private void DebugControls(GameTime gameTime)
        {
            KeyboardControls keyboard = game.controlsManager.getKeyboard();

            UpdateCursorPosition(keyboard);

            if (keyboard.ActionReleased())
            {
                //toolReady = true;
            }

            if (keyboard.BoostPressed())
            {
                game.screenManager.RemoveScreen(this);
            }

            if (menu.highlighted == null)
            {
                return;
            }

            EditorTool highlightedTool = null;

            if (menu.highlighted is MenuElement)
            {
                highlightedTool = ((MenuElement)menu.highlighted).tool;
            }

            if (keyboard.ActionPressed())
            {
                if (menu.currentTool() == highlightedTool)
                {
                    _parent.tool = menu.currentTool();
                    game.screenManager.RemoveScreen(this);
                    return;
                }

                menu.Click();
               
            }
        }
    }
}
