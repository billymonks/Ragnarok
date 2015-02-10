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
using wickedcrush.menu.editor.buttonlist;
using wickedcrush.menu.input;
using wickedcrush.manager.gameplay.room;
using System.IO;
using System.Xml.Linq;

namespace wickedcrush.screen
{
    public class EditorScreen : GameScreen
    {
        SpriteFactory sf;

        public EditorRoom room;
        
        public Point mapOffset;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        public Texture2D cursorTexture;

        public MenuElement current;
        public Toolbox toolbox;
        public EditorTool tool;

        private bool toolReady = false;

        private Dictionary<String, BaseSprite> hud = new Dictionary<String, BaseSprite>();

        public EditorEntityFactory factory;

        public TextInput textInput;

        public RoomInfo roomToLoad = new RoomInfo("");
        public RoomInfo roomToAuthor = new RoomInfo("");

        public Player user;

        public EditorScreen(GameBase game, Player user)
        {
            this.game = game;
            this.user = user;

            Initialize(game);
        }

        public override void Initialize(GameBase g)
        {
            base.Initialize(g);

            exclusiveDraw = true;
            exclusiveUpdate = true;

            sf = new SpriteFactory(g.Content);

            room = new EditorRoom();
            mapOffset = new Point(0, 0);

            factory = new EditorEntityFactory(room);

            cursorPosition = new Vector2();
            scaledCursorPosition = new Vector2();

            toolbox = new Toolbox(this);
            tool = toolbox.tools["wall"];

            

            cursorTexture = game.Content.Load<Texture2D>("debugcontent/img/nice_cursor");

            //textInput = new TextInput(game.controlsManager.getKeyboard());

        }

        public void NewRoom()
        {
            room = new EditorRoom();
            factory.SetMap(room);
        }

        public void LoadRoom()
        {
            game.screenManager.AddScreen(new LoadRoomMenuScreen(game, roomToLoad));

            //room = new EditorRoom(stats)
        }

        public void SaveRoom()
        {
            room.stats.creatorName = game.playerManager.getPlayerList()[0].name;

            room.saveRoom();
            game.roomManager.AddRoomToLocalAtlas(room.stats);
        }

        public void TestRoom()
        {
            roomToAuthor = room.stats;
            game.screenManager.AddScreen(new GameplayScreen(game, roomToAuthor));
            
        }

        public void AuthorRoom()
        {
            PollRoomUpdate();

            if (room.stats.globalId != -1)
            {
                Console.WriteLine("Room: '" + room.stats.roomName + "' with localId: '" + room.stats.localId + "' has already been authored.");
                NewRoom();
                return;
            }

            /*if (!room.stats.readyToAuthor)
            {
                Console.WriteLine("Room: '" + room.stats.roomName + "' with localId: '" + room.stats.localId + "' has not passed its test.");
                return;
            }*/

            SaveRoom();
            game.networkManager.SendMap(room.stats.roomName, room.getXDocument(), room.stats.localId, game.playerManager.getPlayerList()[0].globalId);
            NewRoom();
        }

        private void PollRoom(RoomInfo roomInfo)
        {
            room = new EditorRoom(roomInfo);
            factory.SetMap(room);
        }

        

        public override void Update(GameTime gameTime)
        {
            game.diag = "";

            if (roomToLoad.readyToLoad)
            {
                PollRoom(roomToLoad);
                roomToLoad.readyToLoad = false;
            }

            if (roomToAuthor.readyToAuthor)
            {
                AuthorRoom();
                roomToAuthor.readyToAuthor = false;
            }

            UpdateTextInput(gameTime);

            //saveButton.Update(gameTime, cursorPosition);
            DebugControls(gameTime);
            room.Update(gameTime);
            
        }

