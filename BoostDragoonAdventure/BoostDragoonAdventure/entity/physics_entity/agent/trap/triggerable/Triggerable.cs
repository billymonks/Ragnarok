using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;

namespace wickedcrush.entity.physics_entity.agent.trap.triggerable
{
    public abstract class Triggerable : Agent
    {
        public Triggerable(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
            : base(w, pos, size, center, solid, factory)
        {
            this.factory = factory;
        }

        public abstract void activate();
    }
}
