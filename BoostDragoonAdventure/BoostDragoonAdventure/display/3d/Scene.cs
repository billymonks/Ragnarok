using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d.vertex;
using wickedcrush.map;

namespace wickedcrush.display._3d
{
    public class Scene
    {
        const int ART_GRID_SIZE = 10;
        public List<WCVertex>[,] gridVertices;
        public List<WCVertex> solidGeomVertices;

        public Scene()
        {
            solidGeomVertices = new List<WCVertex>();
        }

        public void BuildScene(Map map)
        {
            gridVertices = new List<WCVertex>[map.width / ART_GRID_SIZE, map.height / ART_GRID_SIZE];
            for(int i = 0; i < gridVertices.GetLength(0); i++)
            {
                for (int j = 0; j < gridVertices.GetLength(1); j++)
                {
                    gridVertices[i, j] = new List<WCVertex>();
                }
            }

            solidGeomVertices.Clear();
        
        }
    }
}
