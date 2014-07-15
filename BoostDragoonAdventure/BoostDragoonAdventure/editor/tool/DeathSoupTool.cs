using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.editor.tool
{
    

    public class DeathSoupTool : EditorTool
    {
        public DeathSoupTool()
        {
            mode = EditorMode.Layer;
            layerType = LayerType.DEATHSOUP;
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

        
    }
}
