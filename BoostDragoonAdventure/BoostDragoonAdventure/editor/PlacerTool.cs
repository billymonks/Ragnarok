using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.editor
{
    public enum EditorMode
    {
        Layer,
        Entity
    }

    public class PlacerTool
    {
        EditorMode mode;
        LayerType layerType;
        EditorEntity entity;

        public PlacerTool()
        {
            mode = EditorMode.Layer;
            layerType = LayerType.WALL;
            entity = null;
        }
    }
}
