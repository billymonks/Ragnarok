using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.attack
{
    public abstract class Attack : Agent
    {
        public int damage, force; // migrate to attackstats class?
        protected bool reactToWall = false, piercing = true, ignoreSameParent = true;

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center, int damage, int force, SoundManager sound, EntityFactory factory)
            : base(w, pos, size, center, false, factory, sound)
        {
            Initialize(damage, force);
        }

        public Attack(World w, Vector2 pos, Vector2 size, Vector2 center, Entity parent, int damage, int force, SoundManager sound, EntityFactory factory)
            : base(w, pos, size, center, false, factory, sound)
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

                    if (!piercing)
                    {
                        this.Remove();
                        //this.remove = true;
                    }
                } else if (reactToWall && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    this.Remove();
                    //this.remove = true;
                }

                c = c.Next;
            }
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Salmon, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, bodies["body"].Position + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Salmon, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            DrawName(spriteBatch, f, camera);
        }
    }
}
