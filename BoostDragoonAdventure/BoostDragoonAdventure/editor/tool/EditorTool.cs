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
        protected LayerType layerType;
        protected EditorEntity entity;

        public abstract void primaryAction(Vector2 pos, EditorMap map);
        public abstract void secondaryAction(Vector2 pos, EditorMap map);

        protected void PlaceLayer(Vector2 pos, EditorMap map)
        {
            Point coordinate = convertPositionToCoordinate(pos, map);

            if (isValidCoordinate(coordinate, map))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 1;
        }

        protected void EraseLayer(Vector2 pos, EditorMap map)
        {
            Point coordinate = convertPositionToCoordinate(pos, map);

            if (isValidCoordinate(coordinate, map))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 0;
        }

        protected Point convertPositionToCoordinate(Vector2 pos, EditorMap map)
        {
            int gridSize = map.width / map.layerList[layerType].GetLength(0);

            return new Point((int)pos.X / gridSize, (int)pos.Y / gridSize);
        }

        protected bool isValidCoordinate(Point coord, EditorMap map)
        {
            if (coord.X < 0 || coord.Y < 0 || coord.X >= map.layerList[layerType].GetLength(0) || coord.Y >= map.layerList[layerType].GetLength(1))
                return false;

            return true;
        }
    }
}
