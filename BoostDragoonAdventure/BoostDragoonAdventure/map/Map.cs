﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.layer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using System.Xml.Linq;
using wickedcrush.map.path;
using wickedcrush.factory.entity;
using wickedcrush.entity;
using wickedcrush.map.circuit;

namespace wickedcrush.map
{
    public class Map
    {

        public int width, height;
        public Dictionary<LayerType, Layer> layerList;
        public List<Circuit> circuitList;
        public String name;

        private EntityFactory factory;

        public Map(String MAP_NAME, World w, EntityFactory factory)
        {
            layerList = new Dictionary<LayerType, Layer>();
            this.factory = factory;
            loadMap(MAP_NAME, w);
        }

        public void addLayer(World w, Boolean[,] data, LayerType layerType) // need map factory for this?
        {
            if(layerType == LayerType.WALL)
                layerList.Add(layerType, new Layer(data, w, width, height, true, LayerType.WALL));
            else
                layerList.Add(layerType, new Layer(data, w, width, height, false, layerType));
        }

        public void addRoomLayer(Point pos, Boolean[,] data, LayerType layerType)
        {
            int gridSize = layerList[layerType].getGridSize();
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    layerList[layerType].data[i + pos.X / gridSize, j + pos.Y / gridSize] = data[i, j];
                }
            }
        }

        public void addEmptyLayer(World w, LayerType layerType)
        {
            Boolean[,] emptyData = new Boolean[1, 1];
            emptyData[0, 0] = false;

            layerList.Add(layerType, new Layer(emptyData, w, width, height, false, layerType));
        }

        public Layer getLayer(LayerType layerType)
        {
            return layerList[layerType];
        }

        private void loadSubMap(String SUB_MAP_NAME, Point pos)
        {
            XDocument doc = XDocument.Load(SUB_MAP_NAME);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");

            pos.X -= 320;
            pos.Y -= 240;

            bool[,] data;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                addRoomLayer(pos, data, LayerType.WALL);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                addRoomLayer(pos, data, LayerType.DEATHSOUP);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                addRoomLayer(pos, data, LayerType.WIRING);
            }

            if (objects != null)
            {
                foreach (XElement e in objects.Elements("TURRET"))
                {
                    factory.addTurret(
                        new Vector2(float.Parse(e.Attribute("x").Value) + pos.X, float.Parse(e.Attribute("y").Value) + pos.Y),
                        (Direction)int.Parse(e.Attribute("angle").Value));
                }

                foreach (XElement e in objects.Elements("CHEST"))
                {
                    factory.addChest(
                        new Vector2(float.Parse(e.Attribute("x").Value) + pos.X, float.Parse(e.Attribute("y").Value) + pos.Y));
                }

                foreach (XElement e in objects.Elements("FLOOR_SWITCH"))
                {
                    factory.addFloorSwitch(
                        new Vector2(float.Parse(e.Attribute("x").Value) + pos.X, float.Parse(e.Attribute("y").Value) + pos.Y));
                }

                foreach (XElement e in objects.Elements("TIMER"))
                {
                    factory.addTimerTrigger(
                        new Vector2(float.Parse(e.Attribute("x").Value) + pos.X, float.Parse(e.Attribute("y").Value) + pos.Y));
                }
            }
        }

        private void loadMap(String MAP_NAME, World w)
        {
            XDocument doc = XDocument.Load(MAP_NAME);

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");
            XElement wiring = rootElement.Element("WIRING");
            XElement objects = rootElement.Element("OBJECTS");

            this.name = MAP_NAME;
            this.width = int.Parse(rootElement.Attribute("width").Value);
            this.height = int.Parse(rootElement.Attribute("height").Value);


            bool[,] data;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                addLayer(w, data, LayerType.WALL);
            }
            else
            {
                addEmptyLayer(w, LayerType.WALL);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                addLayer(w, data, LayerType.DEATHSOUP);
            }
            else
            {
                addEmptyLayer(w, LayerType.DEATHSOUP);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                addLayer(w, data, LayerType.WIRING);
            }
            else
            {
                addEmptyLayer(w, LayerType.WIRING);
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

                foreach (XElement e in objects.Elements("ROOM"))
                {
                    loadSubMap("Content/maps/small/Temple Halls V4.xml", new Point(int.Parse(e.Attribute("x").Value), int.Parse(e.Attribute("y").Value)));
                }

                foreach (KeyValuePair<LayerType, Layer> pair in layerList)
                {
                    if (pair.Key.Equals(LayerType.WALL))
                        pair.Value.generateBodies(w, width, height, true);
                    else
                        pair.Value.generateBodies(w, width, height, false);
                }

                makeCircuits();

                factory.processWorldChanges();

                connectTriggers();
            }
        }

        //perverse circuit logic starts here. beware. move to somewhere smarter someday.
        public void connectTriggers()
        {
            foreach(Circuit c in circuitList)
                c.connectTriggers();
        }

        private void makeCircuits()
        {
            circuitList = new List<Circuit>();

            Layer wiringLayer = getLayer(LayerType.WIRING);

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

            circuitList.Add(c);
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
            if (circuitList == null)
                return false;

            foreach (Circuit c in circuitList)
            {
                if (c.contains(wire))
                    return true;
            }

            return false;
        }

        //perverse circuit logic ends here

        private bool[,] getLayerData(String s)
        {
            char[] separator = new char[1];
            separator[0] = '\n';
            String[] splitString = s.Split(separator);

            bool[,] data = new bool[splitString[0].Length, splitString.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = CharToBool(splitString[j].ToArray()[i]);

            return data;
        }

        private bool CharToBool(Char c)
        {
            return (c == '1');
        }

        public void DebugDraw(Texture2D whiteTexture, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            foreach (Body b in getLayer(LayerType.WALL).bodyList)
            {
                if(b!=null)
                    spriteBatch.Draw(whiteTexture, b.Position, null, Color.Black, b.Rotation, Vector2.Zero, new Vector2(width / getLayer(LayerType.WALL).getWidth(), height / getLayer(LayerType.WALL).getHeight()), SpriteEffects.None, 0f);
            }

            foreach (Body b in getLayer(LayerType.DEATHSOUP).bodyList)
            {
                if (b != null)
                    spriteBatch.Draw(whiteTexture, b.Position, null, Color.Red, b.Rotation, Vector2.Zero, new Vector2(width / getLayer(LayerType.DEATHSOUP).getWidth(), height / getLayer(LayerType.DEATHSOUP).getHeight()), SpriteEffects.None, 0f);
            }

            foreach (Body b in getLayer(LayerType.WIRING).bodyList)
            {
                if (b != null)
                    spriteBatch.Draw(whiteTexture, b.Position, null, Color.Purple, b.Rotation, Vector2.Zero, new Vector2(width / getLayer(LayerType.WIRING).getWidth(), height / getLayer(LayerType.WIRING).getHeight()), SpriteEffects.None, 0f);
            }
        }
    }
}
