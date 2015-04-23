using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.stats;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;
using wickedcrush.factory.entity;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.attack.projectile
{
    public class Bolt : Attack
    {
        float angle = 0f;
        public Bolt(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force, SoundManager sound, EntityFactory factory)
            : base(w, pos, size, center, damage, force, sound, factory)
        {
            this.parent = parent;
            Initialize(damage, force);
        }

        private void Initialize(int damage, int force)
        {
            stats = new PersistedStats(1, 1);
            facing = parent.facing;
            immortal = false;

            this.damage = damage;
            this.force = force;
            this.name = "Bolt";

            speed = 150f;

            reactToWall = true;
            piercing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            moveForward(speed);

            bodySpriter.setAngle(angle);

            ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), new Vector3(-0.3f, -1f, -0.3f), new Vector3(0.6f, 2f, 0.6f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, "particles", 0, "white_to_red");
            particleEmitter.EmitParticles(ps, this.factory, 1);

            angle += gameTime.ElapsedGameTime.Milliseconds;
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("actionskill", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 5, factory._spriterManager.spriters["all"].loader));

            bodySpriter = sPlayers["actionskill"];
            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);
            bodySpriter.setScale(((float)size.X) / 10f);
            height = 10;

        }

        protected void moveForward(float speed)
        {
            Vector2 v = bodies["body"].LinearVelocity;
            v.X = (float)Math.Cos(MathHelper.ToRadians((float)facing)) * speed;
            v.Y = (float)Math.Sin(MathHelper.ToRadians((float)facing)) * speed;
            bodies["body"].LinearVelocity = v;
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, c, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(wTex, bodies["hotspot"].WorldCenter - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Yellow, bodies["hotspot"].Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f, camera);
        }
    }
}
