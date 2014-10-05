using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using wickedcrush.entity;
using wickedcrush.factory.entity;
using wickedcrush.helper;
using wickedcrush.manager.audio;
using wickedcrush.manager.controls;
using wickedcrush.manager.entity;
using wickedcrush.manager.player;
using wickedcrush.map;
using wickedcrush.map.circuit;
using wickedcrush.map.layer;
using wickedcrush.stats;

namespace wickedcrush.manager.map.room
{
    public struct MapStats
    {
        public String filename;
        public List<String> connections;

        public MapStats(String filename, params String[] connections)
        {
            this.filename = filename;
            this.connections = new List<String>();

            foreach (String s in connections)
            {
                this.connections.Add(s);
            }
        }
    }

    public class MapManager
    {
        public Dictionary<String, MapStats> atlas;
        public Map map;

        public EntityManager entityManager;
        public SoundManager soundManager;
        public PlayerManager playerManager; //replace with panelManager
        public MapManager mapManager;

        public RoomManager roomManager;

        public EntityFactory factory;


        public World w;

        public MapManager(Game game)
        {
            w = new World(Vector2.Zero);
            w.Gravity = Vector2.Zero;

            soundManager = new SoundManager(game.Content);
            entityManager = new EntityManager(game);
            playerManager = game.playerManager;

            factory = new EntityFactory(entityManager, game.playerManager, this, game.controlsManager, soundManager, w);
            LoadAtlas();
        }
        
        private void LoadAtlas()
        {

        }

        private void loadSubMap(String SUB_MAP_NAME, Point pos, Direction rotation, bool flipped)
        {
            XDocument doc = XDocument.Load(SUB_MAP_NAME);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");

            pos.X -= 320;
            pos.Y -= 240;

            float tempX, tempY;
            int tempRotation;

            bool[,] data;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                map.addRoomLayer(pos, data, LayerType.WALL, rotation, flipped);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                map.addRoomLayer(pos, data, LayerType.DEATHSOUP, rotation, flipped);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                map.addRoomLayer(pos, data, LayerType.WIRING, rotation, flipped);
            }

            if (objects != null)
            {
                foreach (XElement e in objects.Elements("TURRET"))
                {
                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;
                    tempRotation = int.Parse(e.Attribute("angle").Value);

                    if (flipped)
                    {
                        tempX *= -1;
                        if (tempRotation % 180 == 0)
                            tempRotation += 180;
                    }

                    factory.addTurret(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y),
                            (Direction)((tempRotation + (int)rotation) % 360));

                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;

                    if (flipped)
                        tempX *= -1;

                    factory.addChest(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y));
                }

                foreach (XElement e in objects.Elements("FLOOR_SWITCH"))
                {
                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;

                    if (flipped)
                        tempX *= -1;

                    factory.addFloorSwitch(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y));
                }

                foreach (XElement e in objects.Elements("TIMER"))
                {
                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;

                    if (flipped)
                        tempX *= -1;

                    factory.addTimerTrigger(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y));
                }
            }
        }

        public void loadMap(String MAP_NAME, World w)
        {
            map = new Map(MAP_NAME, w, this);

            XDocument doc = XDocument.Load(MAP_NAME);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");

            map.name = MAP_NAME;
            map.width = int.Parse(rootElement.Attribute("width").Value);
            map.height = int.Parse(rootElement.Attribute("height").Value);


            bool[,] data;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                map.addLayer(w, data, LayerType.WALL);
            }
            else
            {
                map.addEmptyLayer(w, LayerType.WALL);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                map.addLayer(w, data, LayerType.DEATHSOUP);
            }
            else
            {
                map.addEmptyLayer(w, LayerType.DEATHSOUP);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                map.addLayer(w, data, LayerType.WIRING);
            }
            else
            {
                map.addEmptyLayer(w, LayerType.WIRING);
            }

            if (objects != null)
            {
                foreach (XElement e in objects.Elements("TURRET"))
                {
                    factory.addTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value));
                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    factory.addChest(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("FLOOR_SWITCH"))
                {
                    factory.addFloorSwitch(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("TIMER"))
                {
                    factory.addTimerTrigger(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("DOOR"))
                {
                    factory.addDoor(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value));
                    //put dis shit in factory ffs
                    //doorList.Add(new Door(w, new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)), (Direction)int.Parse(e.Attribute("angle").Value), factory, null));
                }

                foreach (XElement e in objects.Elements("MURDERER"))
                {
                    factory.addMurderer(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(24, 24), new Vector2(12, 12), true, new PersistedStats(20, 20));
                }

                foreach (XElement e in objects.Elements("ROOM"))
                {
                    loadSubMap(factory.rm.getRandomRoom(),
                        new Point(int.Parse(e.Attribute("x").Value),
                        int.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value), false);
                }

                foreach (XElement e in objects.Elements("ROOM_MIRROR"))
                {
                    loadSubMap(factory.rm.getRandomRoom(),
                        new Point(int.Parse(e.Attribute("x").Value),
                        int.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value), true);
                }

                foreach (KeyValuePair<LayerType, Layer> pair in map.layerList)
                {
                    if (pair.Key.Equals(LayerType.WALL))
                        pair.Value.generateBodies(w, map.width, map.height, true);
                    else
                        pair.Value.generateBodies(w, map.width, map.height, false);
                }

                makeCircuits();

                factory.processWorldChanges();

                connectTriggers();
            }
        }

        //perverse circuit logic starts here. beware. move to somewhere smarter someday.
        public void connectTriggers()
        {
            foreach (Circuit c in map.circuitList)
                c.connectTriggers();
        }

        private void makeCircuits()
        {
            map.circuitList = new List<Circuit>();

            Layer wiringLayer = map.getLayer(LayerType.WIRING);

            for (int i = 0; i < wiringLayer.bodyList.GetLength(0); i++)
                for (int j = 0; j < wiringLayer.bodyList.GetLength(1); j++)
                {
                    getConnections(wiringLayer, i, j);
                }
        }

        private void getConnections(Layer wiring, int x, int y)
        {
            List<Body> openList = new List<Body>();
            List<Body> closedList = new List<Body>();

            if (wiring.getCoordinate(x, y) && !inCircuit(wiring.bodyList[x, y]))
            {
                addCircuit(wiring, x, y);
            }
        }

        private void addCircuit(Layer wiring, int x, int y)
        {
            Circuit c = new Circuit();

            addConnectionsToCircuit(c, wiring, x, y);

            map.circuitList.Add(c);
        }

        private void addConnectionsToCircuit(Circuit c, Layer wiring, int x, int y)
        {
            if (x >= 0 && x < wiring.getWidth() && y >= 0 && y < wiring.getHeight() && wiring.getCoordinate(x, y) && !c.contains(wiring.bodyList[x, y]))
            {
                c.addWiring(wiring.bodyList[x, y]);

                addConnectionsToCircuit(c, wiring, x - 1, y);
                addConnectionsToCircuit(c, wiring, x + 1, y);
                addConnectionsToCircuit(c, wiring, x, y - 1);
                addConnectionsToCircuit(c, wiring, x, y + 1);
            }
        }

        private bool inCircuit(Body wire)
        {
            if (map.circuitList == null)
                return false;

            foreach (Circuit c in map.circuitList)
            {
                if (c.contains(wire))
                    return true;
            }

            return false;
        }

        private bool[,] getLayerData(String s)
        {
            char[] separator = new char[1];
            separator[0] = '\n';
            String[] splitString = s.Split(separator);

            bool[,] data = new bool[splitString[0].Length, splitString.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = Helper.CharToBool(splitString[j].ToArray()[i]);

            return data;
        }

    }

}
