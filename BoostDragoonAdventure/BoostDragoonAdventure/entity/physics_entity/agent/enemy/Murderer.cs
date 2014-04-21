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
        private Timer navTimer;
        private Timer timer;

        public Murderer(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
            : base(w, pos, size, center, solid)
        {
            Initialize();
            this.stats = stats;
        }

        private void Initialize()
        {
            stats = new PersistedStats(15, 15, 5);
            this.name = "Murderer";

            timer = new Timer();
            navTimer = new Timer(500);
            navTimer.start();
            //timer.start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer.Update(gameTime);
            navTimer.Update(gameTime);

            if (navTimer.isDone())
            {
                createPathToTarget();
                navTimer.resetAndStart();
            }
            
            //if (timer.isDone())
            //{
                //timer.resetAndStart();
            //}
        }
    }
}
