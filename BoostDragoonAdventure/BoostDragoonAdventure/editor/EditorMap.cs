using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.layer;

namespace wickedcrush.editor
{
    public class EditorMap
    {
        public String name;
        public int width, height;
        public Dictionary<LayerType, int[,]> layerList;
        public List<EditorEntity> entityList;

        public EditorMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            name = "Untitled";

            rehydrateLayerList();
        }

        private void rehydrateLayerList()
        {
            layerList = new Dictionary<LayerType, int[,]>();

            layerList.Add(LayerType.WALL, getEmptyLayer(20));
            layerList.Add(LayerType.DEATH_SOUP, getEmptyLayer(20));
            layerList.Add(LayerType.WIRING, getEmptyLayer(10));
            layerList.Add(LayerType.OBJECTS, getEmptyLayer(10));
        }

        private int[,] getEmptyLayer(int gridSize)
        {
            int[,] tempLayer = new int[width / gridSize, height / gridSize];

            for (int i = 0; i < tempLayer.GetLength(0); i++)
                for (int j = 0; j < tempLayer.GetLength(1); j++)
                    tempLayer[i, j] = 0;

            return tempLayer;
        }
    }
}
