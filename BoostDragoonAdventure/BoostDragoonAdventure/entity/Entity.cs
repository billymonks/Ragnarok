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
using wickedcrush.display._3d;
using wickedcrush.inventory;
using wickedcrush.manager.gameplay;

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

    public class Entity : IComparable<Entity>
    {
        #region Variables
        public Vector2 pos, size, center;
        protected String name;

        public Entity parent;
        public Dictionary<String, Entity> subEntityList;
        public List<String> removeList;

        public bool remove = false;
        protected bool initialized = false;
        public bool dead = false;
        public bool immortal = false;
        public bool airborne = false;
        public bool noCollision = false;
        public bool visible = true;

        public Direction facing;
        public int movementDirection;
        public int aimDirection;

        public SoundManager _sound;
        public AudioEmitter emitter;

        public int id = Helper.getUID();

        public int height = 0, skillHeight = 0;

        public Weapon weaponInUse = null;

        protected TimeSpan elapsed;
        
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

            subEntityList = new Dictionary<String, Entity>();
            removeList = new List<String>();
        }
        #endregion

        #region Update
        public virtual void Update(GameTime gameTime)
        {
            elapsed = gameTime.ElapsedGameTime;
            emitter.Position = new Vector3(pos.X+center.X, pos.Y+center.Y, 0f);

            foreach (KeyValuePair<String, Entity> e in subEntityList)
            {
                e.Value.Update(gameTime);
                
                if (e.Value.remove)
                    removeList.Add(e.Key);
            }

            foreach (String e in removeList)
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
                foreach (KeyValuePair<String, Entity> e in subEntityList)
                {
                    e.Value.Remove();
                }

                Dispose();

                remove = true;
            }
        }

        protected virtual void Dispose()
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

        public virtual void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {
            foreach (KeyValuePair<String, Entity> e in subEntityList)
            {
                e.Value.Draw(depthPass, lightList, gameplay);
            }
        }

        public virtual void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            //spriteBatch.Draw(whiteTexture, body, c);
            spriteBatch.Draw(wTex, new Rectangle((int)pos.X - 1, (int)pos.Y - 1, 2, 2), c); 
            spriteBatch.DrawString(f, name, 
                pos
                - new Vector2(camera.cameraPosition.X,
                    camera.cameraPosition.Y), 
                Color.Black);

            foreach (KeyValuePair<String, Entity> e in subEntityList)
                e.Value.DebugDraw(wTex, aTex, gd, spriteBatch, f, c, camera);
        }

        public virtual void FreeDraw()
        {

        }

        protected Vector2 vectorToEntity(Entity e)
        {
            return new Vector2(
                (this.pos.X + this.center.X) - (e.pos.X + e.center.X),
                (this.pos.Y + this.center.Y) - (e.pos.Y + e.center.Y));
        }

        protected float angleToPos(Vector2 pos)
        {
            return directionVectorToAngle(vectorToPos(pos));
        }

        protected Vector2 vectorToPos(Vector2 pos)
        {
            return new Vector2(
                (this.pos.X + this.center.X) - (pos.X),
                (this.pos.Y + this.center.Y) - (pos.Y));
        }

        protected float directionVectorToAngle(Vector2 v)
        {
            return (float)Math.Atan2(-v.Y, -v.X);
        }

        protected float angleToEntity(Entity e)
        {
            return directionVectorToAngle(vectorToEntity(e));
        }

        public int distanceToEntity(Entity e)
        {
            return (int)Math.Sqrt(Math.Pow((pos.X + center.X) - (e.pos.X + e.center.X), 2)
                + Math.Pow((pos.Y + center.Y) - (e.pos.Y + e.center.Y), 2));
        }

        public int distanceToPosition(Vector2 searchPos)
        {
            return (int)Math.Sqrt(Math.Pow((pos.X + center.X) - (searchPos.X), 2)
                + Math.Pow((pos.Y + center.Y) - (searchPos.Y), 2));
        }

        public int CompareTo(Entity other)
        {
            if (other.pos.Y + other.size.Y > pos.Y + size.Y)
            {
                return -1;
            }
            else if (other.pos.Y + other.size.Y == pos.Y + size.Y)
            {
                return 0;
            }
            else return 1;
        }

        public void PlayCue(String name)
        {
            _sound.playCue(name, emitter);
        }

        public virtual void AddLinearVelocity(Vector2 v)
        {
            
        }
    }
        
}
