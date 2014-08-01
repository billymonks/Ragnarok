﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.layer;
using System.Xml.Linq;
using wickedcrush.entity;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.editor;

namespace wickedcrush.editor
{
    public class EditorMap
    {
        public String name;
        public int width, height;
        public Dictionary<LayerType, int[,]> layerList;
        public List<EditorEntity> entityList;

        public EditorEntityFactory factory;

        public EditorMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            name = "Untitled";

            layerList = new Dictionary<LayerType, int[,]>();
            entityList = new List<EditorEntity>();
            createEmptyLayers();

            factory = new EditorEntityFactory(this);
        }

        public EditorMap(String MAP_NAME)
        {
            layerList = new Dictionary<LayerType, int[,]>();
            entityList = new List<EditorEntity>();

            factory = new EditorEntityFactory(this);

            loadMap(MAP_NAME);
        }

        public void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch sb, SpriteFont f, Point offset)
        {
            debugDrawLayer(tex, sb, LayerType.WALL, Color.Black, offset, 20);
            debugDrawLayer(tex, sb, LayerType.DEATHSOUP, Color.Red, offset, 20);
            debugDrawLayer(tex, sb, LayerType.WIRING, Color.Purple, offset, 10);

            debugDrawEntities(tex, gd, sb, f, offset);
        }

        private void debugDrawLayer(Texture2D tex, SpriteBatch sb, LayerType t, Color c, Point offset, int gridSize)
        {
            if (!layerList.ContainsKey(t))
                return;
                
            int[,] data = layerList[t];
            for (int i = 0; i < data.GetLength(0); i++) {
                for (int j = 0; j < data.GetLength(1); j++) {
                    if (data[i, j] == 1)
                        sb.Draw(tex, new Rectangle(i * gridSize + offset.X, j * gridSize + offset.Y, gridSize, gridSize), c);
                }
            }
        }

        private void debugDrawEntities(Texture2D tex, GraphicsDevice gd, SpriteBatch sb, SpriteFont f, Point offset)
        {
            foreach (EditorEntity e in entityList)
            {
                e.DebugDraw(tex, null, gd, sb, f, Color.Green);
            }
        }

        public bool layerCollision(EditorEntity entity, LayerType type) // needs work, all messed up
        {
            Point start, end;
            int[,] layer = layerList[type];

            start = new Point((int) (entity.pos.X / layer.GetLength(0)), (int) (entity.pos.Y / layer.GetLength(1)));
            end = new Point((int) (entity.pos.X+entity.size.X) / layer.GetLength(0), (int) (entity.pos.Y + entity.size.Y) / layer.GetLength(1));

            for (int i = start.X; i <= end.X; i++)
            { for (int j = start.Y; j <= end.Y; j++)
            {
                if (i < 0 || j < 0 || i >= layer.GetLength(0) || j >= layer.GetLength(1))
                    return true;

                if(layer[i,j] == 1)
                    return true;
            }
            }

            return false;
        }

        private void createEmptyLayers()
        {
            createWallLayer();
            createDeathSoupLayer();
            createWiringLayer();
            createEntityLayer();
        }

        private void createWallLayer()
        {
            layerList.Add(LayerType.WALL, getEmptyLayer(20));
        }

        private void createDeathSoupLayer()
        {
            layerList.Add(LayerType.DEATHSOUP, getEmptyLayer(20));
        }

        private void createWiringLayer()
        {
            layerList.Add(LayerType.WIRING, getEmptyLayer(10));
        }

        private void createEntityLayer()
        {
            layerList.Add(LayerType.ENTITY, getEmptyLayer(10)); //internal
        }

        private int[,] getEmptyLayer(int gridSize)
        {
            int[,] tempLayer = new int[width / gridSize, height / gridSize];

            for (int i = 0; i < tempLayer.GetLength(0); i++)
                for (int j = 0; j < tempLayer.GetLength(1); j++)
                    tempLayer[i, j] = 0;

            return tempLayer;
        }

        private void saveMap(String MAP_NAME)
        {
            XDocument doc = new XDocument();

            XElement level = new XElement("level");

            XElement walls = new XElement("WALLS");
            XElement deathSoup = new XElement("DEATHSOUP");
            XElement wiring = new XElement("WIRING");
            XElement objects = new XElement("OBJECTS");

            level.Add(new XAttribute("width", width));
            level.Add(new XAttribute("height", height));

            walls.Value = getLayerString(layerList[LayerType.WALL]);
            walls.Add(new XAttribute("exportMode", "Bitstring"));

            deathSoup.Value = getLayerString(layerList[LayerType.DEATHSOUP]);
            deathSoup.Add(new XAttribute("exportMode", "Bitstring"));

            wiring.Value = getLayerString(layerList[LayerType.WIRING]);
            wiring.Add(new XAttribute("exportMode", "Bitstring"));

            XElement entity;

            foreach (EditorEntity e in entityList)
            {
                entity = new XElement(e.code);
                entity.Add(new XAttribute("x", (int)e.pos.X));
                entity.Add(new XAttribute("y", (int)e.pos.Y));
                entity.Add(new XAttribute("angle", e.angle));
            }

            level.Add(walls);
            level.Add(deathSoup);
            level.Add(wiring);
            level.Add(objects);

            doc.Add(level);

            doc.Save(MAP_NAME);
        }

        private void loadMap(String MAP_NAME)
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

            int[,] data;
            EditorEntity editorEntity;

            if (walls != null)
            {
                data = getLayerData(walls.Value);
                layerList.Add(LayerType.WALL, data);
            }

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                layerList.Add(LayerType.DEATHSOUP, data);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                layerList.Add(LayerType.WIRING, data);
            }
            else
            {

            }

            createEntityLayer();

            if (objects != null)
            {
                foreach (XElement e in objects.Elements())
                {
                    Direction angle = (Direction)0;

                    if (null != e.Attribute("angle"))
                        angle = (Direction)int.Parse(e.Attribute("angle").Value);

                    factory.AddEntity(
                        e.Name.LocalName,
                        new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)),
                        angle);

                    //editorEntity = new EditorEntity(e.Name.LocalName, e.Name.LocalName, new Vector2(float.Parse(e.Attribute("x").Value), float.Parse(e.Attribute("y").Value)), angle);
                    //entityList.Add(editorEntity);
                }
            }
        }

        private int[,] getLayerData(String s)
        {
            char[] separator = new char[1];
            separator[0] = '\n';
            String[] splitString = s.Split(separator);

            int[,] data = new int[splitString[0].Length, splitString.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = int.Parse(splitString[j].ToArray()[i].ToString());

            return data;
        }

        private String getLayerString(int[,] data)
        {
            String result = "";

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    result += data[i, j];
                }

                if(i != data.GetLength(0)-1)
                    result += '\n';
            }

            return result;
        }
    }
}
