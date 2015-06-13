using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.utility.trigger;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.manager.audio;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.utility;

namespace wickedcrush.entity.physics_entity.agent.trap.trigger
{
    public class FloorSwitch : TriggerBase
    {
        private bool triggered = false, ready = true, pressed = false;

        public FloorSwitch(World w, Vector2 pos, EntityFactory factory, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), false, factory, sound)
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

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("trap", new SpriterPlayer(factory._spriterManager.spriters["trap"].getSpriterData(), 0, factory._spriterManager.spriters["trap"].loader));
            bodySpriter = sPlayers["trap"];
            bodySpriter.setFrameSpeed(20);

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

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && (c.Other.UserData is Agent)
                    && !c.Other.UserData.Equals(this))
                {
                    if (!((Agent)c.Other.UserData).airborne)
                    {
                        pressed = true;
                    }
                }

                c = c.Next;
            }

            if (pressed && ready)
            {
                triggered = true;
                //factory.createBooleanChoiceScreen("Are you you?", new Vector2(200f, 200f), "choiceX");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (pressed)
            {
                bodySpriter.setAnimation("pressed_001", 0, 0);
            }
            else
            {
                bodySpriter.setAnimation("unpressed_001", 0, 0);
            }

            if (factory.checkBool("choiceX"))
            {
                //factory.createTextScreen("Now you did it!", new Vector2(200f, 200f));
                
                //factory.savedBools.Remove("choiceX");
            }
        }
    }
}
