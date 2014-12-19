using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.display._3d.texture
{
    public class Tileset
    {
        String name, path;
        Point size;
        Point[] tilePos; // length = 3, represent X and Y pos from top left to bottom right
        Point[] tileSize; // length = 3, represent width and height from top left to bottom right

        public Tileset(String path)
        {
            this.path = path;
        }

        public void LoadContent()
        {

        }

        private void LoadXml()
        {

        }
    }
}
