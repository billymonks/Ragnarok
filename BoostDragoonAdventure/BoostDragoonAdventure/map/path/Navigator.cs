using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.map.layer;
using wickedcrush.helper;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.map.path
{
    public class Navigator
    {
        private Map map;
        private PathNode[,] pathNodeGrid;
        private int agentSize;

        private int closedListLimit = 100; // keep the game from freezing when can't find a path

        public Navigator(Map m, int agentSize)
        {
            map = m;
            this.agentSize = agentSize;
            loadPathNodeGrid(agentSize);
        }

        private void loadPathNodeGrid(int agentSize)
        {
            Layer l = map.getLayer(LayerType.WALL);
            Layer ds = map.getLayer(LayerType.DEATHSOUP);
            int size;

            pathNodeGrid = new PathNode[l.getWidth() * 2, l.getHeight() * 2];

            size = Helper.roundUpDivision(agentSize, (map.width / pathNodeGrid.GetLength(0)));

            for (int i = 0; i < l.getWidth(); i++)
            {
                for (int j = 0; j < l.getHeight(); j++)
                {
                    if (!l.getCoordinate(i, j) && !ds.getCoordinate(i, j))
                    {
                        pathNodeGrid[i * 2, j * 2] = getNodeFromCoordinate(i * 2, j * 2);
                        pathNodeGrid[i * 2 + 1, j * 2] = getNodeFromCoordinate(i * 2 + 1, j * 2);
                        pathNodeGrid[i * 2, j * 2 + 1] = getNodeFromCoordinate(i * 2, j * 2 + 1);
                        pathNodeGrid[i * 2 + 1, j * 2 + 1] = getNodeFromCoordinate(i * 2 + 1, j * 2 + 1);
                    }
                }
            }

            pruneNodes(size);
        }

        private void pruneNodes(int size)
        {
            for (int i = 0; i < pathNodeGrid.GetLength(0); i++)
            {
                for (int j = 0; j < pathNodeGrid.GetLength(1); j++)
                {
                    if (!nodeFits(i, j, size))
                        pathNodeGrid[i, j] = null;
                }
            }
        }

        private bool nodeFits(int x, int y, int size)
        {
            //bool fits = true;
            for (int i = x; i < x + size; i++)
            {
                for (int j = y; j < y + size; j++)
                {
                    if (i < 0 || j < 0 || i >= pathNodeGrid.GetLength(0) || j >= pathNodeGrid.GetLength(1) || pathNodeGrid[i, j] == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Stack<PathNode> getPath(Point start, Point goal) // Crappy A*
        {
            //loadPathNodeGrid(agentSize);

            List<PathNode> openList = new List<PathNode>();
            List<PathNode> closedList = new List<PathNode>();
            Stack<PathNode> path = new Stack<PathNode>();
            PathNode curr = null; 
            
            addNodeToOpenList(start.X, start.Y, start, goal, curr, openList, closedList);

            while (openList.Count > 0)
            {
                // curr = lowest f value
                curr = openList[0];

                if (curr.gridPos == goal)
                {
                    //trace back path
                    while (curr.prev != null)
                    {
                        path.Push(curr);
                        curr = curr.prev;
                    }
                    return path;
                }

                addNodeToOpenList(curr.gridPos.X - 1, curr.gridPos.Y, start, goal, curr, openList, closedList);
                addNodeToOpenList(curr.gridPos.X + 1, curr.gridPos.Y, start, goal, curr, openList, closedList);
                addNodeToOpenList(curr.gridPos.X, curr.gridPos.Y - 1, start, goal, curr, openList, closedList);
                addNodeToOpenList(curr.gridPos.X, curr.gridPos.Y + 1, start, goal, curr, openList, closedList);


                openList.Remove(curr);
                closedList.Add(curr);
                
                //sort openList
                openList.Sort(); //just use quicksort

                if (closedList.Count > closedListLimit)
                    return null;
            }

            return null;
        }

        private void addNodeToOpenList(int x, int y, Point start, Point goal, PathNode curr, List<PathNode> openList, List<PathNode> closedList)
        {
            if (x < 0 || y < 0 || x >= pathNodeGrid.GetLength(0) || y >= pathNodeGrid.GetLength(1) || pathNodeGrid[x, y] == null)
                return;

            if (!openList.Contains(pathNodeGrid[x, y])
                && !closedList.Contains(pathNodeGrid[x, y]))
            {
                pathNodeGrid[x, y].updateWeights(start, goal, curr);
                openList.Add(pathNodeGrid[x, y]);
            }
        }

        private PathNode getNodeFromCoordinate(int x, int y)
        {
            return new PathNode(
                new Point(x, y),
                new Vector2(
                    (map.width / pathNodeGrid.GetLength(0)) * x,
                    (map.height / pathNodeGrid.GetLength(1)) * y),
                    map.width / pathNodeGrid.GetLength(0));
        }

        public virtual void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            foreach (PathNode n in pathNodeGrid)
                if(n!=null)
            {
                spriteBatch.Draw(tex, new Rectangle((int)n.box.X, (int)n.box.Y, n.box.Width, n.box.Height), Color.GreenYellow);
                //spriteBatch.DrawString(f, name, pos, Color.Black);
            }
        }
    }
}
