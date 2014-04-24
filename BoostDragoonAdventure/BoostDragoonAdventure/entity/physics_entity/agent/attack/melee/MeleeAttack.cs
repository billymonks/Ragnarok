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

        public MeleeAttack(World w, Vector2 pos, Vector2 size, Vector2 center)
            : base(w, pos, size, center)
        {
            Initialize();
        }

        public MeleeAttack(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent)
            : base(w, pos, size, center)
        {
            this.parent = parent;
            Initialize();
        }

        private void Initialize()
        {
            immortal = true;
            damage = 5;
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

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(tex, bodies["body"].Position, null, Color.Salmon, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            DrawName(spriteBatch, f);
        }
    }
}
