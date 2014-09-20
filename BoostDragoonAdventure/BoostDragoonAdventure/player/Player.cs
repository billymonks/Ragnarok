using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.stats;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using wickedcrush.factory.entity;
using wickedcrush.inventory;
using wickedcrush.menu.hudpanel;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.player
{
    public class Player
    {
        public Controls c;
        private PlayerAgent agent;
        private PersistedStats stats;

        public String name;
        public int playerNumber;

        public HUDPanel panels;

        private bool remove = false;

        public Player(String name, int playerNumber, Controls c, PersistedStats stats)
        {
            this.name = name;
            this.c = c;
            this.stats = stats;
            this.playerNumber = getPlayerNumber();

            panels = new HUDPanel(Color.Purple, new Rectangle(100, 100, 100, 100));
        }

        public Player(String name, int playerNumber, Controls c)
        {
            this.name = name;
            this.c = c;
            this.playerNumber = getPlayerNumber();

            panels = new HUDPanel(Color.Purple, new Rectangle(100, 100, 100, 100));
        }

        private int getPlayerNumber()
        {
            if (c is KeyboardControls)
                return 5;
            if (c is GamepadControls)
                return (int)((GamepadControls)c).playerIndex;
            else return -1;
        }

        public void setAgent(PlayerAgent agent)
        {
            this.agent = agent;
            //stats = agent.stats;
        }

        public PlayerAgent getAgent()
        {
            return agent;
        }

        public void GenerateAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
        {
            initializeAgentStats();

            agent = factory.addPlayerAgent(pos, size, center, solid, c, stats);
            stats = agent.stats;
        }

        private void initializeAgentStats()
        {
            stats.set("maxBoost", 1000);
            stats.set("boost", 1000);
            stats.set("fillSpeed", 3);
            stats.set("useSpeed", 8);
            stats.set("boostSpeedMod", 0);

            stats.set("boostRecharge", 250);
            stats.set(("iFrameTime"), 150);
            stats.set("staggerDistance", 100);

            stats.inventory.removeAllOfItem(ItemServer.getItem("Healthsweed"));
            stats.inventory.removeAllOfItem(ItemServer.getItem("Fireball"));

            stats.inventory.receiveItem(ItemServer.getItem("Healthsweed"), 3);
            stats.inventory.receiveItem(ItemServer.getItem("Fireball"), 5);
        }



        protected void Remove()
        {
            remove = true;
        }

        public bool readyForRemoval()
        {
            return remove;
        }

        public void setStats(PersistedStats stats)
        {
            this.stats = stats;
        }

        public PersistedStats getStats()
        {
            return stats;
        }

        public void DebugDrawPanels(SpriteBatch sb)
        {
            panels.DebugDraw(sb, new Point((int)agent.pos.X, (int)agent.pos.Y));
        }

        public void Update(GameTime gameTime)
        {
            c.Update();
        }
    }
}
