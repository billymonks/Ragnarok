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

        public Tileset tileset;

        public List<WCVertex> solidGeomVertices;

        public int startVertex, vertCount;

        protected int height;

        //public DynamicVertexBuffer buffer;

        public TileLayer(GameBase game, bool[,] data, int height, String tilesetPath)
        {
            this.data = data;

            this.height = height;
            this.tileset = new Tileset(tilesetPath);

            colorTexture = game.Content.Load<Texture2D>(@tileset.tex);
            normalTexture = game.Content.Load<Texture2D>(@tileset.normal);

            solidGeomVertices = new List<WCVertex>();
        }

        protected void BuildScene(GameBase game)
        {


            AddGeometry();

        }

        protected abstract void AddGeometry();

    }
}
