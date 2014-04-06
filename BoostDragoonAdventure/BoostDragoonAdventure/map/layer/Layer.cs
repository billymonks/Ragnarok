using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;

namespace wickedcrush.map.layer
{
    public class Layer
    {
        public bool[,] data;

        public Layer(int width, int height) // Empty Layer
        {
            data = new bool[width, height]; //x, y
        }

        public Layer(bool[,] data)
        {
            this.data = data;
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
    }
}
