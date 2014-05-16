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
        private bool triggered = false, ready = true, pressed = false;

        public FloorSwitch(World w, Vector2 pos, EntityFactory factory)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), false, factory)
        {
            Initialize();
        }

        private void Initialize()
        {
            immortal = true;
            noCollision = true;
            this.name = "FloorSwitch";
            trigger.repeat = true;
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            //bodies["body"].BodyType = BodyType.Static;
            bodies["body"].IsSensor = true;
        }

        public override bool isTriggered()
        {
            return triggered;
        }

        protected override void HandleCollisions()
        {
            if(!pressed)
                ready = true;
            else
                ready = false;

            pressed = false;
            triggered = false;

            var c = bodies["hotspot"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && (c.Other.UserData is Agent)
                    && !c.Other.UserData.Equals(this))
                {
                    if(!((Agent)c.Other.UserData).airborne)
                        pressed = true;
                }

                c = c.Next;
            }

            if (pressed && ready)
                triggered = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
