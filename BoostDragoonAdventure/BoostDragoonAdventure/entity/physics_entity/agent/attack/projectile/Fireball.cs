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
using wickedcrush.utility;
using wickedcrush.factory.entity;

namespace wickedcrush.entity.physics_entity.agent.attack.projectile
{
    public class Fireball : Attack
    {
        int clusterCount = 0;
        int flightTime = 400;

        public Fireball(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, Direction facing, int damage, int force, int clusterCount, SoundManager sound, EntityFactory factory)
            : base(w, pos, size, center, damage, force, sound, factory)
        {
            this.parent = parent;
            Initialize(damage, force, clusterCount, facing);
        }

        private void Initialize(int damage, int force, int clusterCount, Direction facing)
        {
            stats = new PersistedStats(1, 1);
            this.facing = facing;
            immortal = false;

            this.damage = damage;
            this.force = force;
            this.clusterCount = clusterCount;

            flightTime += clusterCount * 200;
            this.name = "Fireball";

            timers.Add("flightTime", new Timer(flightTime));
            timers["flightTime"].resetAndStart();

            speed = 150f;

            reactToWall = true;
            piercing = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            moveForward(speed);

            checkTimer();
        }

        protected void moveForward(float speed)
        {
            Vector2 v = bodies["body"].LinearVelocity;
            v.X = (float)Math.Cos(MathHelper.ToRadians((float)facing)) * speed;
            v.Y = (float)Math.Sin(MathHelper.ToRadians((float)facing)) * speed;
            bodies["body"].LinearVelocity = v;
        }

        private void checkTimer()
        {
            if(timers["flightTime"].isDone())
            {

                cluster();
                Remove();
            }
        }

        private void cluster()
        {
            if (clusterCount <= 0)
                return;
            
            clusterCount--;
            if (clusterCount % 4 == 0)
            {
                factory.addFireball(pos, size, center, this.parent, Direction.East, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.SouthWest, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.NorthWest, this.damage, this.force, clusterCount);
            }
            else if (clusterCount % 4 == 1)
            {
                factory.addFireball(pos, size, center, this.parent, Direction.West, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.SouthEast, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.NorthEast, this.damage, this.force, clusterCount);
            }
            else if (clusterCount % 4 == 2)
            {
                factory.addFireball(pos, size, center, this.parent, Direction.South, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.NorthEast, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.NorthWest, this.damage, this.force, clusterCount);
            }
            else if (clusterCount % 4 == 3)
            {
                factory.addFireball(pos, size, center, this.parent, Direction.North, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.SouthEast, this.damage, this.force, clusterCount);
                factory.addFireball(pos, size, center, this.parent, Direction.SouthWest, this.damage, this.force, clusterCount);
            }
            
        }

        protected override void HandleCollisions()
        {
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !((Agent)c.Other.UserData).noCollision
                    && !c.Other.UserData.Equals(this.parent))
                {
                    if (this.parent != null
                        && ((Agent)c.Other.UserData).parent != null
                        && ((Agent)c.Other.UserData).parent.Equals(this.parent)
                        && ignoreSameParent)
                        break;

                    ((Agent)c.Other.UserData).TakeHit(this);

                    
                    Remove();
                }
                else if (reactToWall && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    cluster();
                    Remove();
                }

                c = c.Next;
            }
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
