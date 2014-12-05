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
using wickedcrush.menu.panel;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d;
using wickedcrush.factory.menu.panel;
using wickedcrush.helper;
using wickedcrush.manager.gameplay.room;

namespace wickedcrush.player
{
    public class Player
    {
        public Controls c;
        private PlayerAgent agent;
        private PersistedStats stats;

        public String name;
        public int playerNumber;
        public int globalId = -1;
        public String localId;

        public PanelFactory pf; //move this stuff to panel manager
        public Stack<Panel> panels; // this too

        public InventoryPanel inventoryPanel; // this three

        private bool remove = false;

        public Player(String name, int playerNumber, Controls c, PersistedStats stats, PanelFactory pf)
        {
            this.name = name;
            this.c = c;
            this.stats = stats;
            this.playerNumber = getPlayerNumber();

            this.pf = pf;
            panels = new Stack<Panel>();
            inventoryPanel = pf.getInventory(stats.inventory);
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
        }

        public PlayerAgent getAgent()
        {
            return agent;
        }

        public void RespawnAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
        {

        }

        public void GenerateAgent(Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
        {
            //initializeAgentStats();

            agent = factory.addPlayerAgent(name, pos, size, center, solid, c, stats, this);
            stats = agent.stats;
        }

        public void initializeAgentStats()
        {
            stats.set("hp", 15);
            stats.set("maxHP", 15);
            stats.set("maxBoost", 1000);
            stats.set("boost", 1000);
            stats.set("fillSpeed", 3);
            stats.set("useSpeed", 8);
            stats.set("boostSpeedMod", 0);

            stats.set("boostRecharge", 250);
            stats.set(("iFrameTime"), 150);
            stats.set("staggerDistance", 100);

            stats.set("staggerLimit", 100);
            stats.set("staggerDuration", 50);
            stats.set("stagger", 0);

            //stats.inventory.removeAllOfItem(ItemServer.getItem("Healthsweed"));
            //stats.inventory.removeAllOfItem(ItemServer.getItem("Spellbook: Fireball"));

            stats.inventory.receiveItem(ItemServer.getItem("Healthsweed"), 3);
            stats.inventory.receiveItem(ItemServer.getItem("Spellbook: Fireball"), 1);
        }



        public void Remove()
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

        public void DebugDrawPanels(SpriteBatch sb, Camera camera, SpriteFont font)
        {
            foreach(Panel p in panels)
            {
                p.DebugDraw(sb,
                    new Point(
                        (int)Helper.roundTowardZero(agent.pos.X - camera.cameraPosition.X),
                        (int)Helper.roundTowardZero(agent.pos.Y - camera.cameraPosition.Y)),
                        font);
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdatePanels(gameTime);
        }

        public void UpdatePanels(GameTime gameTime)
        {

            if ((c.StartPressed() || agent == null || agent.dead) && panels.Count > 0)
            {
                panels.Pop();
                if (agent != null)
                    agent.busy = false;
            }
            else if ((c.StartPressed() && agent != null && !agent.dead) && panels.Count == 0)
            {
                panels.Push(inventoryPanel);
                agent.busy = true;
            }            

            if (panels.Count > 0)
            {
                panels.Peek().Update(gameTime, c);
            }
            
        }

        public void CloseAllPanels()
        {
            if (agent != null)
                agent.busy = false;

            panels.Clear();
        }
    }
}
