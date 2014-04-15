using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.map;
using FarseerPhysics.Dynamics;

namespace wickedcrush.entity
{
    public enum Direction
    {
        South = 0,
        SouthWest = 45,
        West = 90,
        NorthWest = 135,
        North = 180,
        NorthEast = 225,
        East = 270,
        SouthEast = 315
    }

    public class Entity
    {
        #region Variables
        public Vector2 pos, size, center;
        protected String name;

        public Entity parent;
        public List<Entity> subEntityList;
        public List<Entity> removeList;

        private bool remove = false;
        public bool dead = false;
        public bool immortal = false;

        public Nullable<Direction> facing = null;
        #endregion

        #region Initialization
        public Entity(Vector2 pos, Vector2 size, Vector2 center)
        {
            Initialize(pos, size, center);
        }
        
        protected void Initialize(Vector2 pos, Vector2 size, Vector2 center)
        {
            this.pos = pos;
            this.size = size;
            this.center = center;

            this.name = "Entity";

            subEntityList = new List<Entity>();
            removeList = new List<Entity>();
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            foreach (Entity e in subEntityList)
            {
                e.Update(gameTime);
                
                if (e.remove)
                    removeList.Add(e);
            }

            foreach (Entity e in removeList)
                subEntityList.Remove(e);

            removeList.Clear();
        }
        #endregion

        #region CollisionChecks
        
        #endregion

        protected virtual void Remove()
        {
            foreach (Entity e in subEntityList)
            {
                e.Remove();
            }
            remove = true;
        }

        public bool readyForRemoval()
        {
            return remove;
        }

        public virtual void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //spriteBatch.Draw(whiteTexture, body, c);
            spriteBatch.Draw(tex, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, 2, 2), c); 
            spriteBatch.DrawString(f, name, pos, Color.Black);

            foreach (Entity e in subEntityList)
                e.DebugDraw(tex, gd, spriteBatch, f, c);
        }

        
    }
        
}
