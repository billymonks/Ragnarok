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
using wickedcrush.map.circuit;
using wickedcrush.display._3d;
using wickedcrush.helper;
using wickedcrush.stats;
using wickedcrush.entity.physics_entity.agent.inanimate;
using wickedcrush.manager.map.room;

namespace wickedcrush.map
{
    public class Map
    {

        public int width, height;
        public Dictionary<LayerType, Layer> layerList;
        public List<Circuit> circuitList;
        public List<Door> doorList;
        public String name;

        private MapManager mapManager;

        public Map(String MAP_NAME, World w, MapManager mapManager)
        {
            layerList = new Dictionary<LayerType, Layer>();
            this.mapManager = mapManager;
            //mapManager.map = this;
            //factory.setMap(this);
            doorList = new List<Door>();
            //mapManager.loadMap(MAP_NAME, w);
        }

        public void addLayer(World w, Boolean[,] data, LayerType layerType) // need map factory for this?
        {
            if(layerType == LayerType.WALL)
                layerList.Add(layerType, new Layer(data, w, width, height, true, LayerType.WALL));
            else
                layerList.Add(layerType, new Layer(data, w, width, height, false, layerType));
        }

        public void addRoomLayer(Point pos, Boolean[,] data, LayerType layerType, Direction rotation, bool flipped)
        {
            int gridSize = layerList[layerType].getGridSize();
            
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (flipped)
                    {
                        if (rotation == Direction.East)
                            layerList[layerType].data[i + pos.X / gridSize, j + pos.Y / gridSize] = data[data.GetLength(0) - 1 - i, j];
                        if (rotation == Direction.West)
                            layerList[layerType].data[i + pos.X / gridSize, j + pos.Y / gridSize] = data[i, data.GetLength(1) - 1 - j];
                    } else {
                        if(rotation==Direction.East)
                           layerList[layerType].data[i + pos.X / gridSize, j + pos.Y / gridSize] = data[i, j];
                        if(rotation==Direction.West)
                            layerList[layerType].data[i + pos.X / gridSize, j + pos.Y / gridSize] = data[data.GetLength(0) - 1 - i, data.GetLength(1) - 1 - j];
                    }
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


        public void DebugDraw(Texture2D whiteTexture, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Camera camera)
        {
            for (int i = (int)(camera.cameraPosition.X / 20f) - 6; i < (int)(camera.cameraPosition.X / 20f) + 40; i++)
            {
                for (int j = (int)(camera.cameraPosition.Y / 20f); j < (int)(camera.cameraPosition.Y / 20f) + 25; j++)
                {
                    if (i >= 0 && i < getLayer(LayerType.WALL).getWidth() && j >= 0 && j < getLayer(LayerType.WALL).getHeight())
                    {
                        if(getLayer(LayerType.WALL).bodyList[i, j] != null)
                            spriteBatch.Draw(whiteTexture,
                                getLayer(LayerType.WALL).bodyList[i, j].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y),
                                null,
                                Color.Black,
                                getLayer(LayerType.WALL).bodyList[i, j].Rotation,
                                Vector2.Zero,
                                new Vector2(width / getLayer(LayerType.WALL).getWidth(), height / getLayer(LayerType.WALL).getHeight()),
                                SpriteEffects.None,
                                0f);

                        if(getLayer(LayerType.DEATHSOUP).bodyList[i, j] != null)
                            spriteBatch.Draw(whiteTexture,
                                getLayer(LayerType.DEATHSOUP).bodyList[i, j].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y),
                                null,
                                Color.Red,
                                getLayer(LayerType.DEATHSOUP).bodyList[i, j].Rotation,
                                Vector2.Zero,
                                new Vector2(width / getLayer(LayerType.DEATHSOUP).getWidth(), height / getLayer(LayerType.DEATHSOUP).getHeight()),
                                SpriteEffects.None,
                                0f);
                    }
                }
            }

            foreach (Body b in getLayer(LayerType.WIRING).bodyList)
            {
                if (b != null)
                    spriteBatch.Draw(whiteTexture,
                        b.Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), 
                        null, 
                        Color.Purple, 
                        b.Rotation, 
                        Vector2.Zero, 
                        new Vector2(width / getLayer(LayerType.WIRING).getWidth(), height / getLayer(LayerType.WIRING).getHeight()), 
                        SpriteEffects.None, 
                        0f);
            }
        }
    }
}
