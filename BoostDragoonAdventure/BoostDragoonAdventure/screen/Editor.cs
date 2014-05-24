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

namespace wickedcrush.screen
{
    public class Editor : GameScreen
    {
        public EditorMap map;
        public Point mapOffset;

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
            }
        }
    }
}
