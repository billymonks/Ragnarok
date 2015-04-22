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
        GameBase g;

        public SpriterDrawer drawer;
        public Dictionary<String, SpriterLoader> loaders;
        public Dictionary<String, Spriter> spriters;

        public SpriterManager(GameBase g)
        {
            this.g = g;

            loaders = new Dictionary<String, SpriterLoader>();
            spriters = new Dictionary<String, Spriter>();

            
        }
        public void AddSpriter(String spriter)
        {

        }

        public void LoadContent()
        {
            //loaders.Add("loader1", new SpriterLoader(g));
            loaders.Add("loader2", new SpriterLoader(g));
            loaders.Add("loader3", new SpriterLoader(g));

            //spriters.Add("fuck", new Spriter("Content/sprites/fuck/ass_legacy.scml", loaders["loader1"]));
            //spriters.Add("cursor", new Spriter("Content/sprites/cursor/cursor.scml", loaders["loader1"]));
            
            //spriters.Add("neku", new Spriter("Content/sprites/ui/ui.scml", loaders["loader1"]));

            spriters.Add("all", new Spriter("Content/sprites/all/all.scml", loaders["loader2"]));

            spriters.Add("you", new Spriter("Content/sprites/you/you.scml", loaders["loader3"]));
            

            this.drawer = new SpriterDrawer(g.graphics);
            this.drawer.batch = g.spriteBatch;
            this.drawer.loader = loaders["loader3"];
        }
        public void UnloadContent()
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

        public void DrawPlayers(GameplayManager gm)
        {
            //List<SpriterPlayer> players = gm.entityManager.getSpriterPlayers();

        }
        public void DrawPlayer(SpriterPlayer player)
        {
            this.drawer.loader = player.loader;
            drawer.draw(player);
        }
    }
}
