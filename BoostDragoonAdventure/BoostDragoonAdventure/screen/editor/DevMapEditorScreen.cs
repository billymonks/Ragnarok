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
    public class DevMapEditorScreen : GameScreen
    {
        SpriteFactory sf;

        public EditorRoom room;
        
        public Point mapOffset;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        public Texture2D cursorTexture;

        public MenuElement current;
        public EditorTool tool;

        private bool toolReady = false;

        private Dictionary<String, BaseSprite> hud = new Dictionary<String, BaseSprite>();

        public EditorEntityFactory factory;

        public TextInput textInput;

        public RoomInfo roomToLoad = new RoomInfo("");
        public RoomInfo roomToAuthor = new RoomInfo("");

        public KeyboardControls controls;

        public bool gridEnabled = false;

        public DevMapEditorScreen(GameBase game)
        {
            this.game = game;

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

            cursorTexture = game.Content.Load<Texture2D>("debugcontent/img/nice_cursor");

            //textInput = new TextInput(game.controlsManager.getKeyboard());

            controls = g.controlsManager.addKeyboard();
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

        public override void Draw()
        {
            base.Draw();
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

            if(gridEnabled)
                DrawGrid();

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

            UpdateCursorPosition(controls);
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


        private void DrawCursor()
        {
            game.spriteBatch.Draw(cursorTexture, cursorPosition, Color.LightGreen);
        }

        

        private void DrawCircleTrail()
        {

        }

        private void DrawExpandingCircle()
        {

        }

        private void DrawGrid()
        {
            int width = game.GraphicsDevice.Viewport.Width;
            int height = game.GraphicsDevice.Viewport.Height;

            int gridSize = 10;
            Point offset = new Point(0, 0);

            for (int i = 0; i < width; i += gridSize)
            {
                PrimitiveDrawer.DrawLineSegment(game.spriteBatch, new Vector2(i + offset.X, 0), new Vector2(i + offset.X, height), Color.Yellow, 1);
            }

            for (int i = 0; i < height; i += gridSize)
            {
                PrimitiveDrawer.DrawLineSegment(game.spriteBatch, new Vector2(0, i + offset.Y), new Vector2(width, i + offset.Y), Color.Yellow, 1);
            }
        }

        
    }
}
