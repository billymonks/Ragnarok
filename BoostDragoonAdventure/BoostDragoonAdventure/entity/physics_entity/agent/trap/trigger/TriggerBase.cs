﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.utility.trigger;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public abstract class TriggerBase : Agent
    {
        protected Trigger trigger;

        public TriggerBase(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
            : base(w, pos, size, center, solid, factory)
        {
            trigger = new Trigger(this, new List<ITriggerable>(), c => c.isTriggered());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            trigger.Update(gameTime);
        }

        public abstract bool isTriggered();
    }
}