using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.utility;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public class TimerTrigger : TriggerBase
    {
        int time;
        public TimerTrigger(World w, Vector2 pos, EntityFactory factory, SoundManager sound, int time)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            this.time = time;
            Initialize();
        }

        private void Initialize()
        {
            immortal = false;
            this.name = "Timer";
            trigger.repeat = true;

            activeRange = 300f;

            timers.Add("timer", new Timer(time));
            //timers["timer"].start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (target == null)
            {
                setTargetToClosestPlayer(false);
                timers["timer"].resetAndStart();
            }
            else if (distanceToTarget() > activeRange /*|| !hasLineOfSightToEntity(target)*/)
            {
                target = null;
                timers["timer"].reset();
            }

            
        }

        public override bool isTriggered()
        {
            if (timers["timer"].isDone())
            {
                timers["timer"].resetAndStart();
                return true;
            }

            return false;
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            bodies["body"].BodyType = BodyType.Static;
        }
    }
}
