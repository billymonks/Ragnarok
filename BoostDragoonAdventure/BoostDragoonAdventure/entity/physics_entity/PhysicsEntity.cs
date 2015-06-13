using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;

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

        public float startingFriction = 0.5f;
        public float stoppingFriction = 0.4f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

        public PhysicsEntity(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, SoundManager sound)
            : base(pos, size, center, sound)
        {
            _w = w;
            Initialize(w, pos, size, center, solid);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            setupBody(w, pos, size, center, solid);
        }

        protected virtual void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            bodies = new Dictionary<String, Body>();
            bodies.Add("body", BodyFactory.CreateBody(w, pos - center));
            //bodies.Add("hotspot", BodyFactory.CreateBody(w, pos));

            if (!solid)
                bodies["body"].IsSensor = true;

            //bodies["hotspot"].IsSensor = true;
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

        public void SetPos(Vector2 pos)
        {
            this.pos = pos;
            if(this.bodies.ContainsKey("body"))
                this.bodies["body"].Position = pos - center;
        }

        public override void AddLinearVelocity(Vector2 v)
        {
            foreach(KeyValuePair<string, Body> body in bodies)
                body.Value.LinearVelocity += v;
        }

        public override void Remove()
        {
            removeBodies();
            dead = true;
            base.Remove();
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            if (visible)
            {
                spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X,
                        camera.cameraPosition.Y), null, c, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
                spriteBatch.Draw(aTex, bodies["body"].Position + center - new Vector2(camera.cameraPosition.X,
                        camera.cameraPosition.Y), null, c, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);

                DrawName(spriteBatch, f, camera);

                //spriteBatch.Draw(wTex, bodies["body"].WorldCenter - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Yellow, bodies["hotspot"].Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            }
            foreach (Entity e in subEntityList)
                e.DebugDraw(wTex, aTex, gd, spriteBatch, f, c, camera);

        }

        protected void DrawName(SpriteBatch spriteBatch, SpriteFont f, Camera camera)
        {
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 11) - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 9) - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(1, 10) - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(-1, 10) - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), Color.Black);
            spriteBatch.DrawString(f, name, pos - new Vector2(0, 10) - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), Color.White);
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

            bodies.Clear();
        }
    }
}
