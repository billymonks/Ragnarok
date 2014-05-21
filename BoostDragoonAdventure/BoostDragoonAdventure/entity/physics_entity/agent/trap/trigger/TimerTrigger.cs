﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.utility;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public class TimerTrigger : TriggerBase
    {

        public TimerTrigger(World w, Vector2 pos, EntityFactory factory)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory)
        {
            Initialize();
        }

        private void Initialize()
        {
            immortal = true;
            this.name = "Timer";
            trigger.repeat = true;

            timers.Add("timer", new Timer(1000));
            timers["timer"].start();
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