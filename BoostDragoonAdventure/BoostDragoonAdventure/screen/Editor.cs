using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity;
using wickedcrush.map.layer;
using wickedcrush.map;

namespace wickedcrush.screen
{
    public class Editor : GameScreen
    {
        public Dictionary<LayerType, Layer> layerList;
        public List<Entity> entityList;

        public Editor(Game game)
        {
            this.game = game;

            Initialize(game);
        }

        public override void Initialize(Game g)
        {
            base.Initialize(g);

            game.testMap = new Map(game.mapName, w, factory);
        }

        public override void Update(GameTime gameTime)
        {
            game.diag = "";
        }

        public override void Draw()
        {
            game.testMap.drawMap(game.GraphicsDevice, game.spriteBatch, game.testFont);
        }

        public override void Dispose()
        {

        }
    }
}
