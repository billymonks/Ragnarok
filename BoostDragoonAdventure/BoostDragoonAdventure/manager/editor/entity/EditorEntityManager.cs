using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.entity.physics_entity;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.entity.physics_entity.agent.enemy;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.map.layer;
using wickedcrush.editor;

namespace wickedcrush.manager.editor.entity
{
    public class EditorEntityManager
    {

        public List<EditorEntity> entityList = new List<EditorEntity>();
        private List<EditorEntity> addList = new List<EditorEntity>();
        private List<EditorEntity> removeList = new List<EditorEntity>();

        public EditorEntityManager()
        {
            
        }

        public void Update(GameTime gameTime)
        {
            updateEntities(gameTime);
        }

        private void updateEntities(GameTime gameTime)
        {
            foreach (EditorEntity e in entityList)
            {
                e.Update(gameTime);

                if (e.readyForRemoval())
                    removeList.Add(e);
            }

            performRemoval();
            performAdd();
        }

        private void performAdd()
        {
            if (addList.Count > 0)
            {
                foreach (EditorEntity e in addList)
                {
                    entityList.Add(e);
                }

                addList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (EditorEntity e in removeList)
                {
                    entityList.Remove(e);
                    
                }

                removeList.Clear();
            }
        }

        public void addEntity(EditorEntity e)
        {
            addList.Add(e);
        }

        public void DebugDraw(GraphicsDevice gd, SpriteBatch sb, Texture2D wTex, Texture2D aTex, SpriteFont testFont)
        {
            foreach (EditorEntity e in entityList)
            {
                e.DebugDraw(wTex, aTex, gd, sb, testFont, Color.Green);
            }
        }

        protected void Dispose(bool disposing)
        {
            foreach (EditorEntity e in entityList)
            {
                e.Remove();
            }

            entityList.Clear();
            addList.Clear();
            removeList.Clear();

            entityList = null;
            addList = null;
            removeList = null;
        }

        public bool CanPlace(EditorEntity e)
        {
            foreach (EditorEntity entity in entityList)
            {
                if (entity.Collision(e))
                    return false;
            }
            return true;
        }

        public void DeselectAll()
        {
            foreach (EditorEntity e in entityList)
            {
                e.selected = false;
            }
        }

        public void addSelection(List<EditorEntity> selection, Rectangle r)
        {
            foreach (EditorEntity e in entityList)
            {
                if (e.RectangleCollision(r))
                {
                    e.selected = true;
                    selection.Add(e);
                }
            }
        }
    }
}
