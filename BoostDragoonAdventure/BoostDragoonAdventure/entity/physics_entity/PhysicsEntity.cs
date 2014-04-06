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
    public enum CollisionGroup
    {
        LAYER = 0,
        AGENT = 1
    }

    public class PhysicsEntity : Entity
    {
        protected Body body;

        public PhysicsEntity(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid) : base (pos, size, center)
        {
            Initialize(w, pos, size, center, solid);
        }

        protected virtual void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            setupBody(w, pos, size, center, solid);
        }

        protected virtual void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            body = BodyFactory.CreateBody(w, pos);
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
