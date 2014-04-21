using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using wickedcrush.utility;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using FarseerPhysics.Dynamics;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Murderer : Agent
    {
        private EntityFactory factory;

        public Murderer(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
            : base(w, pos, size, center, solid)
        {
            Initialize();
            this.stats = stats;
        }

        private void Initialize()
        {
            stats = new PersistedStats(10, 10, 5);
            this.name = "Murderer";

            timers.Add("navigation", new Timer(500));
            timers["navigation"].start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (timers["navigation"].isDone())
            {
                createPathToTarget();
                timers["navigation"].resetAndStart();
            }
        }
    }
}
