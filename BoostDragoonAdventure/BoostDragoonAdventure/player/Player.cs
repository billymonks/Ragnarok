using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.controls;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.stats;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace wickedcrush.player
{
    public class Player
    {
        public Controls c;
        private PlayerAgent agent;
        private PersistedStats stats;

        public String name;
        public int playerNumber;

        public bool remove;

        public Player(String name, int playerNumber, Controls c, PersistedStats stats)
        {
            this.name = name;
            this.c = c;
            this.stats = stats;
            this.playerNumber = getPlayerNumber();
        }

        public Player(String name, int playerNumber, Controls c)
        {
            this.name = name;
            this.c = c;
            this.playerNumber = getPlayerNumber();
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
            stats = agent.stats;
        }

        public PlayerAgent getAgent()
        {
            return agent;
        }

        public void GenerateAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            agent = new PlayerAgent(w, pos, size, center, solid, c);
            stats = agent.stats;
        }

        public void RemovePlayer(World w)
        {
            agent.removeBody(w);
            remove = true;
        }

        public void setStats(PersistedStats stats)
        {
            this.stats = stats;
        }

        public PersistedStats getStats()
        {
            return stats;
        }

        public void Update(GameTime gameTime)
        {
            c.Update();

            if (agent != null)
            {
                agent.Update(gameTime);

                if (agent.remove)
                    agent = null;
            }
        }
    }
}
