using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity;
using wickedcrush.map.layer;
using wickedcrush.map;
using wickedcrush.editor;

namespace wickedcrush.screen
{
    public class Editor : GameScreen
    {
        public EditorMap map;

        public Editor(Game game)
        {
            this.game = game;

            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);
            map = new EditorMap(game.mapName);
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
        }

        public override void Draw()
        {
            //map.drawMap
        }

        public override void Dispose()
        {

        }
    }
}