        public void UpdateTextInput(GameTime gameTime)
        {
            if (textInput == null)
                return;
            
            textInput.Update(gameTime);
            
            if(textInput.finished)
            {
                //play a funny sound
                room.stats.roomName = textInput.getText();
                textInput = null;
                return;
            }

            if (textInput.cancelled)
            {
                //play a funny sound
                textInput = null;
                return;
            }
        }

        public override void DebugDraw()
        {
            Direction d;
            if (factory.preview != null)
                d = factory.preview.angle;
            else
                d = Direction.East;

            room.DebugDraw(game.whiteTexture, game.arrowTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, mapOffset);

            if (tool != null && tool.getMode() == EditorMode.Entity)
            {
                EditorEntity tempEntity = ((EntityTool)tool).getEntity(scaledCursorPosition, d);
                Color temp;

                if (factory.CanPlace(((EntityTool)tool).getEntity(scaledCursorPosition, d).code,
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

            DrawDiag();
            DrawCursor();
            DrawTextInput();

            game.spriteBatch.End();
        }

        private void DrawTextInput()
        {
            if (textInput != null)
                textInput.DebugDraw(game.spriteBatch, game.testFont);
        }

        public override void Dispose()
        {

        }

        private void DebugControls(GameTime gameTime)
        {
            if (tool != null)
                    tool.Update(gameTime, user.c, scaledCursorPosition, room, toolReady);

            if (user.c is KeyboardControls)
            {
                KeyboardControls keyboard = (KeyboardControls)user.c;
                UpdateCursorPosition(keyboard);

                

                

                if (keyboard.InteractPressed())
                {
                    //menu.Click();
                    //tool = menu.currentTool();
                }

                if (keyboard.InteractReleased())
                {
                    toolReady = true;
                }

                if (keyboard.BoostPressed())
                {
                    game.screenManager.AddScreen(new EditorMenuScreen(game, this));
                }

                if (keyboard.SelectPressed() || keyboard.StartPressed())
                {
                    game.screenManager.AddScreen(new EditorMenuControlBarScreen(game, this));
                }
            }
            else
            {
                GamepadControls gamepad = (GamepadControls)user.c;
                UpdateCursorPosition(gamepad);

                if (gamepad.InteractReleased())
                {
                    toolReady = true;
                }

                if (gamepad.ItemBPressed())
                {
                    game.screenManager.AddScreen(new EditorMenuScreen(game, this));
                }

                if (gamepad.SelectPressed() || gamepad.StartPressed())
                {
                    game.screenManager.AddScreen(new EditorMenuControlBarScreen(game, this));
                }
            }
        }

        public void UpdateCursorPosition(KeyboardControls c)
        {
            cursorPosition.X = c.mousePosition().X;
            cursorPosition.Y = c.mousePosition().Y;

            scaledCursorPosition.X = c.mousePosition().X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
            scaledCursorPosition.Y = c.mousePosition().Y * (1 / game.debugyscale);

            game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
            game.diag += "Map Name: " + room.stats.roomName;
        }

        public void UpdateCursorPosition(GamepadControls c)
        {
            float sensitivity = 10f;
            cursorPosition.X += c.LStickXAxis() * sensitivity;
            cursorPosition.Y += c.LStickYAxis() * sensitivity;

            scaledCursorPosition.X = cursorPosition.X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
            scaledCursorPosition.Y = cursorPosition.Y * (1 / game.debugyscale);


            game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
            game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";
            game.diag += "Map Name: " + room.stats.roomName;
        }


        private void DrawCursor()
        {
            game.spriteBatch.Draw(cursorTexture, cursorPosition, Color.LightGreen);

            //game.spriteBatch.DrawCircle(cursorPosition, 90, Color.LightGreen, 1, 32); //cool
        }

        

        private void DrawCircleTrail()
        {

        }

        private void DrawExpandingCircle()
        {

        }

        private void PollRoomUpdate()
        {
            room.stats = game.roomManager.GetRoomFromLocalAtlas(room.stats.localId);
        }

        
    }
}
