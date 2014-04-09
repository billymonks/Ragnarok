using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.map.path
{
    public class PathNode
    {
        public Vector2 pos;

        public PathNode(Vector2 pos)
        {
            this.pos = pos;
        }
    }
}
