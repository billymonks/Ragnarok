using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.entity.physics_entity
{
    public enum CollisionGroup
    {
        LAYER = 0,
        AGENT = 1
    }

    public class PhysicsEntity : Entity
    {
        protected World _w;
        protected Body body, hotSpot;

        public float startingFriction = 1f;
        public float stoppingFriction = 0.1f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

        public PhysicsEntity(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid) : base (pos, size, center)
        {
            _w = w;
            Initialize(w, pos, size, center, solid);
        }

        protected virtual void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            setupBody(w, pos, size, center, solid);
        }

        protected virtual void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            body = BodyFactory.CreateBody(w, pos - center);
            hotSpot = BodyFactory.CreateBody(w, pos);
            hotSpot.IsSensor = true;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePos();
            base.Update(gameTime);
        }

        private void UpdatePos()
        {
            pos.X = body.Position.X;
            pos.Y = body.Position.Y;
        }

        protected override void Remove()
        {
            if (remove == false)
            {
                base.Remove();
                _w.RemoveBody(body);
                _w.RemoveBody(hotSpot);
            }
        }

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(tex, body.Position, null, c, body.Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            spriteBatch.DrawString(f, name, pos, Color.Black);
            
            foreach (Entity e in subEntityList)
                e.DebugDraw(tex, gd, spriteBatch, f, c);

        }

        protected void setLocalCenter()
        {
            body.LocalCenter = center;
        }

        public void removeBody(World w)
        {
            w.RemoveBody(body);
        }
    }
}
