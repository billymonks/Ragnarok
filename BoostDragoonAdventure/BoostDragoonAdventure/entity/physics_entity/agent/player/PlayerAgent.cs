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

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : PhysicsEntity
    {
        #region Variables
        protected Controls controls;
        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, float density, Controls controls) : base (w, pos, size, center, density)
        {
            Initialize(w, pos, size, center, density, controls);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, float density, Controls controls)
        {
            this.controls = controls;

            this.name = "Player";
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateMovement();
        }

        protected void UpdateMovement()
        {
            body.LinearVelocity = new Vector2(controls.LStickXAxis() * 150f, controls.LStickYAxis() * 150f);
            
            
        }

        //public override void DebugDraw(GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        //{
            //base.DebugDraw(gd, spriteBatch, f, c);

        //}
        #endregion

        
    }
        
}
