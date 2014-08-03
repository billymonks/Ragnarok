using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.editor;
using wickedcrush.entity;
using wickedcrush.helper;
using wickedcrush.controls;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display.primitives;
using wickedcrush.manager.editor.entity;

namespace wickedcrush.editor.tool
{

    public class SelectorTool : EditorTool
    {
        protected LayerType layerType;

        private EditorEntityFactory f;
        private EditorEntityManager manager;

        private bool prevHold, hold;

        private Vector2 start, end;

        private List<EditorEntity> selection = new List<EditorEntity>();

        private Color outlineColor = new Color(255, 255, 255, 255), fillColor = new Color(100, 100, 100, 100);

        public SelectorTool(EditorEntityFactory f, EditorEntityManager manager)
        {
            mode = EditorMode.Selector;
            this.entity = null;
            this.f = f;
            this.manager = manager;

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

            if (!prevHold && hold)
            {

                manager.DeselectAll();

                selection.Clear();

                start.X = pos.X;
                start.Y = pos.Y;
            }

            if (!hold && prevHold)
            {
                manager.addSelection(selection, getBoundingBox());
            }

            if (hold)
            {
                end.X = pos.X;
                end.Y = pos.Y;
            }
            else
            {
                start.X = pos.X;
                start.Y = pos.Y;
            }

            if (!toolReady)
                return;

            if (controls.DeletePressed())
            {
                RemoveSelection();
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

        private void RemoveSelection()
        {
            foreach (EditorEntity e in selection)
            {
                e.Remove();
            }

            selection.Clear();
        }

        public override void Draw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f)
        {
            if (!PrimitiveDrawer.isInitialized())
            {
                PrimitiveDrawer.LoadContent(gd);
            }

            if (hold)
            {
                spriteBatch.Draw(wTex, getBoundingBox(), fillColor);
                PrimitiveDrawer.DrawRectangle(spriteBatch, getBoundingBox(), outlineColor, 2);
            }
        }
    }
}
