using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace wickedcrush.entity.physics_entity.agent.attack.melee
{
    public class MeleeAttack : Attack
    {
        private bool deployed = false;

        public MeleeAttack(World w, Vector2 pos, Vector2 size, Vector2 center, int damage, int force)
            : base(w, pos, size, center, damage, force)
        {
            Initialize(damage, force);
        }

        public MeleeAttack(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force)
            : base(w, pos, size, center, parent, damage, force)
        {
            Initialize(damage, force);
        }

        private void Initialize(int damage, int force)
        {
            immortal = true;
            this.damage = damage;
            this.force = force;
            this.name = "Melee";
            facing = parent.facing;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void HandleCollisions()
        {
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching 
                    && c.Other.UserData is Agent 
                    && !c.Other.UserData.Equals(this.parent))
                    ((Agent)c.Other.UserData).stats.hp -= damage;

                c = c.Next;
            }

            Remove();
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position, null, Color.Salmon, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center, null, Color.Salmon, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f);
        }
    }
}
