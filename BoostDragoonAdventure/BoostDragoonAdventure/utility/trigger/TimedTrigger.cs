/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using Microsoft.Xna.Framework;

namespace wickedcrush.utility.trigger
{

    public class TimedTrigger : Trigger
    {
        private Timer timer;

        public TimedTrigger(Agent parent, ITriggerable target, Timer t)
            : base(parent, target, c => t.isDone())
        {
            timer = t;

            //this.condition = c => timer.isDone();

            timer.resetAndStart();
        }

        public TimedTrigger(Agent parent, List<ITriggerable> targets, Timer t)
            : base(parent, targets, c=> t.isDone())
        {

            timer = t;

            //this.condition = c => timer.isDone();

            timer.resetAndStart();
        }

        

        public override void Update(GameTime gameTime)
        {
            if (done)
                return;

            timer.Update(gameTime);

            if (testCondition())
            {
                activate();


                if (!repeat)
                    done = true;
                else
                    timer.resetAndStart();
            }
        }
    }
}*/
