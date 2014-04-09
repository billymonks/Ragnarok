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

namespace wickedcrush.map
{
    public class Map
    {

        public int width, height;
        public Dictionary<LayerType, Layer> layerList;
        public PathNode[,] pathNodeGrid;
        public String name;

        public Map(String MAP_NAME, World w)
        {
            layerList = new Dictionary<LayerType, Layer>();

            loadMap(MAP_NAME, w);
        }

        public void addLayer(World w, Boolean[,] data, LayerType layerType)
        {
            if(layerType == LayerType.WALL)
                layerList.Add(layerType, new Layer(data, w, width, height, true));
            else
                layerList.Add(layerType, new Layer(data, w, width, height, false));
            
            //generateWalls(w);
        }

        public Layer getLayer(LayerType layerType)
        {
            return layerList[layerType];
        }

        private void loadMap(String MAP_NAME, World w)
        {
            XDocument doc = XDocument.Load("maps/" + MAP_NAME + ".xml");

            XElement rootElement = new XElement(doc.Element("level"));
            XElement walls = rootElement.Element("WALLS");
            XElement deathSoup = rootElement.Element("DEATHSOUP");

            this.name = MAP_NAME;
            this.width = int.Parse(rootElement.Attribute("width").Value);
            this.height = int.Parse(rootElement.Attribute("height").Value);


            bool[,] data = getLayerData(walls.Value);
            addLayer(w, data, LayerType.WALL);

            data = getLayerData(deathSoup.Value);
            addLayer(w, data, LayerType.DEATH_SOUP);



            loadPathNodeGrid();
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

        private void loadPathNodeGrid()
        {
            Layer l = getLayer(LayerType.WALL);
            Layer ds = getLayer(LayerType.DEATH_SOUP);

            pathNodeGrid = new PathNode[l.getWidth() * 2, l.getHeight() * 2];

            for(int i = 0; i < l.getWidth(); i++) {
                for(int j = 0; j < l.getHeight(); j++) {
                    if (!l.getCoordinate(i, j) && !ds.getCoordinate(i, j)) {
                        pathNodeGrid[i * 2, j * 2] = getNodeFromCoordinate(i * 2, j * 2);
                        pathNodeGrid[i * 2 + 1, j * 2] = getNodeFromCoordinate(i * 2 + 1, j * 2);
                        pathNodeGrid[i * 2, j * 2 + 1] = getNodeFromCoordinate(i * 2, j * 2 + 1);
                        pathNodeGrid[i * 2 + 1, j * 2 + 1] = getNodeFromCoordinate(i * 2 + 1, j * 2 + 1);
                    }
                }
            }
        }

        private PathNode getNodeFromCoordinate(int x, int y)
        {
            return new PathNode(
                new Vector2(
                    (width / pathNodeGrid.GetLength(0)) * x,
                    (height / pathNodeGrid.GetLength(1)) * y));
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

            /*for (int i = 0; i < pathNodeGrid.GetLength(0); i++)
            {
                for (int j = 0; j < pathNodeGrid.GetLength(1); j++)
                {
                    if (pathNodeGrid[i, j] != null)
                    {
                        spriteBatch.Draw(whiteTexture,
                            new Vector2(i * (width / pathNodeGrid.GetLength(0)), j * (height / pathNodeGrid.GetLength(1))),
                            null,
                            Color.Green,
                            0f,
                            Vector2.Zero,
                            new Vector2(width / pathNodeGrid.GetLength(0), height / pathNodeGrid.GetLength(1)),
                            SpriteEffects.None,
                            0f);
                    }
                }
            }*/
        }
    }
}
