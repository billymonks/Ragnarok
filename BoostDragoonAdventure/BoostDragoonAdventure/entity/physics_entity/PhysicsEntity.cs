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
    public class PhysicsEntity : Entity
    {
        protected Body body;

        public PhysicsEntity(World w, Vector2 pos, Vector2 size, Vector2 center, float density) : base (pos, size, center)
        {
            Initialize(w, pos, size, center, density);
        }

        protected virtual void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, float density)
        {
            setupBody(w, pos, size, center, density);
        }

        protected virtual void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, float density)
        {
            body = BodyFactory.CreateBody(w, pos);
            FixtureFactory.AttachRectangle(size.X, size.Y, density, center, body);
            body.FixedRotation = true;
            body.LinearVelocity = Vector2.Zero;
            body.BodyType = BodyType.Dynamic;
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

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //spriteBatch.Draw(tex, new Rectangle((int)_rectangle.Position.X, (int)_rectangle.Position.Y, (int)size.X, (int)size.Y), c);
            spriteBatch.Draw(tex, body.Position, null, c, body.Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.DrawString(f, name, pos, Color.Black);
        }

        protected void setLocalCenter()
        {
            body.LocalCenter = center;
        }
    }
}
