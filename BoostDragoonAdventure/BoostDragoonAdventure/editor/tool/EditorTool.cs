﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.editor.tool
{
    public enum EditorMode
    {
        Layer,
        Entity,
        Selector
    }

    public abstract class EditorTool
    {
        protected EditorMode mode;
        protected EditorEntity entity;

        public virtual void Update(GameTime gameTime, controls.KeyboardControls controls, Vector2 pos, EditorMap map, bool toolReady)
        {
            if (!toolReady)
                return;
        }

        public abstract void primaryAction(Vector2 pos, EditorMap map);
        public abstract void secondaryAction(Vector2 pos, EditorMap map);

        protected bool isValidCoordinate(Point coord, EditorMap map, LayerType layerType)
        {
            if (coord.X < 0 || coord.Y < 0 || coord.X >= map.layerList[layerType].GetLength(0) || coord.Y >= map.layerList[layerType].GetLength(1))
                return false;

            return true;
        }

        public EditorMode getMode()
        {
            return mode;
        }

        public abstract void Draw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f);

        
    }
}
