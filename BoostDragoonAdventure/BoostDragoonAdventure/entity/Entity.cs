using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.map;
using FarseerPhysics.Dynamics;
using wickedcrush.manager.audio;
using wickedcrush.helper;
using wickedcrush.entity.physics_entity.agent.attack;
using Microsoft.Xna.Framework.Audio;

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
        protected bool initialized = false;
        public bool dead = false;
        public bool immortal = false;
        public bool airborne = false;
        public bool noCollision = false;

        public Direction facing;
        public Direction movementDirection;

        public SoundManager _sound;
        public AudioEmitter emitter;

        public int id = Helper.getUID();
        #endregion

        #region Initialization
        public Entity(Vector2 pos, Vector2 size, Vector2 center, SoundManager sound)
        {
            Initialize(pos, size, center, sound);
        }

        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, SoundManager sound)
        {
            this.pos = pos;
            this.size = size;
            this.center = center;

            this._sound = sound;
            this.emitter = new AudioEmitter();

            this.name = "Entity";

            subEntityList = new List<Entity>();
            removeList = new List<Entity>();
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            emitter.Position = new Vector3(pos.X, pos.Y, 0f);

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

        public virtual void TakeHit(Attack attack)
        {

        }

        public bool readyForRemoval()
        {
            return remove;
        }

        public bool isInitialized()
        {
            if (!initialized)
            {
                initialized = true;
                return false;
            }

            return true;
        }

        public virtual void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //spriteBatch.Draw(whiteTexture, body, c);
            spriteBatch.Draw(wTex, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, 2, 2), c); 
            spriteBatch.DrawString(f, name, pos, Color.Black);

            foreach (Entity e in subEntityList)
                e.DebugDraw(wTex, aTex, gd, spriteBatch, f, c);
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
