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
using wickedcrush.display._3d;
using wickedcrush.map;
using System.Xml.Linq;
using System.IO;
using wickedcrush.stats;
using wickedcrush.inventory;
using wickedcrush.manager.network;

namespace wickedcrush.manager.player
{
    public class PlayerManager : Microsoft.Xna.Framework.GameComponent
    {
        Game g;

        private List<Player> playerList = new List<Player>();
        private List<Player> removeList = new List<Player>();

        private ControlsManager _cm;
        private NetworkManager _nm;

        public PlayerManager(Game game)
            : base(game)
        {
            g = game;

            _cm = game.controlsManager;
            _nm = game.networkManager;

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

        public bool checkForTransition(Map m)
        {
            foreach (Player p in playerList)
            {
                if(p.getAgent() != null 
                    && !p.getAgent().busy
                    && (p.getAgent().pos.X < 0
                    || p.getAgent().pos.Y < 0
                    || p.getAgent().pos.X > m.width
                    || p.getAgent().pos.Y > m.height))
                {
                    return true;
                }
            }
            return false;
        }

        public void startTransition()
        {
            CloseAllPanels();

            foreach (Player p in playerList)
            {
                if(p.getAgent() != null)
                    p.getAgent().busy = true;
            }

            saveAllPlayers();
        }

        public void endTransition()
        {
            foreach (Player p in playerList)
            {
                if (p.getAgent() != null)
                    p.getAgent().busy = false;
            }

            g.screenManager.EndLoading();
        }

        public void UpdatePanels(GameTime gameTime)
        {
            foreach (Player p in playerList)
            {
                p.UpdatePanels(gameTime);
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
            foreach (Player player in playerList)
            {
                if (p.c.Equals(player.c))
                {
                    return;
                }
            }
            playerList.Add(p);
        }

        public Player addNewPlayer(String name, int playerNumber, Controls c)
        {
            Player p = new Player(name, playerNumber, c, new PersistedStats(), g.panelFactory);
            
            do
            {
                p.localId = Guid.NewGuid().ToString();
            } while (File.Exists("characters/" + p.localId + ".xml"));

            p.initializeAgentStats();

            savePlayer(p);

            playerList.Add(p);

            _nm.AssignCharacter(p.name, p.localId);

            return p;
        }

        public void saveAllPlayers()
        {
            foreach (Player p in playerList)
            {
                savePlayer(p);
            }
        }

        public void savePlayer(Player p)
        {
            if (!Directory.Exists("characters/"))
                Directory.CreateDirectory("characters/");

            String path = "characters/" + p.localId + ".xml";

            XDocument doc = new XDocument();
            XElement rootElement = new XElement("character");
            XElement statsElement = new XElement("stats");
            XElement inventoryElement = new XElement("inventory");

            XElement temp;

            rootElement.Add(new XAttribute("name", p.name));
            rootElement.Add(new XAttribute("localId", p.localId));
            rootElement.Add(new XAttribute("globalId", p.globalId));

            foreach (KeyValuePair<String, int> pair in p.getStats().numbers)
            {
                temp = new XElement("stat", pair.Value);
                temp.Add(new XAttribute("name", pair.Key));
                statsElement.Add(temp);
            }

            foreach (Item i in p.getStats().inventory.GetItemList())
            {
                temp = new XElement("item", i.name);
                temp.Add(new XAttribute("count", p.getStats().inventory.getItemCount(i)));
                inventoryElement.Add(temp);
            }

            inventoryElement.Add(new XAttribute("gold", p.getStats().inventory.currency));

            doc.Add(rootElement);
            rootElement.Add(statsElement);
            rootElement.Add(inventoryElement);

            doc.Save(path);
        }

        public Player loadPlayer(String localId, int playerNumber, Controls c)
        {
            String path = "characters/" + localId + ".xml";

            if (!File.Exists(path))
                throw new FileLoadException("i'm sorry to break this bad news to you but your character seems to be deleted: " + path);

            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("character"));
            XElement statsElement = new XElement(rootElement.Element("stats"));
            XElement inventoryElement = new XElement(rootElement.Element("inventory"));

            PersistedStats stats = new PersistedStats();

            foreach (XElement statElement in statsElement.Elements("stat"))
            {
                stats.set(statElement.Attribute("name").Value, int.Parse(statElement.Value));
            }

            stats.inventory.addCurrency(int.Parse(inventoryElement.Attribute("gold").Value));

            foreach (XElement itemElement in inventoryElement.Elements("item"))
            {
                stats.inventory.receiveItem(ItemServer.getItem(itemElement.Value), int.Parse(itemElement.Attribute("count").Value));
            }


            Player p = new Player(rootElement.Attribute("name").Value, playerNumber, c, stats, g.panelFactory);

            p.localId = localId;
            p.globalId = int.Parse(rootElement.Attribute("globalId").Value);

            playerList.Add(p);

            if(p.globalId==-1)
                _nm.AssignCharacter(p.name, p.localId);

            return p;
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D whiteTexture, SpriteFont f)
        {
            
        }

        public void DebugDrawPanels(SpriteBatch sb, Camera camera, SpriteFont font)
        {
            foreach (Player p in playerList)
            {
                p.DebugDrawPanels(sb, camera, font);
            }
        }

        public void DrawPlayerHud(SpriteBatch sb, SpriteFont f)
        {
            String hud = "";
            foreach (Player p in getPlayerList())
            {
                hud = "";
                hud += p.name + "\nHP: " + p.getStats().get("hp") + "/" + p.getStats().get("maxHP")
                    + "\nEngine: " + p.getStats().get("boost") + "/" + p.getStats().get("maxBoost");

                if (p.getStats().inventory.itemA != null)
                    hud += "\nItem A: " + p.getStats().inventory.itemA.name + " : " + p.getStats().inventory.getItemCount(p.getStats().inventory.itemA);

                if (p.getStats().inventory.itemB != null)
                    hud += "\nItem B: " + p.getStats().inventory.itemB.name + " : " + p.getStats().inventory.getItemCount(p.getStats().inventory.itemB);

                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 - 1, 5 - 1), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 + 1, 5 + 1), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 - 1, 5 + 1), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 + 1, 5 - 1), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 - 1, 5), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5 + 1, 5), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5, 5 + 1), Color.Black);
                sb.DrawString(f,
                    hud,
                    new Vector2(p.playerNumber * 100 + 5, 5 - 1), Color.Black);

                sb.DrawString(f, 
                    hud, 
                    new Vector2(p.playerNumber * 100 + 5, 5), Color.White);
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

        public bool pollDodgeSuccess()
        {
            bool temp = false;

            foreach (Player p in playerList)
            {
                if (p.getAgent() != null && p.getAgent().pollDodgeSuccess())
                {
                    temp = true;
                }
            }

            return temp;
        }

        public bool AssignGlobalId(String localId, int globalId)
        {
            Player p = GetPlayerByLocalId(localId);

            if (p != null)
            {
                p.globalId = globalId;
                savePlayer(p);
                return true;
            }


            return false;
        }

        public Player GetPlayerByLocalId(String localId)
        {
            foreach (Player p in playerList)
            {
                if (p.localId.Equals(localId))
                {
                    return p;
                }
            }
            return null;
        }

        public void CloseAllPanels()
        {
            foreach (Player p in playerList)
            {
                p.CloseAllPanels();
            }
        }
    }
}
