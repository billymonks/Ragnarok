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

namespace wickedcrush.manager.player
{
    public class PlayerManager : Microsoft.Xna.Framework.GameComponent
    {
        Game g;

        private List<Player> playerList = new List<Player>();
        private List<Player> removeList = new List<Player>();

        public PlayerManager(Game game)
            : base(game)
        {
            g = game;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update(GameTime gameTime, World w)
        {
            updatePlayers(gameTime);
            respawnPlayer(w);
            checkForNewPlayers(w);

            base.Update(gameTime);
        }

        private void updatePlayers(GameTime gameTime)
        {
            foreach (Player p in playerList)
            {
                p.Update(gameTime);

                if (p.remove)
                    removeList.Add(p);
            }


            performRemoval();
        }

        private void checkForNewPlayers(World w)
        {
            if (g.controlsManager.gamepadPressed())
            {
                addPlayer(g.controlsManager.checkAndAddGamepads(), w);
            }

            if (g.controlsManager.keyboardPressed())
            {
                addPlayer(g.controlsManager.addKeyboard(), w);
            }
        }

        private void addPlayer(Controls controls, World w)
        {
            Player p = new Player("bunz", getPlayerList().Count, controls);
            p.GenerateAgent(w, new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true);
            addPlayer(p);

        }

        private void respawnPlayer(World w)
        {
            foreach (Player p in playerList)
            {
                if (p.getAgent() == null && p.c.StartPressed())
                {
                    p.GenerateAgent(w, new Vector2(12, 320), new Vector2(24, 24), new Vector2(12, 12), true);
                }
            }
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
            playerList.Add(p);
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D whiteTexture, SpriteFont f)
        {
            foreach (Player p in playerList)
            {
                if(p.getAgent()!=null)
                    p.getAgent().DebugDraw(whiteTexture, gd, sb, f, Color.Green);
            }
        }

        public List<Player> getPlayerList()
        {
            return playerList;
        }
    }
}
