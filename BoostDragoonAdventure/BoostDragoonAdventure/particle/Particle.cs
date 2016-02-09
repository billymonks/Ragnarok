using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.factory.entity;
using wickedcrush.utility;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.particle
{
    public class ParticleStruct
    {
        public Vector3 pos, posVariance, velocity, velocityVariance, acceleration;
        public float rotation, rotationSpeed, scale, scaleSpeed;

        public double milliseconds;

        public String spriterName, animationName;
        public int entityIndex, msPerFrame;

        public ParticleStruct(
            Vector3 pos,
            Vector3 posVariance,
            Vector3 velocity,
            Vector3 velocityVariance,
            Vector3 acceleration, 
            float rotation, 
            float rotationSpeed, 
            double milliseconds,
            int msPerFrame,
            String spriterName, 
            int entityIndex,
            String animationName)
        {
            this.pos = pos;
            this.posVariance = posVariance;
            this.velocity = velocity;
            this.velocityVariance = velocityVariance;
            this.acceleration = acceleration;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            this.milliseconds = milliseconds;
            this.msPerFrame = msPerFrame;
            this.spriterName = spriterName;
            this.entityIndex = entityIndex;
            this.animationName = animationName;
            this.scale = 1f;
            this.scaleSpeed = 0.02f;
        }

        public ParticleStruct(
            Vector3 pos,
            Vector3 posVariance,
            Vector3 velocity,
            Vector3 velocityVariance,
            Vector3 acceleration,
            float rotation,
            float rotationSpeed,
            double milliseconds,
            int msPerFrame,
            String spriterName,
            int entityIndex,
            String animationName,
            float scale,
            float scaleSpeed)
        {
            this.pos = pos;
            this.posVariance = posVariance;
            this.velocity = velocity;
            this.velocityVariance = velocityVariance;
            this.acceleration = acceleration;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            this.milliseconds = milliseconds;
            this.msPerFrame = msPerFrame;
            this.spriterName = spriterName;
            this.entityIndex = entityIndex;
            this.animationName = animationName;
            this.scale = scale;
            this.scaleSpeed = scaleSpeed;
        }

        public ParticleStruct(
            Vector3 pos,
            Vector3 posVariance,
            Vector3 velocity,
            Vector3 velocityVariance,
            Vector3 acceleration,
            float rotation,
            float rotationSpeed,
            double milliseconds,
            int msPerFrame,
            String spriterName,
            int entityIndex,
            String animationName,
            float scale)
        {
            this.pos = pos;
            this.posVariance = posVariance;
            this.velocity = velocity;
            this.velocityVariance = velocityVariance;
            this.acceleration = acceleration;
            this.rotation = rotation;
            this.rotationSpeed = rotationSpeed;
            this.milliseconds = milliseconds;
            this.msPerFrame = msPerFrame;
            this.spriterName = spriterName;
            this.entityIndex = entityIndex;
            this.animationName = animationName;
            this.scale = scale;
            this.scaleSpeed = 0.02f;
        }
    }
    public class Particle
    {
        public Vector3 pos, velocity, acceleration;
        public float rotation, rotationSpeed, scale, scaleChange;

        public SpriterOffsetStruct animation;
        public Timer duration;

        public bool readyForRemoval = false;

        public EntityFactory factory;

        public Particle(ParticleStruct p, EntityFactory factory)
        {
            this.pos = p.pos + new Vector3(p.posVariance.X * (float)factory.random.NextDouble(),
                    p.posVariance.Y * (float)factory.random.NextDouble(),
                    p.posVariance.Z * (float)factory.random.NextDouble());
            this.velocity = p.velocity 
                + new Vector3(p.velocityVariance.X * (float)factory.random.NextDouble(), 
                    p.velocityVariance.Y * (float)factory.random.NextDouble(), 
                    p.velocityVariance.Z * (float)factory.random.NextDouble());
            this.acceleration = p.acceleration;
            this.rotation = p.rotation;
            this.rotationSpeed = p.rotationSpeed;

            duration = new Timer(p.milliseconds);
            duration.resetAndStart();

            animation = new SpriterOffsetStruct(
                new SpriterPlayer(
                    factory._spriterManager.spriters[p.spriterName].getSpriterData(), 
                    p.entityIndex,
                    factory._spriterManager.spriters[p.spriterName].loader), 
                    Vector3.Zero, 0);

            animation.player.setAnimation(p.animationName, 0, 0);
            animation.player.setFrameSpeed(p.msPerFrame);

            this.scale = p.scale;
            this.scaleChange = p.scaleSpeed;

            animation.player.setScale(scale);

            Vector2 spritePos = new Vector2(
                (pos.X + animation.offset.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                ((pos.Z + animation.offset.Y - factory._gm.camera.cameraPosition.Y - pos.Y) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                );

            animation.player.update(spritePos.X, spritePos.Y);

            this.factory = factory;
        }

        public void Update(GameTime gameTime)
        {
            scale += scaleChange;
            rotation += rotationSpeed;
            Vector2 spritePos = new Vector2(
                (pos.X + animation.offset.X - factory._gm.camera.cameraPosition.X) * (2f / factory._gm.camera.zoom) * 2.25f - 500 * (2f - factory._gm.camera.zoom),
                ((pos.Z + animation.offset.Y - factory._gm.camera.cameraPosition.Y - pos.Y) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100)
                );

            if (spritePos.Y <= -200f && spritePos.Y >= 800f)
            {
                return;
            }

            float poopNear = -0.236f, poopFar = 0.06199996f;
            float temp = ((pos.Z + animation.offset.Y - factory._gm.camera.cameraPosition.Y) * (2f / factory._gm.camera.zoom) * -2.25f * (float)(Math.Sqrt(2) / 2) + 240 * (2f - factory._gm.camera.zoom) - 100);
            float depth = MathHelper.Lerp(0.97f + poopNear, 0.37f + poopFar, temp / -1080f); //so bad

            //depth = 0f;

            animation.player.SetDepth(depth);
            animation.player.setScale(scale);
            animation.player.setAngle(rotation);
            animation.player.update(spritePos.X,
                spritePos.Y);

            duration.Update(gameTime);

            if (duration.isDone())
                Remove();

            pos += velocity * ((float)gameTime.ElapsedGameTime.Milliseconds / 16f);
            velocity += acceleration * ((float)gameTime.ElapsedGameTime.Milliseconds / 16f);
        }

        public void Draw(bool depthPass)
        {

            //float top = sPlayer.getBoundingBox().top;
            //float bottom = sPlayer.getBoundingBox().bottom;

            factory._gm._screen.spriteEffect.Parameters["depth"].SetValue(animation.player.depth);
            factory._spriterManager.DrawPlayer(animation.player);
        }

        public void Remove()
        {
            readyForRemoval = true;
        }
    }
}
