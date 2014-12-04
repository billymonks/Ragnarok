using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.screen;
using Microsoft.Xna.Framework;
using wickedcrush.entity;

namespace wickedcrush.editor.tool
{
    public class Toolbox
    {
        public Dictionary<String, EditorTool> tools;
        public EditorScreen _parent;

        public Toolbox(EditorScreen parent)
        {
            _parent = parent;

            tools = new Dictionary<String, EditorTool>();

            tools.Add("wall", new TerrainTool(LayerType.WALL));
            tools.Add("deathsoup", new TerrainTool(LayerType.DEATHSOUP));
            tools.Add("wiring", new TerrainTool(LayerType.WIRING));
            tools.Add("selector", new SelectorTool(_parent.factory, _parent.room.manager));
            tools.Add("chest", new EntityTool(_parent.factory.LoadEntity("CHEST", Vector2.Zero, Direction.East), _parent.factory));
            tools.Add("turret", new EntityTool(_parent.factory.LoadEntity("TURRET", Vector2.Zero, Direction.East), _parent.factory));
            tools.Add("pot", new EntityTool(_parent.factory.LoadEntity("POT", Vector2.Zero, Direction.East), _parent.factory));
            tools.Add("floorswitch", new EntityTool(_parent.factory.LoadEntity("FLOOR_SWITCH", Vector2.Zero, Direction.East), _parent.factory));
            tools.Add("timer", new EntityTool(_parent.factory.LoadEntity("TIMER", Vector2.Zero, Direction.East), _parent.factory));

        }
    }
}
