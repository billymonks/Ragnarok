using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.editor;
using wickedcrush.entity;
using wickedcrush.helper;
using wickedcrush.controls;

namespace wickedcrush.editor.tool
{

    public class SelectorTool : EditorTool
    {
        protected LayerType layerType;

        private EditorEntityFactory f;
        private EditorMap map;

        private bool prevHold, hold;

        private Vector2 start, end;

        private List<EditorEntity> selection = new List<EditorEntity>();

        public SelectorTool(EditorEntityFactory f, EditorMap map)
        {
            mode = EditorMode.Selector;
            this.entity = null;
            this.f = f;
            this.map = map;

            prevHold = false;
            hold = false;

            start = new Vector2(0, 0);
            end = new Vector2(0, 0);
        }

        public override void Update(GameTime gameTime, KeyboardControls controls, Vector2 pos, EditorMap map, bool toolReady)
        {
            base.Update(gameTime, controls, pos, map, toolReady);

            prevHold = hold;
            hold = controls.ActionHeld();

            if (!toolReady)
                return;

            if (!prevHold && hold)
            {

                foreach (EditorEntity e in map.entityList)
                {
                    e.selected = false;
                }

                selection.Clear();

                start.X = pos.X;
                start.Y = pos.Y;
            }

            if (!hold && prevHold)
            {
                end.X = pos.X;
                end.Y = pos.Y;

                Rectangle r = getBoundingBox();

                foreach (EditorEntity e in map.entityList)
                {
                    if (e.RectangleCollision(r))
                    {
                        e.selected = true;
                        selection.Add(e);
                    }
                }
            }

        }

        private Rectangle getBoundingBox()
        {
            Rectangle r = new Rectangle();

            r.X = (int)Math.Min(start.X, end.X);
            r.Y = (int)Math.Min(start.Y, end.Y);
            r.Width = (int)Math.Max(start.X - end.X, end.X - start.X);
            r.Height = (int)Math.Max(start.Y - end.Y, end.Y - start.Y);

            return r;
        }

        public override void primaryAction(Vector2 pos, EditorMap map)
        {

        }

        public override void secondaryAction(Vector2 pos, EditorMap map)
        {
            
        }
    }
}
