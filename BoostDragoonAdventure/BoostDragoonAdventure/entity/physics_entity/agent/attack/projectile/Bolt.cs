﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.stats;

namespace wickedcrush.entity.physics_entity.agent.attack.projectile
{
    public class Bolt : Attack
    {
        private float speed = 150f;

        public Bolt(World w, Vector2 pos, Vector2 size, Vector2 center)
            : base(w, pos, size, center)
        {
            Initialize();
        }

        public Bolt(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent)
            : base(w, pos, size, center)
        {
            this.parent = parent;
            Initialize();
        }

        private void Initialize()
        {
            stats = new PersistedStats(1, 1, 0);
            facing = parent.facing;
            immortal = false;
            damage = 1;
            this.name = "Bolt";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            moveForward(speed);
        }

        protected void moveForward(float speed)
        {
            Vector2 v = body.LinearVelocity;
            v.X = (float)Math.Cos(MathHelper.ToRadians((float)facing)) * speed;
            v.Y = (float)Math.Sin(MathHelper.ToRadians((float)facing)) * speed;
            body.LinearVelocity = v;
        }

        protected override void HandleCollisions()
        {
            var c = body.ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && !c.Other.UserData.Equals(this.parent))
                {
                    if(c.Other.UserData is Agent)
                        ((Agent)c.Other.UserData).stats.hp -= damage;
                    Remove();
                }

                c = c.Next;
            }
        }

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(tex, body.Position, null, Color.Salmon, body.Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            spriteBatch.DrawString(f, name, pos, Color.Black);
        }
    }
}
