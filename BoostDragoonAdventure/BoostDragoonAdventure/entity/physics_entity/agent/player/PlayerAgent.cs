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
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;
        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls) : base (w, pos, size, center, solid)
        {
            Initialize(w, pos, size, center, solid, controls);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls)
        {
            
            this.controls = controls;

            this.name = "Player";
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, body);
            body.FixedRotation = true;
            body.LinearVelocity = Vector2.Zero;
            body.BodyType = BodyType.Dynamic;
            body.CollisionGroup = (short)CollisionGroup.AGENT;

            if (!solid)
                body.IsSensor = true;

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
            if (!(controls.LStickXAxis() == 0f) || !(controls.LStickYAxis() == 0f))
                body.LinearVelocity = new Vector2(controls.LStickXAxis() * 150f, controls.LStickYAxis() * 150f);
            else
                body.LinearVelocity /= 2f;
            //body.ApplyLinearImpulse(new Vector2(controls.LStickXAxis() * 950f, controls.LStickYAxis() * 950f));
            //body.ApplyForce(new Vector2(controls.LStickXAxis() * 150f, controls.LStickYAxis() * 150f));
            
        }
        #endregion

        
    }
        
}
