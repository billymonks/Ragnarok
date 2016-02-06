using FarseerPhysics.Dynamics;
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
using Com.Brashmonkey.Spriter.player;
using wickedcrush.particle;
using wickedcrush.helper;

namespace wickedcrush.entity.physics_entity.agent.attack.projectile
{
    public class TrackingProjectile : Attack
    {
        int angle = 0;

        public TrackingProjectile(World w, Vector2 pos, Vector2 size, int damage, int force, int initialAimDirection, Entity parent, Entity target, SoundManager sound, EntityFactory factory)
            : base(-1, w, pos, size, size / 2f, false, damage, force, sound, factory)
        {
            this.parent = parent;
            this.target = target;
            Initialize(damage, force, initialAimDirection);
        }

        private void Initialize(int damage, int force, int initialAimDirection)
        {
            stats = new PersistedStats(1, 1);
            facing = parent.facing;
            immortal = false;
            visible = false;
            
            this.damage = damage;
            this.force = force;
            this.movementDirection = initialAimDirection;
            this.name = "TrackingProjectile";

            speed = 0f;
            targetSpeed = 100f;

            reactToWall = true;
            piercing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (remove)
                return;

            visible = true;

            

            if (target != null)
            {
                angle = (Helper.degreeConversion(angleToEntity(target)) + 360) % 360;
                movementDirection =  (movementDirection + 360) % 360;

                if (angle - movementDirection > 0)
                {
                    movementDirection += 1;
                }
                else
                {
                    movementDirection -= 1;
                }
            }

            moveForward(speed);

            bodySpriter.setAngle(movementDirection);

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-0.3f, -1f, -0.3f), new Vector3(0.6f, 2f, 0.6f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, 16, "particles", 0, "white_to_red");
            particleEmitter.EmitParticles(ps, this.factory, 1);

            //angle += gameTime.ElapsedGameTime.Milliseconds;

            
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("actionskill", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 5, factory._spriterManager.spriters["all"].loader));

            bodySpriter = sPlayers["actionskill"];
            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);
            bodySpriter.setScale((((float)size.X) / 100f) * (2f / factory._gm.camera.zoom));
            height = 10;

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;

            UpdateSpriters();

        }

        protected void moveForward(float speed)
        {
            Vector2 v = bodies["body"].LinearVelocity;
            v.X += (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)) * speed;
            v.Y += (float)Math.Sin(MathHelper.ToRadians((float)movementDirection)) * speed;
            bodies["body"].LinearVelocity = v;
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(wTex, bodies["hotspot"].WorldCenter - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Yellow, bodies["hotspot"].Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f, camera);
        }

        public override void TakeKnockback(Vector2 pos, float force)
        {
            this.Die();
        }

        public override void TakeKnockback(Vector2 pos, int dmg, float force)
        {
            this.Die();
            

        }
    }
}
