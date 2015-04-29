﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.stats;
using wickedcrush.manager.audio;
using wickedcrush.utility;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    class Giant : Agent
    {
        private const int attackTellLength = 700, postAttackLength = 1000, navigationResetLength = 500, attackRange = 60;

        public Giant(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, size, center, solid, factory, stats, sound)
        {
            Initialize();
            this.stats = stats;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "Murderer";

            timers.Add("navigation", new Timer(navigationResetLength));
            timers["navigation"].start();

            timers.Add("attack_tell", new Timer(attackTellLength));
            timers.Add("post_attack", new Timer(postAttackLength));
            //timers.Add("falling", new Timer(500));

            this.speed = 120;

            activeRange = 300f;

            SetupStateMachine();

            InitializeHpBar();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateHpBar();
            UpdateAnimation();
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            //bodies.Add("activeArea", BodyFactory.CreateBody(w, pos - new Vector2(300f, 300f)));
            //FixtureFactory.AttachCircle(300f, 1f, bodies["activeArea"], center);
            //bodies["activeArea"].IsSensor = true;
            //bodies["activeArea"].BodyType = BodyType.Dynamic;
            //bodies["activeArea"].LinearVelocity = Vector2.Zero;
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("falling",
                new State("falling",
                    c => ((Giant)c).timers["falling"].isActive(),
                    c =>
                    {
                        this.height -= 3;
                    }));
            ctrl.Add("staggered",
                new State("staggered",
                    c => ((Giant)c).staggered,
                    c =>
                    {
                        if (!sm.previousControlState.name.Equals("staggered"))
                        {
                            this.PlayCue("vanquished");
                        }
                        
                        ResetAllTimers();

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("post_attack",
                new State("post_attack",
                    c => ((Giant)c).timers["post_attack"].isActive(),
                    c =>
                    {
                        if (!sm.previousControlState.name.Equals("post_attack"))
                        {
                            this.PlayCue("explosion");
                        }
                        
                        if (timers["post_attack"].isDone())
                        {
                            timers["post_attack"].reset();
                        }

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("attack_tell",
                new State("attack_tell",
                    c => ((Giant)c).timers["attack_tell"].isActive(),
                    c =>
                    {
                        if (!sm.previousControlState.name.Equals("attack_tell"))
                        {
                            faceTarget();
                            ParticleStruct ps = new ParticleStruct(new Vector3(pos.X + center.X - 3f, height + 30, pos.Y + center.Y - 3f), new Vector3(6f, 0f, 6f), new Vector3(-3f, 5f, -3f), new Vector3(6f, 0f, 6f), new Vector3(0f, -0.01f, 0f), 0f, 0f, 500f, "particles", 0, "white_to_yellow");
                            this.EmitParticles(ps, 5);
                            PlayCue("chime");
                        }

                        if (timers["attack_tell"].isDone())
                        {
                            attackForward(new Vector2(96, 96), 40, 1000);
                            timers["post_attack"].resetAndStart();
                            timers["attack_tell"].reset();
                        }

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("chase",
                new State("chase",
                    c => distanceToTarget() > attackRange,
                    c =>
                    {
                        if (!timers["navigation"].isActive())
                        {
                            timers["navigation"].resetAndStart();
                        }

                        if (timers["navigation"].isDone())
                        {
                            createPathToTarget();
                            timers["navigation"].resetAndStart();
                        }

                        FollowPath(false);

                        if (distanceToTarget() < attackRange)
                            timers["attack_tell"].resetAndStart();


                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        if (target == null)
                            setTargetToClosestPlayer();
                        else if (distanceToTarget() < attackRange)
                        {
                            timers["attack_tell"].resetAndStart();
                            _sound.setGlobalVariable("InCombat", 1f);
                        }
                        else
                        {
                            _sound.setGlobalVariable("InCombat", 1f);
                        }

                    }));
            sm = new StateMachine(ctrl);
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            //sPlayers.Add("standing", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 0, factory._spriterManager.spriters["all"].loader));
            //sPlayers.Add("boosting", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 1, factory._spriterManager.spriters["all"].loader));
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            //sPlayer.setAnimation("standing_north", 0, 0);
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);

        }

        protected void UpdateAnimation()
        {
            //if (sPlayer == null)
            //return;

            String bad = "";

            if (timers["falling"].isActive())
            {
                bodySpriter.setAnimation("fall_000", 0, 0);
                return;
            }

            switch (facing)
            {
                case Direction.East:
                    bad += "east";

                    //bodySpriter.setAnimation("east", 0, 0);
                    break;

                case Direction.North:
                    bad += "north";
                    //bodySpriter.setAnimation("north", 0, 0);
                    break;

                case Direction.South:
                    bad += "south";
                    //bodySpriter.setAnimation("south", 0, 0);
                    break;

                case Direction.West:
                    bad += "west";
                    //bodySpriter.setAnimation("west", 0, 0);
                    break;

                case Direction.NorthEast:
                    bad += "northeast";
                    //bodySpriter.setAnimation("northeast", 0, 0);
                    break;

                case Direction.NorthWest:
                    bad += "northwest";
                    //bodySpriter.setAnimation("northwest", 0, 0);
                    break;

                case Direction.SouthEast:
                    bad += "southeast";
                    //bodySpriter.setAnimation("southeast", 0, 0);
                    break;

                case Direction.SouthWest:
                    bad += "southwest";
                    //bodySpriter.setAnimation("southwest", 0, 0);
                    break;
            }

            if (bodies["body"].LinearVelocity.Length() >= 1f)
            {
                bad += "_run_000";
            }
            else
            {
                bad += "_stand_000";
            }

            bodySpriter.setAnimation(bad, 0, 0);
        }
    }
}
