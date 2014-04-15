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

        private EntityFactory factory;
        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, EntityFactory factory)
            : base(w, pos, size, center, solid)
        {
            Initialize(pos, size, center, solid, controls);

            this.factory = factory;
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
            Direction movement = (Direction)facing;
            Vector2 v = body.LinearVelocity;
            Vector2 unitVector;

            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            //v.X /= 1.1f;
            //v.Y /= 1.1f;

            unitVector = new Vector2(
                (float)Math.Cos(Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis())) * magnitude,
                (float)Math.Sin(Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis())) * magnitude
            );

            if (Math.Abs(unitVector.X) > Math.Abs(unitVector.Y))
            {
                if (unitVector.X < 0f)
                    facing = Direction.West;
                else
                    facing = Direction.East;
            }
            else if (Math.Abs(unitVector.X) < Math.Abs(unitVector.Y))
            {
                if (v.Y < 0f)
                    facing = Direction.North;
                else
                    facing = Direction.South;
            }

            v += unitVector * magnitude * 150f;

            body.LinearVelocity = v;

            if (controls.ActionPressed())
            {
                factory.addAttack(
                    new Vector2(
                        (float)(pos.X - size.X * Math.Sin(MathHelper.ToRadians((float)facing))), //x component of pos
                        (float)(pos.Y + size.Y * Math.Cos(MathHelper.ToRadians((float)facing)))), //y component of pos
                    size,
                    new Vector2(size.X / 2, size.Y / 2), //center point, useless i think, idk why i bother setting it here, Vector2.Zero could be memory saving
                    this); //set parent to self, don't hurt self

            }
            
        }
        #endregion

        
    }
        
}
