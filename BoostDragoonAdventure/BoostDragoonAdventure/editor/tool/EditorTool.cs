using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace wickedcrush.editor.tool
{
    public abstract class EditorTool
    {
        protected EditorMode mode;
        protected LayerType layerType;
        protected EditorEntity entity;

        public abstract void primaryAction(Vector2 pos, EditorMap map);
        public abstract void secondaryAction(Vector2 pos, EditorMap map);
    }
}
