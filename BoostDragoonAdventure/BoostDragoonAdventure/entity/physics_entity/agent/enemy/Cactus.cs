﻿using Com.Brashmonkey.Spriter.player;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using wickedcrush.map.path;
using wickedcrush.utility;
using wickedcrush.display._3d;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Cactus : Attack
    {
        Stack<PathNode> patrol;
        //private float speed = 60f;
        private int navigationResetLength = 500, spikeAnimationTimer = 500, deathTimer = 1000;
        int rank = 0;
        float angle = 0f;

        public Cactus(int id, World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound, Stack<PathNode> patrol, int damage, int force, int rank)
            : base(id, w, pos, size, center, false, damage, force, sound, factory)
        {
            this.rank = rank;
            Initialize();
            
            //this.stats = stats;
            this.patrol = patrol;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "Cactus";

            timers.Add("navigation", new Timer(navigationResetLength));
            timers["navigation"].start();

            timers.Add("spike_animation", new Timer(spikeAnimationTimer));

            timers.Add("death_timer", new Timer(deathTimer + random.Next(100)));

            //timers.Add("falling", new Timer(500));

            startingFriction = 0.5f;
            stoppingFriction = 0.2f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            this.airborne = true;
            this.immortal = false;

            activeRange = 300f;
            this.speed = 100f;

            if (rank == 1)
            {
                killValue = 200;
            }
            else if (rank == 2)
            {
                killValue = 300;
            }

            SetupStateMachine();

            AddLight(new PointLightStruct(new Vector4(0.1f, 0.85f, 0.05f, 1f), 0.2f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 30f));

            InitializeCactusSprites(1f, 1f, 1f, 1f, 1f, 1f);

            
            targetBobAmount = 0.2f;
            timers["bob"].setInterval(500 + random.Next(500));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (remove)
            {
                return;
            }

            //bodySpriter.setAngle(angle);
            //angle += gameTime.ElapsedGameTime.Milliseconds;

            //UpdateHpBar();
            //UpdateAnimation();
            //facePosition(factory._game.playerManager.getMeanPlayerPos());

            if(timers["spike_animation"].isActive())
            {
                bobAmount = 1f;
                //bodySpriter.setAnimation("spikey", 0, 0);
            } else if(timers["spike_animation"].isDone())
            {
                //bodySpriter.setAnimation("plain", 0, 0);
                timers["spike_animation"].reset();
            }

            if (timers["death_timer"].isActive())
            {
                ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height + 20, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.3f, 0), 0f, 0f, 500, 17, "particles", 0, "white_to_red");
                particleEmitter.EmitParticles(ps, this.factory, 3);

                targetLight.PointLightRange = 100f;
                targetLight.DiffuseIntensity = 1f;
                targetLight.SpecularIntensity = 1f;
                light.SpecularColor = new Vector4(1f, .25f, 0f, 1f);
                light.DiffuseColor = new Vector4(1f, 0f, .25f, 1f);

                if(timers["death_timer"].isDone())
                {
                    this.Die();
                }
            }

            movementDirection = (int)facing;
            aimDirection = (int)Math.Abs(Math.Sin(timers["bob"].getPercent() * Math.PI) * 90) + 45;

        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
        }

        protected override void Die()
        {
            this.dead = true;

            targetBobAmount = 1f;
            timers["bob"].setInterval(300);
            
            if (timers["death_timer"].isDone())
            {
                if (rank == 2)
                {
                    useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f),
                            0, 16, 16, 0, 337, false, 50f, 500, 0, 1f,
                            new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 100, 10, "particles", 0, "white_to_green")),
                            "", 0, "",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(100f, 0f), -10, 50, 5000, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, false),
                            true));
                }
                else if (rank == 1)
                {
                    useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f),
                            0, 8, 8, 0, 315, false, 50f, 500, 0, 1f,
                            new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 100, 10, "particles", 0, "white_to_green")),
                            "", 0, "",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(100f, 0f), -10, 50, 5000, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, false),
                            true));
                }

                //factory.RegisterDeath(id);

                base.Die();
            }
            else if (!timers["death_timer"].isActive())
            {
                timers["death_timer"].resetAndStart();
            }

            

            
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("patrol",
                new State("patrol",
                    c => true,
                    c =>
                    {
                        if (path == null || path.Count == 0)
                        {
                            path = new Stack<PathNode>(patrol);
                        }

                        //this.speed = walkSpeed;

                        FollowPath(false);
                    }));

            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);
            drawBody = false;

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;
            
            //height = 10;
            

        }

        protected void InitializeCactusSprites(float torsoScale, float armWidth, float legWidth, float armStance, float legStance, float spineStance)
        {
            float tempHeight = 0;
            float sScale = size.X / 24f;

            tempHeight += 10f;
            AddAngledElement("A", "shapes", "green", 0, new Vector3(0f, tempHeight, 0f) * sScale, 0, 1.45f * sScale, 0f, new Vector3(-0.25f, 0f, 0.25f) * sScale, 0);

            AddAngledElement("nub5", "shapes", "green", 1, new Vector3(2f, tempHeight, 16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.35f, 1f, 0.35f) * sScale, 0);
            AddAngledElement("nub6", "shapes", "green", 1, new Vector3(-2f, tempHeight, -16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.35f, 1f, 0.35f) * sScale, 0);
            
            tempHeight += 25f;

            AddAngledElement("nub1", "shapes", "green", 1, new Vector3(2f, tempHeight, 16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.25f, 1f, 0.25f) * sScale, 0);
            AddAngledElement("nub2", "shapes", "green", 1, new Vector3(-2f, tempHeight, -16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.25f, 1f, 0.25f) * sScale, 0);

            AddAngledElement("B", "shapes", "green", 0, new Vector3(5f, tempHeight, 0f) * sScale, 0, 1.25f * sScale, 0f, new Vector3(-5.5f, 0f, 5.5f) * sScale, 0);
            tempHeight += 20f;

            AddAngledElement("nub3", "shapes", "green", 1, new Vector3(2f, tempHeight, 16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.45f, 1f, 0.45f) * sScale, 0);
            AddAngledElement("nub4", "shapes", "green", 1, new Vector3(-2f, tempHeight, -16f) * sScale, 0, 1f * sScale, 0f, new Vector3(-0.45f, 1f, 0.45f) * sScale, 0);

            AddAngledElement("C", "shapes", "green", 0, new Vector3(-10f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-7f, 0f, 7f) * sScale, 0);
            tempHeight += 25f;
            AddAngledElement("D", "shapes", "green", 0, new Vector3(0f, tempHeight, -0.5f) * sScale, 0, 2.5f * sScale, 0f, new Vector3(-12f, 0f, 12f) * sScale, 0);
            
            tempHeight += 25f;
            if (rank == 1)
            {
                AddAngledElement("E", "shapes", "red", 0, new Vector3(1f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-5f, 0f, 5f) * sScale, 0);
            } else if (rank == 2)
            {
                AddAngledElement("E", "shapes", "red", 0, new Vector3(-6f, tempHeight, -3f) * sScale, 0, 0.75f * sScale, 0f, new Vector3(-3f, 2f, -3f) * sScale, 0);
                AddAngledElement("F", "shapes", "red", 0, new Vector3(6f, tempHeight, -3f) * sScale, 0, 0.75f * sScale, 0f, new Vector3(3f, 2.5f, -3f) * sScale, 0);
                AddAngledElement("G", "shapes", "red", 0, new Vector3(0f, tempHeight, 3f) * sScale, 0, 0.75f * sScale, 0f, new Vector3(0f, 1.5f, 3f) * sScale, 0);

            }
            //AddAngledElement("F", "shapes", "green", 0, new Vector3(0f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-7f, 0f, 7f) * sScale);
            //tempHeight += 10f;
            //AddAngledElement("G", "shapes", "green", 0, new Vector3(0f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-8f, 0f, 8f) * sScale);
            //tempHeight += 10f;
            //AddAngledElement("H", "shapes", "green", 0, new Vector3(0f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-7f, 0f, 7f) * sScale);
            //tempHeight += 10f;
            //AddAngledElement("I", "shapes", "green", 0, new Vector3(0f, tempHeight, 0f) * sScale, 0, 1f * sScale, 0f, new Vector3(-8f, 0f, 8f) * sScale);
            tempHeight -= 146f;
            AddAngledElement("Face", "cactus", "love", 1, new Vector3(1.5f, tempHeight, 0f) * sScale, 0, 0.6f * sScale, 0f, new Vector3(-13f, 1f, 13f) * sScale, 0);


            //aimDirection = (int)(random.NextDouble() * 360);
        }

        protected override void HandleCollisions()
        {
            if (dead)
            {
                return;
            }

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                

                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !(c.Other.UserData is Cactus)
                    && !((Agent)c.Other.UserData).noCollision)
                {
                    //factory.DisplayMessage("!");
                    ((Agent)c.Other.UserData).TakeHit(this, true);
                    timers["spike_animation"].resetAndStart();

                    if (rank == 2)
                    {
                        stats.set("hp", 0);
                        Die();

                    }
                }

                //if (c.Other.UserData is Chimera || !((Chimera)c.Other.UserData).airborne)
                //{
                    //this.Die();
                //}


                c = c.Next;
            }
        }
    }
}
