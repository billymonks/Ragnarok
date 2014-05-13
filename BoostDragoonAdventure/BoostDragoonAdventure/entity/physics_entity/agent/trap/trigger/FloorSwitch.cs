using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.utility.trigger;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public class FloorSwitch : TriggerBase
    {
        public FloorSwitch(World w, Vector2 pos, EntityFactory factory, ITriggerable target)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), false, factory)
        {

        }

        protected virtual void Initialize(Vector2 pos, Vector2 size, Vector2 center)
        {
            immortal = true;
            this.name = "FloorSwitch";
        }

        public override bool isTriggered()
        {
            throw new NotImplementedException();
        }


    }
}
