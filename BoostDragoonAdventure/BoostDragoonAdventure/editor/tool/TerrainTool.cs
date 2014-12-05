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

        public override void Update(GameTime gameTime, controls.Controls controls, Vector2 pos, EditorRoom map, bool toolReady)
        {
            base.Update(gameTime, controls, pos, map, toolReady);

            if (!toolReady)
                return;

            if (controls.ActionHeld() || controls.InteractHeld())
                primaryAction(pos, map);

            if (controls.StrafeHeld() || controls is controls.GamepadControls && controls.ItemAHeld())
                secondaryAction(pos, map);

            
        }

        public override void primaryAction(Vector2 pos, EditorRoom map)
        {
            PlaceLayer(pos, map);
        }

        public override void secondaryAction(Vector2 pos, EditorRoom map)
        {
            EraseLayer(pos, map);
        }

        protected void PlaceLayer(Vector2 pos, EditorRoom map)
        {
            Point coordinate = Helper.convertPositionToCoordinate(pos, map, layerType);

            if (isValidCoordinate(coordinate, map, layerType))
            {
                map.layerList[layerType][coordinate.X, coordinate.Y] = 1;

                // if layer lock not true
                // {
                if (layerType == LayerType.WALL)
                {
                    map.layerList[LayerType.DEATHSOUP][coordinate.X, coordinate.Y] = 0;
                }
                if (layerType == LayerType.DEATHSOUP)
                {
                    map.layerList[LayerType.WALL][coordinate.X, coordinate.Y] = 0;
                }
            }
            // }

        }

        protected void EraseLayer(Vector2 pos, EditorRoom map)
        {
            Point coordinate;

            // if layer lock = true
            // {
            /* coordinate = Helper.convertPositionToCoordinate(pos, map, layerType);

            if (isValidCoordinate(coordinate, map, layerType))
                map.layerList[layerType][coordinate.X, coordinate.Y] = 0;*/
            // }

            //else
            foreach(KeyValuePair<LayerType, int[,]> pair in map.layerList)
            {
                coordinate = Helper.convertPositionToCoordinate(pos, map, pair.Key);

                if (isValidCoordinate(coordinate, map, pair.Key))
                    pair.Value[coordinate.X, coordinate.Y] = 0;
            }


        }

        public override void Draw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            //throw new NotImplementedException();
        }
        
    }
}
