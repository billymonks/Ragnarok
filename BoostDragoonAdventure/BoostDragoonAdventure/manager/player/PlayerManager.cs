using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.entity.physics_entity;
using FarseerPhysics.Dynamics;
using wickedcrush.controls;
using wickedcrush.factory.entity;
using wickedcrush.manager.controls;

namespace wickedcrush.manager.player
{
    public class PlayerManager : Microsoft.Xna.Framework.GameComponent
    {
        Game g;

        private List<Player> playerList = new List<Player>();
        private List<Player> removeList = new List<Player>();

        private ControlsManager _cm;

        public PlayerManager(Game game, ControlsManager cm)
            : base(game)
        {
            g = game;

            _cm = cm;

            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            updatePlayers(gameTime);
            

            base.Update(gameTime);
        }

        private void updatePlayers(GameTime gameTime)
        {
            foreach (Player p in playerList)
            {
                p.Update(gameTime);

                if (p.readyForRemoval())
                    removeList.Add(p);
            }


            performRemoval();
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Player p in removeList)
                {
                    playerList.Remove(p);
                }

                removeList.Clear();
            }
        }

        public void addPlayer(Player p)
        {
            foreach (Player player in playerList)
            {
                if (p.c.Equals(player.c))
                {
                    return;
                }
            }
            playerList.Add(p);
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D whiteTexture, SpriteFont f)
        {
            //foreach (Player p in playerList)
            //{
                //if(p.getAgent()!=null)
                    //p.getAgent().DebugDraw(whiteTexture, gd, sb, f, Color.Green);
            //}
        }

        public void DrawPlayerHud(SpriteBatch sb, SpriteFont f)
        {
            String hud = "";
            foreach (Player p in getPlayerList())
            {
                hud += p.name + "\nHP: " + p.getStats().get("hp") + "/" + p.getStats().get("maxHP")
                    + "\nEngine: " + p.getStats().get("boost") + "/" + p.getStats().get("maxBoost");

                if (p.getStats().inventory.itemA != null)
                    hud += "\nItem A: " + p.getStats().inventory.itemA.name + " : " + p.getStats().inventory.getItemCount(p.getStats().inventory.itemA);

                if (p.getStats().inventory.itemB != null)
                    hud += "\nItem B: " + p.getStats().inventory.itemB.name + " : " + p.getStats().inventory.getItemCount(p.getStats().inventory.itemB);

                sb.DrawString(f, 
                    hud, 
                    new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }
        }

        public void DrawPlayerSelect(SpriteBatch sb, SpriteFont f)
        {
            foreach (Player p in getPlayerList())
            {
                sb.DrawString(f, p.name, new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
            }
        }

        public List<Player> getPlayerList()
        {
            return playerList;
        }

        public Vector2 getMeanPlayerPos()
        {
            Vector2 meanPos = new Vector2(0f, 0f);
            foreach(Player p in playerList)
            {
                meanPos += p.getAgent().pos;
            }

            meanPos /= playerList.Count;

            return meanPos;
        }
    }
}
