using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;

namespace wickedcrush.entity.physics_entity.agent.trap.triggerable
{
    public abstract class Triggerable : Agent
    {
        public Triggerable(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound)
            : base(w, pos, size, center, solid, factory, sound)
        {
            this.factory = factory;
        }

        public abstract void activate();

        public abstract void delayedActivate(double ms);
    }
}
