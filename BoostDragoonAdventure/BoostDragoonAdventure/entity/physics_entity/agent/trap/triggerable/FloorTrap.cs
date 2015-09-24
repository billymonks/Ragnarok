using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.entity.physics_entity.agent.player;

namespace wickedcrush.entity.physics_entity.agent.trap.triggerable
{
    public class FloorTrap : Triggerable
    {
        private bool triggered = false, ready = true, pressed = false;

        public FloorTrap(World w, Vector2 pos, EntityFactory factory, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize();
            //activeRange = 300;
        }

        private void Initialize()
        {
            immortal = true;
            noCollision = true;
            this.name = "FloorTrap";
            timers.Add("spike", new utility.Timer(500));
            timers.Add("spike_reset", new utility.Timer(500));
            //trigger.repeat = true;
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

        protected override void HandleCollisions()
        {
            pressed = false;
            //triggered = false;

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && (c.Other.UserData is Agent)
                    && !c.Other.UserData.Equals(this)
                    && !((Agent)c.Other.UserData).noCollision)
                {
                    if (!((Agent)c.Other.UserData).airborne)
                    {
                        pressed = true;
                    }

                    if(triggered)
                        ((Agent)c.Other.UserData).TakeKnockback(pos + center, 5000);
                }


                c = c.Next;
            }

            if (pressed && ready)
            {
                ready = false;
                //triggered = true;
                timers["spike"].resetAndStart();
                factory.addText("!", pos + center + new Vector2(0, -10), 500);
                _sound.playCue("0x53");
                //factory.createBooleanChoiceScreen("Are you you?", new Vector2(200f, 200f), "choiceX");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (timers["spike_reset"].isActive())
            {
                bodySpriter.setAnimation("pressed", 0, 0);
            }
            else
            {
                bodySpriter.setAnimation("unpressed", 0, 0);
            }

            if (timers["spike_reset"].isDone())
            {
                timers["spike_reset"].reset();
                triggered = false;
                ready = true;
            }

            if (timers["spike"].isDone())
            {
                triggered = true;
                timers["spike"].reset();
                timers["spike_reset"].resetAndStart();
            }
            
            

            //if (factory.checkBool("choiceX"))
            //{
            //factory.createTextScreen("Now you did it!", new Vector2(200f, 200f));

            //factory.savedBools.Remove("choiceX");
            //}


        }

        public override void activate()
        {
            if (!dead)
            {
                //timers["shot"].setInterval(shotTimer);
                //timers["shot"].resetAndStart();
            }
        }

        public override void delayedActivate(double ms)
        {
            //timers["shot"] = new Timer(ms);
            timers["spike"].setInterval(ms);
            timers["spike"].resetAndStart();

        }
    }
}
