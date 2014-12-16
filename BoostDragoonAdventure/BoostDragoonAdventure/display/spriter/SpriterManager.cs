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
        GameplayManager _gameplay;

        GraphicsDeviceManager graphics;
        SpriterDrawer drawer;
        Dictionary<String, SpriterLoader> loaders;
        SpriterPlayer player1;

        public SpriterManager(Game g, GameplayManager gameplay)
        {
            this.g = g;
            _gameplay = gameplay;
        }
        public void LoadContent()
        {
            loaders.Add("loader1", new SpriterLoader(g));
            //player1 = new SpriterPlayer(Spriter.getSpriter("monster/basic.scml", loader1), 0, loader1);
            Spriter spriter = new Spriter("monster/basic.scml", loaders["loader1"]);
            player1 = new SpriterPlayer(spriter.getSpriterData(), 0, loaders["loader1"]);
            player1.setAnimation("dash", 0, 0);
            player1.setFrameSpeed(20);

            this.drawer = new SpriterDrawer(this.graphics);
            this.drawer.batch = g.spriteBatch;
            this.drawer.loader = loaders["loader1"];
        }
        protected override void UnloadContent()
        {
            foreach (KeyValuePair<String, SpriterLoader> pair in loaders)
            {
                pair.Value.dispose();
            }
            this.drawer.dispose();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Entity e in _gameplay.entityManager.entityList)
            {
                //e.player
            }
        }
    }
}
