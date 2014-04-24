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
using wickedcrush.stats;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.factory.entity;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;
        private float speed = 150f;
        
        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, EntityFactory factory)
            : base(w, pos, size, center, solid, factory)
        {
            Initialize(pos, size, center, solid, controls);

            
        }

        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls)
        {
            this.controls = controls;
            stats = new PersistedStats(5, 5, 5);
            this.name = "Player";

            this.facing = Direction.East;
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
            Vector2 v = bodies["body"].LinearVelocity;
            
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis())),
                (float)Math.Sin(Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()))
            );

            if(magnitude != 0f)
                facing = (Direction)
                    Helper.degreeConversion((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));

            v += unitVector * magnitude * speed * startingFriction;

            bodies["body"].LinearVelocity = v;

            if (controls.ActionPressed())
            {
                attackForward();
            }
            
        }
        #endregion

        
    }
        
}
