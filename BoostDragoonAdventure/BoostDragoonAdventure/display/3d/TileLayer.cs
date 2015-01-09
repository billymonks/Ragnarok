using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d.texture;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d.vertex;

namespace wickedcrush.display._3d
{
    public abstract class TileLayer
    {
        protected int ART_GRID_SIZE = 10;

        protected bool[,] data;

        public Texture2D colorTexture;
        public Texture2D normalTexture;

        protected Tileset tileset;

        public List<WCVertex>[,] gridVertices;
        public List<WCVertex> solidGeomVertices;

        protected int height;

        public DynamicVertexBuffer buffer;

        public TileLayer(Game game, bool[,] data, int height, String tilesetPath)
        {
            this.data = data;

            this.height = height;
            this.tileset = new Tileset(tilesetPath);

            colorTexture = game.Content.Load<Texture2D>(@tileset.tex);
            normalTexture = game.Content.Load<Texture2D>(@tileset.normal);

            solidGeomVertices = new List<WCVertex>();
        }

        protected void PrepareVertices()
        {
            int startX, startY, endX, endY;
            solidGeomVertices.Clear();

            startX = 0;
            startY = 0;
            endX = gridVertices.GetLength(0);
            endY = gridVertices.GetLength(1);

            //startX = (int)cameraPosition.X / 10 - (int)(sceneDimensions.X / 20) - 1;
            //startY = (int)cameraPosition.Z / 10 - (int)(sceneDimensions.Y / 20) - 20;
            //endX = startX + (int)(sceneDimensions.X / 10) + 2; // gridVertices.GetLength(0);
            //endY = startY + (int)(sceneDimensions.Y / 10) + 27; // gridVertices.GetLength(1);

            if (startX < 0)
                startX = 0;
            if (startY < 0)
                startY = 0;
            if (endX >= gridVertices.GetLength(0))
            {
                endX = gridVertices.GetLength(0) - 1;
            }
            if (endY >= gridVertices.GetLength(1))
            {
                endY = gridVertices.GetLength(1) - 1;
            }

            if (endX > gridVertices.GetLength(0))
                endX = gridVertices.GetLength(0);
            if (endY > gridVertices.GetLength(1))
                endY = gridVertices.GetLength(1);

            for (int i = startX; i < endX; i++)
                for (int j = endY; j >= startY; j--)
                    foreach (WCVertex v in gridVertices[i, j])
                    {
                        solidGeomVertices.Add(v);
                    }

        }

        protected void BuildScene(Game game)
        {
            gridVertices = new List<WCVertex>[data.GetLength(0), data.GetLength(1)];

            for (int i = 0; i < gridVertices.GetLength(0); i++)
            {
                for (int j = 0; j < gridVertices.GetLength(1); j++)
                {
                    gridVertices[i, j] = new List<WCVertex>();
                }
            }

            AddGeometry();
            PrepareVertices();
            buffer = new DynamicVertexBuffer(game.GraphicsDevice, typeof(WCVertex), solidGeomVertices.Count, BufferUsage.WriteOnly);
            buffer.SetData(solidGeomVertices.ToArray());
        }

        protected abstract void AddGeometry();

    }
}
