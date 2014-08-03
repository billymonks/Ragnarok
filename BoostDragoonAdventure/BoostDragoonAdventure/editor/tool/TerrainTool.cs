using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.helper;
using Microsoft.Xna.Framework.Graphics;

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

        public override void Update(GameTime gameTime, controls.KeyboardControls controls, Vector2 pos, EditorMap map, bool toolReady)
        {
            base.Update(gameTime, controls, pos, map, toolReady);

            if (!toolReady)
                return;

            if (controls.ActionHeld())
                primaryAction(pos, map);

            if (controls.StrafeHeld())
                secondaryAction(pos, map);
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

        public override void Draw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            //throw new NotImplementedException();
        }
        
    }
}
