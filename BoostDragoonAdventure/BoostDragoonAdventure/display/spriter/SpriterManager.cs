using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Com.Brashmonkey.Spriter.player;
using Com.Brashmonkey.Spriter;
using wickedcrush.entity;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.display.spriter
{
    public class SpriterManager
    {
        GameBase g;

        public SpriterDrawer drawer;
        public Dictionary<String, SpriterLoader> loaders;
        public Dictionary<String, Spriter> spriters;

        public Effect unlitSpriteEffect;

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
            unlitSpriteEffect = g.Content.Load<Effect>("fx/UnlitSprite");

            //loaders.Add("loader1", new SpriterLoader(g));
            loaders.Add("loader2", new SpriterLoader(g));
            loaders.Add("loader3", new SpriterLoader(g));
            loaders.Add("loader4", new SpriterLoader(g));
            loaders.Add("particles", new SpriterLoader(g));
            loaders.Add("chest", new SpriterLoader(g));
            loaders.Add("trap", new SpriterLoader(g));
            loaders.Add("hud", new SpriterLoader(g));
            loaders.Add("weapons", new SpriterLoader(g));
            loaders.Add("shadow", new SpriterLoader(g));
            loaders.Add("cursor", new SpriterLoader(g));
            loaders.Add("monster_a", new SpriterLoader(g));
            loaders.Add("cactus", new SpriterLoader(g));
            loaders.Add("shapes", new SpriterLoader(g));

            spriters.Add("all", new Spriter("Content/sprites/all/all.scml", loaders["loader2"]));
            spriters.Add("cursor", new Spriter("Content/sprites/all/cursor.scml", loaders["cursor"]));

            spriters.Add("you", new Spriter("Content/sprites/you/you.scml", loaders["loader3"]));
            spriters.Add("you_pink", new Spriter("Content/sprites/you_pink/you.scml", loaders["loader4"]));

            spriters.Add("particles", new Spriter("Content/sprites/particles/particles.scml", loaders["particles"]));

            spriters.Add("chest", new Spriter("Content/sprites/chest/chest.scml", loaders["chest"]));

            spriters.Add("trap", new Spriter("Content/sprites/trap/trap.scml", loaders["trap"]));

            spriters.Add("hud", new Spriter("Content/sprites/hud/hud.scml", loaders["hud"]));

            spriters.Add("weapons", new Spriter("Content/sprites/weapons/weapons.scml", loaders["weapons"]));

            spriters.Add("shadow", new Spriter("Content/sprites/shadow/shadow.scml", loaders["shadow"]));

            spriters.Add("monster_a", new Spriter("Content/sprites/enemy/monster_a.scml", loaders["monster_a"]));

            spriters.Add("cactus", new Spriter("Content/sprites/cactus/cactus.scml", loaders["cactus"]));

            spriters.Add("shapes", new Spriter("Content/sprites/shapes/shapes.scml", loaders["shapes"]));

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

        public void DrawPlayer(SpriterPlayer player, Effect effect)
        {
            this.drawer.loader = player.loader;
            drawer.draw(player);
        }
    }
}
