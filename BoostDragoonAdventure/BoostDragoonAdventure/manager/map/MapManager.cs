using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using wickedcrush.map;
using wickedcrush.helper;
using FarseerPhysics.Dynamics;
using wickedcrush.manager.gameplay;
using wickedcrush.entity;
using wickedcrush.map.layer;
using wickedcrush.map.circuit;
using wickedcrush.manager.gameplay.room;
using wickedcrush.display._3d.texture;
using wickedcrush.map.path;

namespace wickedcrush.manager.map
{
    public struct Connection
    {
        public String mapName;
        public int doorIndex;

        public Connection(String mapName, int doorIndex)
        {
            this.mapName = mapName;
            this.doorIndex = doorIndex;
        }
    }

    public struct MapStats
    {
        public String name, filename, theme;
        public List<Connection> connections;

        public MapStats(String name, String filename, List<Connection> connections)
        {
            this.name = name;
            this.filename = filename;
            this.connections = connections;
            this.theme = "default";
        }

        public MapStats(String name, String filename, List<Connection> connections, String theme)
        {
            this.name = name;
            this.filename = filename;
            this.connections = connections;
            this.theme = theme;
        }
    }

    public class MapManager
    {
        private GameBase _game;
        public Dictionary<String, MapStats> atlas;

        public MapManager(GameBase game)
        {
            atlas = new Dictionary<String, MapStats>();
            LoadAtlas();
            _game = game;
        }

        public MapStats getMapStatsFromAtlas(String mapString)
        {
            return atlas[mapString];
        }

        public void LoadMap(GameplayManager gm, Map map, MapStats mapStats)
        {
            int doorCount = 0;
            int rank = 3;

            XDocument doc = XDocument.Load(mapStats.filename);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");
            XElement art1 = rootElement.Element("ART1");
            XElement art2 = rootElement.Element("ART2");

            map.name = mapStats.name;
            map.width = int.Parse(rootElement.Attribute("width").Value);
            map.height = int.Parse(rootElement.Attribute("height").Value);

            bool[,] data;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                map.addLayer(gm.w, data, LayerType.WALL);
            }
            else
            {
                map.addEmptyLayer(gm.w, LayerType.WALL);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                map.addLayer(gm.w, data, LayerType.DEATHSOUP);
            }
            else
            {
                map.addEmptyLayer(gm.w, LayerType.DEATHSOUP);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                map.addLayer(gm.w, data, LayerType.WIRING);
            }
            else
            {
                map.addEmptyLayer(gm.w, LayerType.WIRING);
            }

            if (art1 != null)
            {
                data = getLayerData(art1.Value);
                map.addLayer(gm.w, data, LayerType.ART1);
            }
            else
            {
                map.addEmptyLayer(gm.w, LayerType.ART1);
            }

            if (art2 != null)
            {
                data = getLayerData(art2.Value);
                map.addLayer(gm.w, data, LayerType.ART2);
            }
            else
            {
                map.addEmptyLayer(gm.w, LayerType.ART2);
            }

            if (objects != null)
            {
                foreach (XElement e in objects.Elements("ROOM"))
                {
                    if (gm.testMode)
                    {
                        loadSubMap(gm, map, gm._roomManager.getGameplayRoom(gm._screen.GetRoom()),
                        new Point(int.Parse(e.Attribute("x").Value),
                        int.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value), false);
                    }
                    else
                    {
                        loadSubMap(gm, map, getRandomRoom(),
                            new Point(int.Parse(e.Attribute("x").Value),
                            int.Parse(e.Attribute("y").Value)),
                            (Direction)int.Parse(e.Attribute("angle").Value), false);
                    }
                }

                foreach (XElement e in objects.Elements("ROOM_MIRROR"))
                {
                    loadSubMap(gm, map, getRandomRoom(),
                        new Point(int.Parse(e.Attribute("x").Value),
                        int.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value), true);
                }

                foreach (XElement e in objects.Elements("WALLTILE"))
                {
                    map.artTileList.Add(
                        new ArtTile(
                            new Vector2(float.Parse(e.Attribute("x").Value),
                                float.Parse(e.Attribute("y").Value)),
                            new Vector2(float.Parse(e.Attribute("width").Value),
                                float.Parse(e.Attribute("height").Value)),
                            e.Attribute("Color").Value,
                            e.Attribute("Normal").Value,
                            float.Parse(e.Attribute("Height").Value),
                            float.Parse(e.Attribute("angle").Value)));

                }

                foreach (XElement e in objects.Elements("TURRET"))
                {
                    rank = 3;

                    if (e.Attributes("rank").Count<XAttribute>() > 0)
                        rank = int.Parse(e.Attribute("rank").Value);

                    gm.factory.addTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value),
                        rank);
                }

