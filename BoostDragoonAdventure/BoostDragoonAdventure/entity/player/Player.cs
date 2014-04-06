using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.controls;

namespace wickedcrush.entity.player
{
    public class Player : Entity
    {
        #region Variables
        protected Controls controls;
        #endregion

        #region Initialization
        public Player(Vector2 pos, Vector2 size, Vector2 center, Boolean solid, Controls controls) :base(pos, size, center, solid)
        {
            Initialize(pos, size, center, solid, controls);
        }
        
        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, Boolean solid, Controls controls)
        {
            base.Initialize(pos, size, center, solid);

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
            //accel.X = (float)(controls.LStickXAxis() * 0.2f);
            //accel.Y = (float)(controls.LStickYAxis() * 0.2f);

            //accel.X = (float)Math.Floor(controls.LStickXAxis() * 0.5f);
            //accel.Y = (float)Math.Floor(controls.LStickYAxis() * 0.5f);

            velocity.X = roundTowardZero(velocity.X + controls.LStickXAxis() * 5f);
            velocity.Y = roundTowardZero(velocity.Y + controls.LStickYAxis() * 5f);
            
        }

        //protected bool checkCollision()
        //{

        //}
        #endregion

        
    }
        
}
