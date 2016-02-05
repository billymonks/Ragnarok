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
using wickedcrush.particle;
using wickedcrush.display._3d;
using wickedcrush.entity.physics_entity.agent.action;

namespace wickedcrush.entity.physics_entity.agent.trap.triggerable
{
    public class FloorTrap : Triggerable
    {
        private bool triggered = false, ready = true, pressed = false;

        ParticleStruct fireParticleA, fireParticleB, fireParticleC;

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

            fireParticleA = new ParticleStruct(new Vector3(pos.X, 1f, pos.Y), Vector3.Zero,
                    new Vector3(-0.5f, 1f, -0.5f), new Vector3(1f, 0.25f, 1f), Vector3.Zero, 0, 0, 500, 17, "particles", 0, "white_to_yellow");
            fireParticleB = new ParticleStruct(new Vector3(pos.X, 1f, pos.Y), Vector3.Zero,
                    new Vector3(-0.325f, 1f, -0.325f), new Vector3(0.75f, 1f, 0.75f), Vector3.Zero, 0, 0, 500, 17, "particles", 0, "white_to_orange");
            fireParticleC = new ParticleStruct(new Vector3(pos.X, 1f, pos.Y), Vector3.Zero,
                    new Vector3(-0.25f, 1f, -0.25f), new Vector3(0.5f, 1f, 0.5f), Vector3.Zero, 0, 0, 500, 17, "particles", 0, "white_to_red");
            //trigger.repeat = true;

            AddLight(new PointLightStruct(new Vector4(1f, .8f, .5f, 1f), 0f, new Vector4(1f, .5f, .5f, 1f), 0f, new Vector3(pos.X, 0f, pos.Y), 300f));
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
                    && !(c.Other.UserData is ActionSkill)
                    && !c.Other.UserData.Equals(this)
                    && !((Agent)c.Other.UserData).noCollision)
                {
                    if (!((Agent)c.Other.UserData).airborne)
                    {
                        pressed = true;
                    }

                    if (triggered)
                    {
                        ((Agent)c.Other.UserData).TakeKnockback(pos + center, 3, 5000);

                    }
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

                SetTargetDiffuseIntensity(0f);
                SetTargetSpecularIntensity(0f);
            }

            if (timers["spike"].isDone())
            {
                triggered = true;
                timers["spike"].reset();
                timers["spike_reset"].resetAndStart();
                SetTargetDiffuseIntensity(1f);
                SetTargetSpecularIntensity(1f);
            }

            if(triggered)
            {
                
                this.EmitParticles(fireParticleA, 1);
                this.EmitParticles(fireParticleB, 1);
                this.EmitParticles(fireParticleC, 1);
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
