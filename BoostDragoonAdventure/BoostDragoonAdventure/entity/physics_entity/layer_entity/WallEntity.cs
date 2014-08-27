using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.controls;
using wickedcrush.helper;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.manager.audio;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class WallEntity : PhysicsEntity
    {
        #region Variables

        #endregion

        #region Initialization
        public WallEntity(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, SoundManager sound)
            : base(w, pos, size, center, solid, sound)
        {
            Initialize(w, pos, size, center, solid);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {

            this.name = "Wall";
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, bodies["body"]);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Static;
            bodies["body"].CollisionGroup = (short)CollisionGroup.LAYER;

            if (!solid)
                bodies["body"].IsSensor = true;

        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //public override void DebugDraw(GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        //{
            //base.DebugDraw(gd, spriteBatch, f, c);

        //}
        #endregion

        
    }
        
}
