using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.helper;

namespace wickedcrush.editor.tool
{

    public class TerrainTool : EditorTool
    {
        protected LayerType layerType;

        public TerrainTool(LayerType type)
        {
            mode = EditorMode.Layer;
            layerType = type;
            entity = null;
        }

        public override void primaryAction(Vector2 pos, EditorMap map)
        {
            PlaceLayer(pos, map);
        }

        public override void secondaryAction(Vector2 pos, EditorMap map)
        {
            EraseLayer(pos, map);
        }

        protected void PlaceLayer(Vector2 pos, EditorMap map)
        {
            Point coordinate = Helper.convertPositionToCoordinate(pos, map, layerType);

            if (isValidCoordinate(coordinate, map, layerType))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 1;
        }

        protected void EraseLayer(Vector2 pos, EditorMap map)
        {
            Point coordinate = Helper.convertPositionToCoordinate(pos, map, layerType);

            if (isValidCoordinate(coordinate, map, layerType))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 0;
        }
        
    }
}
