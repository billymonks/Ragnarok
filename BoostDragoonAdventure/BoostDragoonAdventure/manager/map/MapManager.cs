﻿using System;
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
        public String name, filename;
        public List<Connection> connections;

        public MapStats(String name, String filename, List<Connection> connections)
        {
            this.name = name;
            this.filename = filename;
            this.connections = connections;
        }
    }

    public class MapManager
    {
        private Game _game;
        public Dictionary<String, MapStats> atlas;

        public MapManager(Game game)
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

            XDocument doc = XDocument.Load(mapStats.filename);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");

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

            if (objects != null)
            {
                foreach (XElement e in objects.Elements("TURRET"))
                {
                    gm.factory.addTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        (Direction)int.Parse(e.Attribute("angle").Value));
                }

                foreach (XElement e in objects.Elements("AIM_TURRET"))
                {
                    gm.factory.addAimTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    gm.factory.addChest(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
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
                    gm.factory.addTimerTrigger(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)));
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

                foreach (XElement e in objects.Elements("MURDERER"))
                {
                    gm.factory.addMurderer(
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        new Vector2(24, 24), new Vector2(12, 12), true);
                }

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

                foreach (KeyValuePair<LayerType, Layer> pair in map.layerList)
                {
                    if (pair.Key.Equals(LayerType.WALL))
                        pair.Value.generateBodies(gm.w, map.width, map.height, true);
                    else
                        pair.Value.generateBodies(gm.w, map.width, map.height, false);
                }

                makeCircuits(map);

                gm.factory.processWorldChanges();

                connectTriggers(map);
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
                String name, fileName;
                List<Connection> connections = new List<Connection>();

                name = e.Attribute("name").Value;
                fileName = e.Attribute("filename").Value;

                foreach (XElement connection in e.Elements("connection"))
                {
                    connections.Add(
                        new Connection(connection.Value,
                            int.Parse(connection.Attribute("doorIndex").Value)));
                }

                MapStats temp = new MapStats(name, fileName, connections);


                atlas.Add(name, temp);
            }

        }

        private void loadSubMap(GameplayManager gm, Map map, String SUB_MAP_PATH, Point pos, Direction rotation, bool flipped)
        {
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
                            (Direction)((tempRotation + (int)rotation) % 360));

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

                    gm.factory.addTimerTrigger(
                        new Vector2(
                            320f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempX + pos.X,
                            240f + (float)Math.Cos(MathHelper.ToRadians((float)rotation)) * tempY + pos.Y));
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
        public void connectTriggers(Map map)
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
        }
    }
}
