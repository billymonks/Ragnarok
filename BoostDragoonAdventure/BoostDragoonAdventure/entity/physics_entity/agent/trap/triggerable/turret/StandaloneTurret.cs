﻿using System;
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
    public class StandaloneTurret : Triggerable
    {
        //private EntityFactory factory;
        float skillVelocity = 0f, blowVelocity = 300f;
        int spreadDuration = 60, blowCount = 8, blowPerSpread=1, scatterCount = 0, spread=1, blowDuration = 200, blowReleaseDelay = 0, shotTimer = 333, rank = 3;


        public StandaloneTurret(int id, World w, Vector2 pos, EntityFactory factory, Direction facing, SoundManager sound, int rank)
            : base(id, w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize(facing, rank);
            activeRange = 300;
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (target == null)
            {
                setTargetToClosestPlayer(true, 5);
                if (target != null)
                {
                    timers["shot"].resetAndStart();
                    factory.addText("!", pos + center + new Vector2(0, -10), 500);
                    //TellProximityTarget(target);
                    //enemyState = EnemyState.Alert;
                    _sound.playCue("0x53");
                }
            }
            else if (!timers["shot"].isActive() && !timers["shot"].isDone())
            {
                timers["shot"].resetAndStart();
            }

            if (timers["shot"].isDone())
            {
                useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(skillVelocity, 0f), 
                    spreadDuration, blowCount, blowPerSpread, scatterCount, spread, false, blowVelocity, blowDuration, blowReleaseDelay, 1f,
                    new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 100, 10, "particles", 0, "white_to_green")),
                    "", 0, "",
                    SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 50, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, false),
                    true));
                
                _sound.addCueInstance("Jump7", "Jump7" + this.id, false);
                _sound.setCueVariable("PitchIncrease", MathHelper.Lerp(0f, 100f, (float)(timers["shot"].getInterval() / 1000.0)), "Jump7" + this.id);
                _sound.playCueInstance("Jump7" + this.id);
                
                timers["shot"].reset();
                target = null;
            }
        }

        private void Initialize(Direction facing, int rank)
        {
            //stats = new PersistedStats(5, 5);
            this.name = "Turret";
            this.rank = rank;

            this.facing = facing;
            this.movementDirection = (int)facing;
            this.aimDirection = (int)facing;

            timers.Add("shot", new Timer(shotTimer * rank));

            targetable = true;
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
                //timers["shot"].setInterval(shotTimer);
                //timers["shot"].resetAndStart();
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
