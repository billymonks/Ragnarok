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
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.manager.player
{
    public class PlayerManager : Microsoft.Xna.Framework.GameComponent
    {
        GameBase g;

        private List<Player> playerList = new List<Player>();
        private List<Player> removeList = new List<Player>();

        private ControlsManager _cm;
        private NetworkManager _nm;

        private Dictionary<Player, PersistedStats> tempSavedStats = new Dictionary<Player,PersistedStats>();

        

        public PlayerManager(GameBase game)
            : base(game)
        {
            g = game;

            

            Initialize();
        }

        public override void Initialize()
        {
            _cm = g.controlsManager;
            _nm = g.networkManager;

            base.Initialize();


        }

        public void Update(GameTime gameTime)
        {
            updatePlayers(gameTime);
            



            base.Update(gameTime);
        }

        public void LoadContent()
        {
            //
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

        public void SaveTempStats()
        {
            tempSavedStats.Clear();
            foreach (Player p in playerList)
            {
                tempSavedStats.Add(p, p.getStats());
            }
        }

        public void SetTempStatsForEditorTest()
        {
            tempSavedStats.Clear();
            foreach (Player p in playerList)
            {
                tempSavedStats.Add(p, p.getStats());
                InitializeStatsForEditorTest(p);
            }
        }

        private void InitializeStatsForEditorTest(Player p)
        {
            PersistedStats stats = new PersistedStats();
            stats.set("hp", 1);
            stats.set("maxHP", 1);
            stats.set("maxBoost", 1000);
            stats.set("boost", 1000);
            stats.set("fillSpeed", 0);
            stats.set("useSpeed", 8);
            stats.set("boostSpeedMod", 0);

            stats.set("boostRecharge", 250);
            stats.set(("iFrameTime"), 150);
            stats.set("staggerDistance", 100);

            stats.set("staggerLimit", 100);
            stats.set("staggerDuration", 50);
            stats.set("stagger", 0);

            p.setStats(stats);
        }

        public void LoadTempStats()
        {
            foreach (KeyValuePair<Player, PersistedStats> pair in tempSavedStats)
            {
                pair.Key.setStats(pair.Value);
            }
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
                    if (p.getAgent() != null)
                        p.getAgent().Remove();
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
            XElement equipmentElement = new XElement("equipment");
            XElement gearElement = new XElement("gear");

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

            foreach (KeyValuePair<String, String> pair in p.getStats().strings)
            {
                temp = new XElement("string", pair.Value);
                temp.Add(new XAttribute("name", pair.Key));
                statsElement.Add(temp);
            }

            foreach (Weapon i in p.getStats().inventory.GetWeaponList())
            {
                temp = new XElement("weapon", i.name);
                temp.Add(new XAttribute("count", p.getStats().inventory.getItemCount(i)));
                inventoryElement.Add(temp);
            }

            foreach (Consumable i in p.getStats().inventory.GetConsumableList())
            {
                temp = new XElement("consumable", i.name);
                temp.Add(new XAttribute("count", p.getStats().inventory.getItemCount(i)));
                inventoryElement.Add(temp);
            }

            foreach (Part i in p.getStats().inventory.GetPartList())
            {
                temp = new XElement("part", i.name);
                temp.Add(new XAttribute("count", p.getStats().inventory.getItemCount(i)));
                inventoryElement.Add(temp);
            }

            if (p.getStats().inventory.equippedWeapon != null)
            {
                temp = new XElement("equippedWeapon", p.getStats().inventory.equippedWeapon.name);
                equipmentElement.Add(temp);
            }

            SaveGear(p, gearElement);

            inventoryElement.Add(new XAttribute("gold", p.getStats().inventory.currency));

            doc.Add(rootElement);
            rootElement.Add(statsElement);
            rootElement.Add(inventoryElement);
            rootElement.Add(equipmentElement);
            rootElement.Add(gearElement);

            doc.Save(path);
        }

        private void SaveGear(Player p, XElement gearElement)
        {
            XElement coreElement = new XElement("core");
            coreElement.Add(new XAttribute("name", p.getStats().inventory.gear.core.part.name));

            List<EquippedConnection> connections = p.getStats().inventory.gear.core.equippedConnections;

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].connection.female && connections[i].child != null)
                {
                    SavePart(p, connections[i].child, coreElement, i);
                }
            }

            gearElement.Add(coreElement);
        }

        private void SavePart(Player p, EquippedPart childPart, XElement parentElement, int connectionIndex)
        {
            XElement partElement = new XElement("part");
            partElement.Add(new XAttribute("index", connectionIndex));
            partElement.Add(new XAttribute("name", childPart.part.name));

            List<EquippedConnection> connections = childPart.equippedConnections;

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].connection.female && connections[i].child != null)
                {
                    SavePart(p, connections[i].child, partElement, i);
                }
            }

            parentElement.Add(partElement);
        }

        private void LoadGear(Player p, XElement gearElement)
        {
            XElement coreElement = gearElement.Element("core");

            EquippedPart core = new EquippedPart(InventoryServer.getPart(coreElement.Attribute("name").Value), null);

            p.getStats().inventory.gear = new Gear(core, new List<EquippedPart>());

            foreach (XElement partElement in coreElement.Elements("part"))
            {
                LoadPart(p, partElement, core.equippedConnections[int.Parse(partElement.Attribute("index").Value)]);
            }

            p.getStats().ApplyStats();
        }

        private void LoadPart(Player p, XElement partElement, EquippedConnection parentConnection)
        {
            EquippedPart tempPart = p.getStats().inventory.gear.EquipPart(InventoryServer.getPart(partElement.Attribute("name").Value), parentConnection);
            

            foreach (XElement subElement in partElement.Elements("part"))
            {
                LoadPart(p, subElement, tempPart.equippedConnections[int.Parse(subElement.Attribute("index").Value)]);
            }
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
            XElement equipmentElement = new XElement(rootElement.Element("equipment"));
            XElement gearElement = new XElement(rootElement.Element("gear"));
            

            PersistedStats stats = new PersistedStats();

            foreach (XElement statElement in statsElement.Elements("stat"))
            {
                stats.set(statElement.Attribute("name").Value, int.Parse(statElement.Value));
            }

            foreach (XElement statElement in statsElement.Elements("string"))
            {
                stats.set(statElement.Attribute("name").Value, statElement.Value);
            }

            if (!stats.stringsContainsKey("home"))
            {
                stats.set("home", "a1");
            }

            stats.inventory.addCurrency(int.Parse(inventoryElement.Attribute("gold").Value));

            foreach (XElement itemElement in inventoryElement.Elements("weapon"))
            {
                stats.inventory.receiveItem(InventoryServer.getWeapon(itemElement.Value), int.Parse(itemElement.Attribute("count").Value));
            }

            foreach (XElement itemElement in inventoryElement.Elements("consumable"))
            {
                stats.inventory.receiveItem(InventoryServer.getConsumable(itemElement.Value), int.Parse(itemElement.Attribute("count").Value));
            }

            foreach (XElement itemElement in inventoryElement.Elements("part"))
            {
                stats.inventory.receiveItem(InventoryServer.getPart(itemElement.Value), int.Parse(itemElement.Attribute("count").Value));
            }

            foreach (XElement equippedWeaponElement in equipmentElement.Elements("equippedWeapon"))
            {
                stats.inventory.equippedWeapon = InventoryServer.getWeapon(equippedWeaponElement.Value);
            }


            Player p = new Player(rootElement.Attribute("name").Value, playerNumber, c, stats, g.panelFactory);

            LoadGear(p, gearElement);

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

        public void DrawHud()
        {
            foreach (Player p in playerList)
            {
                p.hudChargeSpriter.update(0f, -930f);
                p.hpSpriter.update(120f, -980f);
                p.fuelSpriter.update(140f, -1010f);
                p.hpSpriter.SetDepth(0.1f);
                p.fuelSpriter.SetDepth(0.2f);
                p.hudChargeSpriter.SetDepth(0.3f);
                g.spriterManager.DrawPlayer(p.hudChargeSpriter);
                g.spriterManager.DrawPlayer(p.hpSpriter);
                g.spriterManager.DrawPlayer(p.fuelSpriter);

                

            }

            
        }

        public void DrawPlayerHud(SpriteBatch sb, SpriteFont f)
        {
            String hud = "";
            foreach (Player p in getPlayerList())
            {
                hud = "";
                hud += p.name + "\nHP: " + p.getStats().getNumber("hp") + "/" + p.getStats().getNumber("maxHP")
                    + "\nEngine: " + p.getStats().getNumber("boost") + "/" + p.getStats().getNumber("maxBoost");

                if (p.getStats().inventory.equippedWeapon != null)
                    hud += "\nItem A: " + p.getStats().inventory.equippedWeapon.name + " : " + p.getStats().inventory.getItemCount(p.getStats().inventory.equippedWeapon);

                p.getAgent().bodySpriter.calcBoundingBox(null);
                hud += "\nX: " + p.getAgent().bodySpriter.getBoundingBox().left;
                hud += " Y: " + p.getAgent().bodySpriter.getBoundingBox().top;
                hud += " W: " + p.getAgent().bodySpriter.getBoundingBox().width;
                hud += " H: " + p.getAgent().bodySpriter.getBoundingBox().height;

                //if (p.getAgent() != null)
                    //hud += "\nPos: " + p.getAgent().pos.X + ", " + p.getAgent().pos.Y;

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
