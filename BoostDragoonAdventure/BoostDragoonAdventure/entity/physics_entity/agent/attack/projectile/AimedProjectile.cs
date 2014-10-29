﻿using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.display._3d;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using wickedcrush.stats;

namespace wickedcrush.entity.physics_entity.agent.attack.projectile
{
    public class AimedProjectile : Attack
    {
        public AimedProjectile(World w, Vector2 pos, Vector2 size, Vector2 center, int damage, int force, int aimedDirection, SoundManager sound)
            : base(w, pos, size, center, damage, force, sound, (EntityFactory)null)
        {
            Initialize(damage, force, aimedDirection);
        }

        public AimedProjectile(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force, int aimedDirection, SoundManager sound)
            : base(w, pos, size, center, damage, force, sound, (EntityFactory)null)
        {
            this.parent = parent;
            Initialize(damage, force, aimedDirection);
        }

        private void Initialize(int damage, int force, int aimedDirection)
        {
            stats = new PersistedStats(1, 1);
            facing = parent.facing;
            immortal = false;

            this.damage = damage;
            this.force = force;
            this.movementDirection = aimedDirection;
            this.name = "AimProj";

            speed = 150f;

            reactToWall = true;
            piercing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            moveForward(speed);
        }

        protected void moveForward(float speed)
        {
            Vector2 v = bodies["body"].LinearVelocity;
            v.X = (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)) * speed;
            v.Y = (float)Math.Sin(MathHelper.ToRadians((float)movementDirection)) * speed;
            bodies["body"].LinearVelocity = v;
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            spriteBatch.Draw(wTex, bodies["hotspot"].WorldCenter - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Yellow, bodies["hotspot"].Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f, camera);
        }
    }
}