using System;
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

namespace wickedcrush.map
{
    public class Map
    {

        public int width, height;
        public Dictionary<LayerType, Layer> layerList;
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

        public Layer getLayer(LayerType layerType)
        {
            return layerList[layerType];
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

            if (deathSoup != null)
            {
                data = getLayerData(deathSoup.Value);
                addLayer(w, data, LayerType.DEATH_SOUP);
            }

            if (wiring != null)
            {
                data = getLayerData(wiring.Value);
                addLayer(w, data, LayerType.WIRING);
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
                    data[i, j] = CharToBool(splitString[j].ToArray()[i]);

            return data;
        }

        private bool CharToBool(Char c)
        {
            return (c == '1');
        }

        public void drawMap(GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            Texture2D whiteTexture = new Texture2D(gd, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            whiteTexture.SetData(data);

            foreach (Body b in getLayer(LayerType.WALL).bodyList)
            {
                spriteBatch.Draw(whiteTexture, b.Position, null, Color.Black, b.Rotation, Vector2.Zero, new Vector2(width / getLayer(LayerType.WALL).getWidth(), height / getLayer(LayerType.WALL).getHeight()), SpriteEffects.None, 0f);
            }

            foreach (Body b in getLayer(LayerType.DEATH_SOUP).bodyList)
            {
                spriteBatch.Draw(whiteTexture, b.Position, null, Color.Red, b.Rotation, Vector2.Zero, new Vector2(width / getLayer(LayerType.DEATH_SOUP).getWidth(), height / getLayer(LayerType.DEATH_SOUP).getHeight()), SpriteEffects.None, 0f);
            }
        }
    }
}
