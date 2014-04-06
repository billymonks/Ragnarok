using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.entity.physics_entity;

namespace wickedcrush.map.layer
{
    public class Layer
    {
        public bool[,] data;
        public List<Body> bodyList;

        public Layer(int width, int height) // Empty Layer
        {
            data = new bool[width, height]; //x, y
            bodyList = new List<Body>();
        }

        public Layer(bool[,] data, World w, int width, int height, bool solid)
        {
            this.data = data;
            bodyList = new List<Body>();
            generateBodies(w, width, height, solid);
        }

        public void setCoordinate(int x, int y, bool b)
        {
            data[x, y] = b;
        }

        public bool getCoordinate(int x, int y)
        {
            if (x < 0 || y < 0 || x >= getWidth() || y >= getHeight())
                return false;

            return data[x, y];
        }

        public int getWidth()
        {
            return (data.GetLength(0));
        }

        public int getHeight()
        {
            return (data.GetLength(1));
        }

        public bool collision(Rectangle r)
        {
            for (int i = r.X; i <= r.Width; i++)
            {
                for (int j = r.Y; j <= r.Height; j++)
                {
                    if (getCoordinate(i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Rectangle getRectangle(int width, int height, int x, int y)
        {
            int gridWidth = width/getWidth();
            int gridHeight = height/getHeight();

            return new Rectangle(
                x * gridWidth, 
                y * gridHeight, 
                gridWidth, 
                gridHeight);
        }

        private Body createBodyFromLayerCoordinate(World w, int width, int height, int x, int y, bool solid) //width/height = map size in pixels
        {
            
            int gridWidth = width / getWidth();
            int gridHeight = height / getHeight();

            Body body = new Body(w, new Vector2(x * gridWidth, y * gridHeight));
            FixtureFactory.AttachRectangle(gridWidth, gridHeight, 1f, new Vector2(gridWidth * 0.5f, gridHeight * 0.5f), body);

            body.CollisionGroup = (short)CollisionGroup.LAYER;
            body.BodyType = BodyType.Static;

            if (!solid)
                body.IsSensor = true;

            return body;
        }

        public void generateBodies(World w, int width, int height, bool solid)
        {

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    if (getCoordinate(i, j))
                        bodyList.Add(createBodyFromLayerCoordinate(w, width, height, i, j, solid));
        }

        public void removeBodies(World w)
        {

        }
    }
}
