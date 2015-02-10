using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.editor;
using wickedcrush.entity;
using wickedcrush.helper;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.editor.tool
{

    public class EntityTool : EditorTool
    {
        protected LayerType layerType;

        private EditorEntityFactory f;

        public EntityTool(EditorEntity entity, EditorEntityFactory f)
        {
            mode = EditorMode.Entity;
            this.entity = entity;
            this.f = f;
        }

        public override void Update(GameTime gameTime, controls.Controls controls, Vector2 pos, EditorRoom map, bool toolReady)
        {
            base.Update(gameTime, controls, pos, map, toolReady);

            if (!toolReady)
                return;

            


            
            if (controls.InteractPressed())
                primaryAction(pos, map);
            if (controls.StrafePressed())
                f.preview.rotateCCW();
            if (controls.BoostPressed())
                f.preview.rotateCW();
            
        }

        public override void primaryAction(Vector2 pos, EditorRoom map)
        {
            PlaceEntity(pos, map);
        }

        public override void secondaryAction(Vector2 pos, EditorRoom map) //rotate (rotato)
        {
            f.preview.rotateCW();
        }

        protected void PlaceEntity(Vector2 pos, EditorRoom map)
        {
            bool ready = true;
            Point coordinate;

            foreach (KeyValuePair<LayerType, int[,]> pair in map.layerList)
            {
                coordinate = Helper.convertPositionToCoordinate(pos, map, pair.Key);
                if (!isValidCoordinate(coordinate, map, pair.Key))
                {
                    ready = false;
                    break;
                }
                if (map.layerList[pair.Key][coordinate.X, coordinate.Y] == 1)
                {
                    ready = false;
                    break;
                }
            }

            if(ready)
                f.AddEntity(entity.code, pos, f.preview.angle);

        }

        protected void EraseEntity(Vector2 pos, EditorRoom map)
        {
            //Point coordinate = convertPositionToCoordinate(pos, map, layerType);

            //if (isValidCoordinate(coordinate, map, layerType))
                //map.layerList[layerType][coordinate.X, coordinate.Y] = 0;
        }

        public EditorEntity getEntity(Vector2 pos, Direction d)
        {
            return f.getEntity(entity.code, pos, d);
        }

        public override void Draw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            //throw new NotImplementedException();
        }
        
    }
}
