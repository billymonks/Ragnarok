using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.map.layer;

namespace wickedcrush.map.path
{
    public class Navigator
    {
        private Map map;
        private PathNode[,] pathNodeGrid;
        
        public Navigator(Map m)
        {
            map = m;
            loadPathNodeGrid();
        }

        private void loadPathNodeGrid()
        {
            Layer l = map.getLayer(LayerType.WALL);
            Layer ds = map.getLayer(LayerType.DEATH_SOUP);

            pathNodeGrid = new PathNode[l.getWidth() * 2, l.getHeight() * 2];

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
        }

        public Stack<PathNode> getPath(Point start, Point goal) // Crappy A*
        {
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

                //sort openList

                openList.Remove(curr);
                closedList.Add(curr);
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

        
    }
}
