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

    public class PlacerTool : EditorTool
    {
        public PlacerTool()
        {
            mode = EditorMode.Layer;
            layerType = LayerType.WALL;
            entity = null;
        }

        public override void primaryAction(Vector2 pos, EditorMap map)
        {
            Place(pos, map);
        }

        public override void secondaryAction(Vector2 pos, EditorMap map)
        {
            Erase(pos, map);
        }

        private void Place(Vector2 pos, EditorMap map)
        {
            Point coordinate = convertPositionToCoordinate(pos, map);

            if (isValidCoordinate(coordinate, map))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 1;
        }

        private void Erase(Vector2 pos, EditorMap map)
        {
            Point coordinate = convertPositionToCoordinate(pos, map);

            if(isValidCoordinate(coordinate, map))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 0;
        }

        private Point convertPositionToCoordinate(Vector2 pos, EditorMap map)
        {
            int gridSize = map.width / map.layerList[layerType].GetLength(0);

            return new Point((int) pos.X / gridSize, (int) pos.Y / gridSize);
        }

        private bool isValidCoordinate(Point coord, EditorMap map)
        {
            if (coord.X < 0 || coord.Y < 0 || coord.X >= map.layerList[layerType].GetLength(0) || coord.Y >= map.layerList[layerType].GetLength(1))
                return false;

            return true;
        }
    }
}
