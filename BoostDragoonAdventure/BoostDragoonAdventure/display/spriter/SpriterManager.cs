using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Com.Brashmonkey.Spriter.player;
using Com.Brashmonkey.Spriter;
using wickedcrush.entity;
using wickedcrush.manager.gameplay;

namespace wickedcrush.display.spriter
{
    public class SpriterManager
    {
        Game g;
        public GameplayManager _gameplay;

        SpriterDrawer drawer;
        public Dictionary<String, SpriterLoader> loaders;
        public Dictionary<String, Spriter> spriters;

        public SpriterManager(Game g, GameplayManager gameplay)
        {
            this.g = g;
            _gameplay = gameplay;

            loaders = new Dictionary<String, SpriterLoader>();
            spriters = new Dictionary<String, Spriter>();
        }
        public void LoadContent()
        {
            loaders.Add("loader1", new SpriterLoader(g));
            //player1 = new SpriterPlayer(Spriter.getSpriter("monster/basic.scml", loader1), 0, loader1);
            spriters.Add("monster/basic", new Spriter("monster/basic.scml", loaders["loader1"]));
            

            this.drawer = new SpriterDrawer(g.graphics);
            this.drawer.batch = g.spriteBatch;
            this.drawer.loader = loaders["loader1"];
        }
        protected void UnloadContent()
        {
            foreach (KeyValuePair<String, SpriterLoader> pair in loaders)
            {
                pair.Value.dispose();
            }
            this.drawer.dispose();
        }
        public void Update(GameTime gameTime)
        {

        }
        public void DrawPlayer(SpriterPlayer player)
        {
            drawer.draw(player);
        }
    }
}
