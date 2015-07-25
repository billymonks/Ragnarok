using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.map.path
{
    public class PathNode : IComparable
    {
        public Vector2 pos;
        public Point gridPos;
        public PathNode prev;

        public int f, g, h, gridSize;

        public Rectangle box;

        public PathNode(Point gridPos, int gridSize)
        {
            this.gridPos = gridPos;
            this.pos = new Vector2(gridPos.X * gridSize, gridPos.Y * gridSize);

            this.gridSize = gridSize;

            box = new Rectangle((int)pos.X, (int)pos.Y, gridSize, gridSize);

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

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            PathNode otherNode = obj as PathNode;
            if (otherNode != null)
                return this.f.CompareTo(otherNode.f);
            else
                throw new ArgumentException("Object is not a PathNode");
        }
    }
}
