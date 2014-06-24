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

namespace wickedcrush.screen
{
    public class Editor : GameScreen
    {
        public EditorMap map;
        public Point mapOffset;

        public Vector2 cursorPosition;
        public Vector2 scaledCursorPosition;

        private Texture2D cursorTexture;

        private EditorTool tool;

        private EditorMenu menu;

        public Editor(Game game)
        {
            this.game = game;

            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);
            map = new EditorMap(game.mapName);
            mapOffset = new Point(0, 0);

            cursorPosition = new Vector2();
            scaledCursorPosition = new Vector2();

            tool = new PlacerTool();

            menu = new EditorMenu();

            cursorTexture = game.Content.Load<Texture2D>("debug/img/nice_cursor");
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";

            DebugControls();
        }

        public override void DebugDraw()
        {
            map.DebugDraw(game.whiteTexture, game.GraphicsDevice, game.spriteBatch, game.testFont, mapOffset);
        }

        public override void FreeDraw()
        {
            game.spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearWrap, null, RasterizerState.CullNone, null, Matrix.Identity);
            
            DrawDiag();
            DrawCursor();

            game.spriteBatch.End();
        }

        public override void Dispose()
        {

        }

        private void DebugControls()
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
                    cursorPosition.X = ((KeyboardControls)p.c).mousePosition().X;
                    cursorPosition.Y = ((KeyboardControls)p.c).mousePosition().Y;

                    scaledCursorPosition.X = ((KeyboardControls)p.c).mousePosition().X * (1 / game.debugyscale) - (game.GraphicsDevice.Viewport.Width * 0.5f * (1 / game.debugyscale) - 320);
                    scaledCursorPosition.Y = ((KeyboardControls)p.c).mousePosition().Y * (1 / game.debugyscale);

                    game.diag += "Cursor Position: " + cursorPosition.X + ", " + cursorPosition.Y + "\n";
                    game.diag += "4:3 Cursor Position: " + scaledCursorPosition.X + ", " + scaledCursorPosition.Y + "\n";

                    if (((KeyboardControls)p.c).ActionHeld()) //lmb
                    {
                        tool.primaryAction(scaledCursorPosition, map);
                    }

                    if (((KeyboardControls)p.c).StrafeHeld()) //rmb
                    {
                        tool.secondaryAction(scaledCursorPosition, map);
                    }
                }
            }
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
        }

        private void DrawMenu()
        {
            menu.DebugDraw(game.spriteBatch, game.testFont);
        }
    }
}