                foreach (XElement e in objects.Elements("AIM_TURRET"))
                {
                    rank = 3;

                    if (e.Attributes("rank").Count<XAttribute>() > 0)
                        rank = int.Parse(e.Attribute("rank").Value);

                    gm.factory.addAimTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        rank);
                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    gm.factory.addChest(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("NPC"))
                {
                    gm.factory.addNPC(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(float.Parse(e.Attribute("width").Value), float.Parse(e.Attribute("height").Value)), e.Attribute("dialog").Value);
                }

                foreach (XElement e in objects.Elements("TERMINAL"))
                {
                    gm.factory.addTerminal(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("FLOOR_SWITCH"))
                {
                    gm.factory.addFloorSwitch(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("TIMER"))
                {
                    int time = 1500;

                    if (e.Attribute("time") != null)
                        time = int.Parse(e.Attribute("time").Value);

                    gm.factory.addTimerTrigger(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        time);
                }

                foreach (XElement e in objects.Elements("DOOR"))
                {
                    gm.factory.addDoor(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value),
                        mapStats.connections[doorCount]);

                    doorCount++;
                }

                foreach (XElement e in objects.Elements("TEST_DESTINATION"))
                {
                    gm.factory.addDestination(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("PATH_OF_DEATH"))
                {
                    Stack<PathNode> patrol = new Stack<PathNode>();
                    //int angle = 0;

                    foreach (XElement n in e.Elements("node"))
                    {
                        patrol.Push(new PathNode(new Point(int.Parse(n.Attribute("x").Value) / 10, int.Parse(n.Attribute("y").Value) / 10), 10));
                    }
                    patrol.Reverse<PathNode>();

                    //if (e.Attribute("angle") != null)
                    //{
                        //angle = int.Parse(e.Attribute("angle").Value);
                    //}

                    /*gm.factory.addMurderer(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(float.Parse(e.Attribute("width").Value), float.Parse(e.Attribute("height").Value)), true, patrol, angle);*/
                    gm.factory.addPathOfDeath(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(float.Parse(e.Attribute("width").Value), float.Parse(e.Attribute("height").Value)), patrol);
                }

                foreach (XElement e in objects.Elements("CACTUS"))
                {
                    Stack<PathNode> patrol = new Stack<PathNode>();
                    //int angle = 0;

                    foreach (XElement n in e.Elements("node"))
                    {
                        patrol.Push(new PathNode(new Point(int.Parse(n.Attribute("x").Value) / 10, int.Parse(n.Attribute("y").Value) / 10), 10));
                    }
                    patrol.Reverse<PathNode>();

                    //if (e.Attribute("angle") != null)
                    //{
                    //angle = int.Parse(e.Attribute("angle").Value);
                    //}

                    /*gm.factory.addMurderer(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(float.Parse(e.Attribute("width").Value), float.Parse(e.Attribute("height").Value)), true, patrol, angle);*/
                    gm.factory.addCactus(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(30, 30), 1, patrol);
                }

                foreach (XElement e in objects.Elements("MURDERER"))
                {
                    Stack<PathNode> patrol = new Stack<PathNode>();
                    int angle = 0;

                    foreach (XElement n in e.Elements("node"))
                    {
                        patrol.Push(new PathNode(new Point(int.Parse(n.Attribute("x").Value) / 10, int.Parse(n.Attribute("y").Value) / 10), 10));
                    }
                    patrol.Reverse<PathNode>();

                    if (e.Attribute("angle") != null)
                    {
                        angle = int.Parse(e.Attribute("angle").Value);
                    }

                    gm.factory.addMurderer(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(float.Parse(e.Attribute("width").Value), float.Parse(e.Attribute("height").Value)), true, patrol, angle);
                }

                foreach (XElement e in objects.Elements("WEAKLING"))
                {
                    Stack<PathNode> patrol = new Stack<PathNode>();
                    gm.factory.addWeakling(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(12, 12), new Vector2(6, 6), patrol);
                }

                foreach (XElement e in objects.Elements("SHIFTYSHOOTER"))
                {
                    gm.factory.addShiftyShooter(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(24, 24), new Vector2(12, 12),
                        int.Parse(e.Attribute("spreadDuration").Value), int.Parse(e.Attribute("blowCount").Value), int.Parse(e.Attribute("blowPerSpread").Value),
                        int.Parse(e.Attribute("scatterCount").Value), int.Parse(e.Attribute("spread").Value), float.Parse(e.Attribute("blowVelocity").Value), int.Parse(e.Attribute("blowDuration").Value),
                        int.Parse(e.Attribute("blowReleaseDelay").Value), int.Parse(e.Attribute("moveLength").Value), int.Parse(e.Attribute("standLength").Value), int.Parse(e.Attribute("standToShootLength").Value),
                        int.Parse(e.Attribute("skillVelocity").Value));
                }

                foreach (XElement e in objects.Elements("GIANT"))
                {
                    gm.factory.addGiant(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                        
                }

                foreach (XElement e in objects.Elements("BOSS"))
                {
                    if (int.Parse(e.Attribute("Type").Value) == 0)
                    {
                        gm.factory.addCentipede(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                    }
                    else if(int.Parse(e.Attribute("Type").Value) == 1)
                    {
                        gm.factory.addChimera(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                    } else if(int.Parse(e.Attribute("Type").Value) == 2)
                    {
                        gm.factory.addWoundedCentipede(new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                    }
                }

                

                foreach (KeyValuePair<LayerType, Layer> pair in map.layerList)
                {
                    if (pair.Key.Equals(LayerType.WALL))
                        pair.Value.generateLayerBody(gm.w, map.width, map.height, true);
                    else if (pair.Key.Equals(LayerType.WIRING))
                    {
                        pair.Value.generateBodies(gm.w, map.width, map.height, false);
                    }
                    else
                        pair.Value.generateLayerBody(gm.w, map.width, map.height, false);
                    
                }

                //makeCircuits(map);

                gm.factory.processWorldChanges();

                //connectTriggers(map);
            }
        }

        private void LoadAtlas()
        {
            atlas.Clear();

            String path = "Content/maps/atlas/MapAtlas.xml";
            XDocument doc = XDocument.Load(path);
            XElement rootElement = new XElement(doc.Element("atlas"));

            foreach (XElement e in rootElement.Elements("map"))
            {
                String name, fileName, theme;
                List<Connection> connections = new List<Connection>();

                name = e.Attribute("name").Value;
                fileName = e.Attribute("filename").Value;
                if (e.Attribute("theme") != null)
                {
                    theme = e.Attribute("theme").Value;
                }
                else
                {
                    theme = "default";
                }

                foreach (XElement connection in e.Elements("connection"))
                {
                    connections.Add(
                        new Connection(connection.Value,
                            int.Parse(connection.Attribute("doorIndex").Value)));
                }

                MapStats temp = new MapStats(name, fileName, connections, theme);


                atlas.Add(name, temp);
            }

        }

        private void loadSubMap(GameplayManager gm, Map map, String SUB_MAP_PATH, Point pos, Direction rotation, bool flipped)
        {
            int rank = 3;

            XDocument doc = XDocument.Load(SUB_MAP_PATH);

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
                    rank = 3;

                    if (e.Attributes("rank").Count<XAttribute>() > 0)
                        rank = int.Parse(e.Attribute("rank").Value);

                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;
                    tempRotation = int.Parse(e.Attribute("angle").Value);

                    if (flipped)
                    {
                        tempX *= -1;
                        if (tempRotation % 180 == 0)
                            tempRotation += 180;
                    }

                    gm.factory.addTurret(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y),
                            (Direction)((tempRotation + (int)rotation) % 360),
                            rank);

                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    tempX = float.Parse(e.Attribute("x").Value) - 320f;
                    tempY = float.Parse(e.Attribute("y").Value) - 240f;

                    if (flipped)
                        tempX *= -1;

                    gm.factory.addChest(
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

                    gm.factory.addFloorSwitch(
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

                    int time = 1500;

                    if (e.Attribute("time") != null)
                        time = int.Parse(e.Attribute("time").Value);

                    gm.factory.addTimerTrigger(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y),
                            time);
                }
            }
        }

        

        private String getRandomRoom()
        {
            if (_game.roomManager.onlineAtlas.Count > 0)
            {
                return _game.roomManager.getRandomOnlineRoom();
            }
            else
            {
                return _game.roomManager.getRandomOfflineRoom();
            }
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


        //perverse circuit logic starts here. beware. move to somewhere smarter someday.
        /*public void connectTriggers(Map map)
        {
            foreach (Circuit c in map.circuitList)
                c.connectTriggers();
        }

        private void makeCircuits(Map map)
        {
            map.circuitList = new List<Circuit>();

            Layer wiringLayer = map.getLayer(LayerType.WIRING);

            for (int i = 0; i < wiringLayer.bodyList.GetLength(0); i++)
                for (int j = 0; j < wiringLayer.bodyList.GetLength(1); j++)
                {
                    getConnections(map, wiringLayer, i, j);
                }
        }

        private void getConnections(Map map, Layer wiring, int x, int y)
        {
            List<Body> openList = new List<Body>();
            List<Body> closedList = new List<Body>();

            if (wiring.getCoordinate(x, y) && !inCircuit(map, wiring.bodyList[x, y]))
            {
                addCircuit(map, wiring, x, y);
            }
        }

        private void addCircuit(Map map, Layer wiring, int x, int y)
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

        private bool inCircuit(Map map, Body wire)
        {
            if (map.circuitList == null)
                return false;

            foreach (Circuit c in map.circuitList)
            {
                if (c.contains(wire))
                    return true;
            }

            return false;
        }*/
    }
}
