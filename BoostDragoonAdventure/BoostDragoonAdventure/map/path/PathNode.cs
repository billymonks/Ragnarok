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
        public Point gridPos;
        public PathNode prev;

        public int f, g, h;

        public PathNode(Point gridPos, Vector2 pos)
        {
            this.gridPos = gridPos;
            this.pos = pos;

            prev = null;
        }

        public void updateWeights(Point start, Point goal, PathNode curr)
        {
            h = Math.Abs(gridPos.X - goal.X) + Math.Abs(gridPos.Y - goal.Y);

            prev = curr;
            if (prev == null)
                g = 0;
            else
                g = prev.g + 1;
            
            f = g + h;
        }
    }
}
