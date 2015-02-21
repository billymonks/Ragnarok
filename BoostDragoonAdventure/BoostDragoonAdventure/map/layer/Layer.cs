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
        public Body[,] bodyList;
        public Body layerBody;
        public LayerType layerType;

        public int width, height;

        public Layer(int width, int height) // Empty Layer
        {
            this.width = width;
            this.height = height;
            data = new bool[width, height]; //x, y
            bodyList = new Body[getWidth(), getHeight()];
        }

        public Layer(bool[,] data, World w, int width, int height, bool solid, LayerType layerType)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            bodyList = new Body[getWidth(), getHeight()];
            this.layerType = layerType;
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

        public int getGridSize()
        {
            return width / getWidth();
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

        public void generateLayerBody(World w, int width, int height, bool solid)
        {
            int gridWidth = width / getWidth();
            int gridHeight = height / getHeight();

            Body body = new Body(w, Vector2.Zero);

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    if (getCoordinate(i, j))
                        FixtureFactory.AttachRectangle(gridWidth, gridHeight, 1f, new Vector2(gridWidth * 0.5f + i * gridWidth, gridHeight * 0.5f + j * gridHeight), body);

            body.UserData = layerType;

            if (!solid)
                body.IsSensor = true;

            layerBody = body;
        }

        private Body createBodyFromLayerCoordinate(World w, int width, int height, int x, int y, bool solid) //width/height = map size in pixels
        {
            
            int gridWidth = width / getWidth();
            int gridHeight = height / getHeight();

            Body body = new Body(w, new Vector2(x * gridWidth, y * gridHeight));
            FixtureFactory.AttachRectangle(gridWidth, gridHeight, 1f, new Vector2(gridWidth * 0.5f, gridHeight * 0.5f), body);

            body.FixedRotation = true;
            body.LinearVelocity = Vector2.Zero;

            body.CollisionGroup = (short)CollisionGroup.LAYER;

            if (layerType == LayerType.WALL)
            {
                body.BodyType = BodyType.Static;
            }
            else
            {
                body.BodyType = BodyType.Dynamic;
                body.IsSensor = true;
            }
            

            body.UserData = layerType;

            if (!solid)
                body.IsSensor = true;

            return body;
        }

        public void generateBodies(World w, int width, int height, bool solid)
        {

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    if (getCoordinate(i, j))
                        bodyList[i, j] = createBodyFromLayerCoordinate(w, width, height, i, j, solid);
        }

        public void removeBodies(World w)
        {

        }
    }
}
