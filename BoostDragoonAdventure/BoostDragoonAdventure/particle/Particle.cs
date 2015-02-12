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
    public struct ParticleStruct
    {
        public Vector3 pos, velocity, acceleration;
        public float rotation, rotationSpeed;

        public double milliseconds;

        public String spriterName;
        public int entityIndex;
    }
    public class Particle
    {
        public Vector3 pos, velocity, acceleration;
        public float rotation, rotationSpeed;

        public SpriterOffsetStruct animation;
        public Timer duration;

        public bool readyForRemoval = false;

        public EntityFactory factory;

        public Particle(ParticleStruct p, EntityFactory factory)
        {
            this.pos = p.pos;
            this.velocity = p.velocity;
            this.acceleration = p.acceleration;
            this.rotation = p.rotation;
            this.rotationSpeed = p.rotationSpeed;

            duration = new Timer(p.milliseconds);
            duration.resetAndStart();

            animation = new SpriterOffsetStruct(
                new SpriterPlayer(
                    factory._spriterManager.spriters[p.spriterName].getSpriterData(), 
                    p.entityIndex, 
                    factory._spriterManager.loaders["loader1"]), 
                    Vector2.Zero);

            this.factory = factory;
        }

        public void Update(GameTime gameTime)
        {
            duration.Update(gameTime);

            if (duration.isDone())
                readyForRemoval = true;

            pos += velocity * ((float)gameTime.ElapsedGameTime.Milliseconds / 16f);
            velocity += acceleration * ((float)gameTime.ElapsedGameTime.Milliseconds / 16f);
        }

        public void Draw()
        {
            Vector2 spritePos = new Vector2(pos.X + animation.offset.X - factory._gm.camera.cameraPosition.X,
                pos.Y + animation.offset.Y - factory._gm.camera.cameraPosition.Y - pos.Z);

            if (spritePos.Y <= -200f && spritePos.Y >= 800f)
            {
                return;
            }

            float near = -200f;
            float far = 800f;

            float depth = 1f - ((((spritePos.Y + pos.Z) * 1.03f - near - animation.offset.Y)) / (far - near));

            //float depth = 0f;

            animation.player.SetDepth(depth);

            animation.player.update(spritePos.X * 2.25f,
                (spritePos.Y * -2.25f * (float)(Math.Sqrt(2) / 2) - 100));


            //float top = sPlayer.getBoundingBox().top;
            //float bottom = sPlayer.getBoundingBox().bottom;

            factory._spriterManager.DrawPlayer(animation.player);
        }
    }
}
