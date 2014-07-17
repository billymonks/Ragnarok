using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.editor.tool
{
    public enum EditorMode
    {
        Layer,
        Entity
    }

    public abstract class EditorTool
    {
        protected EditorMode mode;
        protected EditorEntity entity;

        public abstract void primaryAction(Vector2 pos, EditorMap map);
        public abstract void secondaryAction(Vector2 pos, EditorMap map);

        

        protected Point convertPositionToCoordinate(Vector2 pos, EditorMap map, LayerType layerType)
        {
            int gridSize = map.width / map.layerList[layerType].GetLength(0);

            return new Point((int)pos.X / gridSize, (int)pos.Y / gridSize);
        }

        protected bool isValidCoordinate(Point coord, EditorMap map, LayerType layerType)
        {
            if (coord.X < 0 || coord.Y < 0 || coord.X >= map.layerList[layerType].GetLength(0) || coord.Y >= map.layerList[layerType].GetLength(1))
                return false;

            return true;
        }
    }
}
