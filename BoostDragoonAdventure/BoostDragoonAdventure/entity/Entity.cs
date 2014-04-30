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
        East = 0,
        SouthEast = 45,
        South = 90,
        SouthWest = 135,
        West = 180,
        NorthWest = 225,
        North = 270,
        NorthEast = 315
    }

    public class Entity
    {
        #region Variables
        public Vector2 pos, size, center;
        protected String name;

        public Entity parent;
        public List<Entity> subEntityList;
        public List<Entity> removeList;

        protected bool remove = false;
        public bool dead = false;
        public bool immortal = false;
        public bool airborne = false;

        public Direction facing;
        public Direction movementDirection;
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

        public virtual void Remove()
        {
            if (remove == false)
            {
                foreach (Entity e in subEntityList)
                {
                    e.Remove();
                }
                remove = true;
            }
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

        protected Vector2 vectorToEntity(Entity e)
        {
            return new Vector2(
                (this.pos.X + this.center.X) - (e.pos.X + e.center.X),
                (this.pos.Y + this.center.Y) - (e.pos.Y + e.center.Y));
        }

        protected float directionVectorToAngle(Vector2 v)
        {
            return (float)Math.Atan2(-v.Y, -v.X);
        }

        protected float angleToEntity(Entity e)
        {
            return directionVectorToAngle(vectorToEntity(e));
        }

        
    }
        
}
