using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using FarseerPhysics.Factories;
using wickedcrush.utility;
using wickedcrush.utility.trigger;
using wickedcrush.manager.audio;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.helper;
using wickedcrush.particle;


namespace wickedcrush.entity.physics_entity.agent.trap.triggerable.turret
{
    public class Turret : Triggerable
    {
        //private EntityFactory factory;
        float skillVelocity = 0f, blowVelocity = 300f;
        int spreadDuration = 40, blowCount = 8, blowPerSpread=4, scatterCount = 0, spread=20, blowDuration = 200, blowReleaseDelay = 0;


        public Turret(World w, Vector2 pos, EntityFactory factory, Direction facing, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize(facing);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (timers["shot"].isDone())
            {
                //fireBolt();
                useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(skillVelocity, 0f), 
                    spreadDuration, blowCount, blowPerSpread, scatterCount, spread, false, blowVelocity, blowDuration, blowReleaseDelay, new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 500, "particles", 0, "white_to_green"))));
                _sound.addCueInstance("Jump7", "Jump7" + this.id, false);
                _sound.setCueVariable("PitchIncrease", MathHelper.Lerp(0f, 100f, (float)(timers["shot"].getInterval() / 2000.0)), "Jump7" + this.id);
                //_sound.setCueVariable("PitchIncrease", 100f, "Jump7" + this.id);
                _sound.playCueInstance("Jump7" + this.id);
                //_sound.playCue("whsh", emitter);
                timers["shot"].reset();
            }
        }

        private void Initialize(Direction facing)
        {
            //stats = new PersistedStats(5, 5);
            this.name = "Turret";

            this.facing = facing;
            this.movementDirection = (int)facing;

            timers.Add("shot", new Timer(1));
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            
            bodies["body"].BodyType = BodyType.Static;
        }

        

        public override void activate()
        {
            if (!dead)
            {
                _sound.playCue("whsh", emitter);
                fireBolt();
            }
        }

        public override void delayedActivate(double ms)
        {
            //timers["shot"] = new Timer(ms);
            timers["shot"].setInterval(ms);
            timers["shot"].resetAndStart();

        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
