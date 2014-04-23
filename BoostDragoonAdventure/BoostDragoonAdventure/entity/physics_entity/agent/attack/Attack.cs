using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.factory.entity;

namespace wickedcrush.entity.physics_entity.agent.attack
{
    public abstract class Attack : Agent
    {
        protected int damage;

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center)
            : base(w, pos, size, center, false, (EntityFactory)null)
        {
            Initialize();
        }

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent)
            : base(w, pos, size, center, false, (EntityFactory)null)
        {
            this.parent = parent;
            Initialize();
        }

        private void Initialize()
        {
            immortal = true;
            damage = 5;
            this.name = "Attack";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //if (deployed)
                //Remove();
            //else
                //deployed = true;
        }

        protected override void HandleCollisions()
        {
            var c = body.ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching 
                    && c.Other.UserData is Agent 
                    && !c.Other.UserData.Equals(this.parent))
                    ((Agent)c.Other.UserData).stats.hp -= damage;

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
