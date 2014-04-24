﻿using System;
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
        protected Dictionary<String, Body> bodies;

        public float startingFriction = 0.1f;
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
            bodies = new Dictionary<String, Body>();
            bodies.Add("body", BodyFactory.CreateBody(w, pos - center));
            bodies.Add("hotspot", BodyFactory.CreateBody(w, pos));
            bodies["hotspot"].IsSensor = true;
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePos();
            base.Update(gameTime);
        }

        private void UpdatePos()
        {
            pos.X = bodies["body"].Position.X;
            pos.Y = bodies["body"].Position.Y;
        }

        protected override void Remove()
        {
            if (remove == false)
            {
                base.Remove();
                removeBodies();
            }
        }

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(tex, bodies["body"].Position, null, c, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            
            spriteBatch.DrawString(f, name, pos, Color.Black);

            spriteBatch.Draw(tex, bodies["body"].WorldCenter, null, Color.Yellow, bodies["hotspot"].Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            
            foreach (Entity e in subEntityList)
                e.DebugDraw(tex, gd, spriteBatch, f, c);

        }

        protected void setLocalCenter()
        {
            bodies["body"].LocalCenter = center;
        }

        public void removeBodies()
        {
            foreach (KeyValuePair<String, Body> pair in bodies)
            {
                _w.RemoveBody(pair.Value);
            }
        }
    }
}
