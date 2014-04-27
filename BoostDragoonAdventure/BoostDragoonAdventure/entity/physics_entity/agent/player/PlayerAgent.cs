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
using wickedcrush.behavior.state;
using wickedcrush.behavior;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;
        
        
        private float boostSpeed = 100f;

        
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
            movementDirection = facing;

            SetupStateMachine();
        }

        
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            WalkForward();
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("boosting",
                new State("boosting",
                    c => ((PlayerAgent)c).controls.BoostHeld(),
                    c =>
                    {
                        UpdateDirection();
                        BoostForward();

                        if (controls.ActionPressed())
                        {
                            attackForward();
                        }
                    }));
            ctrl.Add("default",
                new State("default",
                    c => true,
                    c =>
                    {
                        UpdateDirection();
                        WalkForward();

                        if (controls.ActionPressed())
                        {
                            attackForward();
                        }
                    }));

            sm = new StateMachine(ctrl);
        }

        protected void UpdateDirection()
        {
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            if (magnitude == 0f)
                return;

            Direction temp = (Direction)
                        Helper.degreeConversion((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));

            strafe = controls.StrafeHeld();

            if (!strafe)
                facing = temp;

            movementDirection = temp;
        }

        protected void WalkForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;
            
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );

            v += unitVector * magnitude * speed * startingFriction;

            bodies["body"].LinearVelocity = v;
        }

        protected void BoostForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );

            v += unitVector * boostSpeed;

            bodies["body"].LinearVelocity = v;
        }
        #endregion

        
    }
        
}
