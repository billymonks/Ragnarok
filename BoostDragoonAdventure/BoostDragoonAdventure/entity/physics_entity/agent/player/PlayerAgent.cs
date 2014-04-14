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
            stats = new PersistedStats(5, 5, 5);
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
            Vector2 v = body.LinearVelocity;

            v.X /= 1.1f;
            v.Y /= 1.1f;

            if (!(controls.LStickXAxis() == 0f))
                v.X += controls.LStickXAxis() * 150f;
            if (!(controls.LStickYAxis() == 0f))
                v.Y += controls.LStickYAxis() * 150f;

            body.LinearVelocity = v;

            if (controls.ActionPressed())
            {
                Attack a = new Attack(_w, new Vector2(pos.X + size.X, pos.Y), size, new Vector2(size.X/2, size.Y/2));
                a.parent = this;
                subEntityList.Add(a);
            }
            
        }
        #endregion

        
    }
        
}
