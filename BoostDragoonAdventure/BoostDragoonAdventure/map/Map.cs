using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.layer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.map
{
    public class Map
    {

        public int width, height;
        public Dictionary<LayerType, Layer> layerList;
        public String name;

        public Map(int width, int height, String name)
        {
            this.width = width;
            this.height = height;
            this.name = name;

            layerList = new Dictionary<LayerType, Layer>();
        }

        public void addLayer(Boolean[,] data, LayerType layerType)
        {
            layerList.Add(layerType, new Layer(data));
        }

        public Layer getLayer(LayerType layerType)
        {
            return layerList[layerType];
        }

        public Boolean predictedLayerCollision(Rectangle r, LayerType layerType, Vector2 v)
        {
            Rectangle testRect = r;

            testRect.X += (int)roundTowardZero(v.X);
            testRect.Y += (int)roundTowardZero(v.Y);

            return layerCollision(testRect, layerType);
        }

        protected float roundTowardZero(float f)
        {
            if (f < 0)
            {
                return (float)Math.Ceiling(f);
            }
            else
            {
                return (float)Math.Floor(f);
            }
        }

        public Boolean layerCollision(Rectangle r, LayerType layerType)
        {
            Layer layer = getLayer(layerType);
            return layer.collision(
                new Rectangle( //grid coordinates, rectangle represents the coordinates that r is in, width is not really width, they are the right/bottom bounds
                    r.X / (width / layer.getWidth()), 
                    (r.Y / (height /layer.getHeight())), 
                    (r.X + r.Width) / (width / layer.getWidth()),
                    (r.Y + r.Height)/( height / layer.getHeight()))
                );
        }

        private int roundUpDivision(int a, int b) // a/b
        {
            return (a / b + (a % b > 0 ? 1 : 0));
        }

        public void drawMap(GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            Texture2D whiteTexture = new Texture2D(gd, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            whiteTexture.SetData(data);

            Color color = new Color(0, 0, 0);

            foreach(var pair in layerList)
            {
                color.R += 50;
                color.G += 50;
                color.B += 50;

                for (int i = 0; i < pair.Value.getWidth(); i++)
                {
                    for (int j = 0; j < pair.Value.getHeight(); j++)
                    {
                        if (pair.Value.getCoordinate(i, j))
                        {
                            spriteBatch.Draw(whiteTexture, pair.Value.getRectangle(width, height, i, j), color);
                        }
                    }
                }
            }
        }
    }
}
