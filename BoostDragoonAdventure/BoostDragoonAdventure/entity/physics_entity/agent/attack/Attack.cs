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
        protected int damage, force; // migrate to attackstats class?

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center, int damage, int force)
            : base(w, pos, size, center, false, (EntityFactory)null)
        {
            Initialize(damage, force);
        }

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
            : base(w, pos, size, center, false, (EntityFactory)null)
        {
            this.parent = parent;
            Initialize(damage, force);
        }

        private void Initialize(int damage, int force)
        {
            airborne = true;
            immortal = true;
            this.damage = damage;
            this.force = force;
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
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData is Agent
                    && !c.Other.UserData.Equals(this.parent))
                {
                    ((Agent)c.Other.UserData).stats.addTo("hp", -damage);
                    ((Agent)c.Other.UserData).stats.addTo("stagger", -force);
                }

                c = c.Next;
            }
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position, null, Color.Salmon, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center, null, Color.Salmon, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f);
        }
    }
}
