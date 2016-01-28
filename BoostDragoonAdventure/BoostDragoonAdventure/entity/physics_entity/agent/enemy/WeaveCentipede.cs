using System;
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
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d;
using wickedcrush.behavior.state;
using wickedcrush.particle;
using wickedcrush.behavior;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.helper;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.inventory;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class WeaveCentipede : Agent
    {
        public int rank = 2;

        private int tellLength = 2000, counterAttackLength = 500;
        private StateName state;
        private bool bounce = false;

        Vector2 startingPoint;

        bool wallCollision = false;

        float initialVelocity = 200f, velocity = 0f;

        int counterShots = 0;

        


        public WeaveCentipede(World w, Vector2 pos, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, new Vector2(200f, 200f), new Vector2(100f, 100f), true, factory, sound)
        {
            
            this.stats = stats;
            Initialize();
            
        }

        private void Initialize()
        {
            this.name = "Weave Centipede";
            
            startingPoint = pos;

            movementDirection = 90;
            facing = Direction.South;

            //timers.Add("done_moving", new Timer(moveLength));
            //timers.Add("done_standing", new Timer(standLength));
            addTimer("counter_attack", counterAttackLength);
            addTimer("done_tell", tellLength);

            timers["done_tell"].reset();
            timers["counter_attack"].reset();
            //timers.Add("done_attack", new Timer(attackLength));

            startingFriction = 0.7f;
            stoppingFriction = 0.001f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            state = StateName.Moving;
            //timers["done_moving"].resetAndStart();

            //timers["done_standing"].resetAndStart();

            activeRange = 1000f;
            this.speed = 70f;

            SetupStateMachine();

            InitializeHpBar();
            UpdateHpBar();

            light = new PointLightStruct(new Vector4(1f, 0.85f, 0.85f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 1f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 1000f);
            factory._gm.scene.AddLight(light);

            targetable = true;
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> counterAttackBranch = new Dictionary<String, State>();

            counterAttackBranch.Add("counter_attack",
                new State("counter_attack",
                c => true,
                c =>
                {
                    setTargetToClosestPlayer(true, 360);

                    int phase = 1;
                    if ((((float)this.stats.getNumber("hp")) / ((float)this.stats.getNumber("MaxHP"))) < 0.5f)
                    {
                        phase = 2;
                    }


                    int counterShotLimit = 3;

                    if (phase == 2)
                        counterShotLimit = 6;

                    if (timers["counter_attack"].isDone())
                    {
                        timers["counter_attack"].resetAndStart();
                        
                        counterShots++;

                        if (target != null)
                        {
                            if (phase == 2 && counterShots % 2 == 1)
                            {
                                faceTarget();
                                if (rank == 2)
                                {
                                    this.aimDirection += 45;
                                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(220f, 0f), 1, 4, 4, 1, 270, false, 600f, 800, 800, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));


                                    this.aimDirection -= 90;
                                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(220f, 0f), 1, 4, 4, 1, 270, false, 600f, 800, 800, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));

                                } else if(rank == 1)
                                {
                                    //this.aimDirection += 5;
                                    //this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 100, 4, 1, 0, 1, false, 300f, 300, 300, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    //SkillServer.GenerateProjectile(new Vector2(5f, 5f), new Vector2(200, 0), -25, 100, 1800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));

                                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 0, 5, 5, 0, 30, false, 50f, 0, 0, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(100, 0), -25, 100, 5300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));

                                    //this.aimDirection -= 10;
                                    //this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 200, 8, 1, 0, 5, false, 300f, 300, 800, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    //SkillServer.GenerateProjectile(new Vector2(5f, 5f), new Vector2(200, 0), -25, 100, 2300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                                }
                            }
                            else
                            {
                                faceTarget();
                                if (rank == 1)
                                {
                                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 0, 3, 3, 0, 30, false, 50f, 0, 0, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(100, 0), -25, 100, 5300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));

                                } else if(rank == 2)
                                {
                                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(220f, 0f), 1, 4, 4, 1, 270, false, 600f, 800, 800, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                                    SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));


                                }
                            }
                        }

                        if (counterShots > counterShotLimit)
                        {
                            this.state = StateName.Moving;
                            movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                            MoveForward(false, velocity);
                        }
                    }

                    if (!timers["counter_attack"].isActive())
                    {
                        timers["counter_attack"].resetAndStart();
                    }
                }));

            Dictionary<String, State> staggeredBranch = new Dictionary<String, State>();

            staggeredBranch.Add("staggered",
                new State("staggered",
                    c => true,
                    c =>
                    {
                        if (!this.staggered)
                        {
                            velocity = initialVelocity;
                            movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                            MoveForward(false, velocity);
                            state = StateName.CounterAttack;
                            counterShots = 0;
                            timers["counter_attack"].resetAndStart();
                        }

                        //this.height = (int)MathHelper.Lerp(0, staggerHeight, (float)Math.Sin(((double)stats.get("stagger") / (double)stats.get("staggerDuration")) * Math.PI));

                        float amt = (float)(((float)stats.getNumber("stagger")) / ((float)stats.getNumber("staggerDuration")));

                        //2 bounce
                        if (amt > 0.333f)
                        {
                            this.height = (int)MathHelper.Lerp(0, staggerHeight, (float)Math.Sin(((amt - 0.333f) * (3f / 2f)) * (Math.PI / 1.0)));
                        }
                        else
                        {
                            this.height = (int)MathHelper.Lerp(0, staggerHeight / 3, (float)Math.Sin(((amt) * 3f) * (Math.PI / 1.0)));
                        }
                        
                        timers["done_tell"].reset();
                    }));


            Dictionary<String, State> mainBranch = new Dictionary<String, State>();

            mainBranch.Add("staggered_main_branch",
                new State("staggered_main_branch",
                    c => this.staggered,
                    c =>
                    {
                        state = StateName.Staggered;
                        timers["done_tell"].reset();
                    }));

            mainBranch.Add("attack_tell",
                new State("attack_tell",
                    c => timers["done_tell"].isActive(),
                    c =>
                    {
                        state = StateName.Tell;
                    }));

            mainBranch.Add("begin_attack_tell",
                new State("begin_attack_tell",
                    c => this.distanceToPosition(startingPoint) < 30 && state == StateName.Moving,
                    c =>
                    {
                        if(!timers["done_tell"].isActive())
                            timers["done_tell"].resetAndStart();
                        state = StateName.Tell;
                    }));

            
            mainBranch.Add("at_start_pos",
                new State("at_start_pos",
                    c => timers["done_tell"].isDone() && state == StateName.Tell,
                    c =>
                    {
                        if (target == null)
                        {
                            setTargetToClosestPlayer(true, 360);
                            state = StateName.Tell;
                        }
                        else
                        {
                            faceTarget();
                            velocity = initialVelocity;
                            movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                            MoveForward(false, velocity);
                            //Strafe(velocity);
                            state = StateName.Attacking;
                            timers["done_tell"].reset();
                        }


                    }));

            mainBranch.Add("away_from_start_pos",
                new State("away_from_start_pos",
                    c => this.velocity <= 0f && state == StateName.Attacking,
                    c =>
                    {
                        velocity = initialVelocity;
                        movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                        MoveForward(false, velocity);
                        state = StateName.Moving;


                    }));

            mainBranch.Add("in_transit",
                new State("in_transit",
                    c => state == StateName.Attacking,
                    c =>
                    {
                        MoveForward(false, velocity);
                        //faceTarget();
                        //movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                        //Strafe(velocity);
                        velocity -= (((float)this.elapsed.TotalMilliseconds) / 10f);
                    }));

            mainBranch.Add("in_return",
                new State("in_return",
                    c => state == StateName.Moving,
                    c =>
                    {
                        MoveForward(false, velocity);
                        state = StateName.Moving;
                    }));
            

            
            stateTree = new StateTree();

            stateTree.AddBranch("counter_attack", new StateBranch(c => state == StateName.CounterAttack, counterAttackBranch));
            stateTree.AddBranch("staggered", new StateBranch(c => state == StateName.Staggered, staggeredBranch));
            stateTree.AddBranch("default", new StateBranch(c => true, mainBranch));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (remove)
                return;

            UpdateHpBar();
            UpdateAnimation();

        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();

            sPlayers.Add("monster_a", new SpriterPlayer(factory._spriterManager.spriters["monster_a"].getSpriterData(), 0, factory._spriterManager.spriters["monster_a"].loader));

            bodySpriter = sPlayers["monster_a"];
            bodySpriter.setFrameSpeed(20);

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;

        }

        protected void UpdateAnimation()
        {


            switch (state)
            {
                case StateName.Attacking:
                    bodySpriter.setAnimation("attack", 0, 0);
                    break;
                case StateName.Moving:
                    bodySpriter.setAnimation("moving", 0, 0);
                    break;
                case StateName.Standing:
                    bodySpriter.setAnimation("still", 0, 0);
                    break;
                case StateName.Tell:
                    bodySpriter.setAnimation("tell", 0, 0);
                    break;
                case StateName.CounterAttack:
                    bodySpriter.setAnimation("attack", 0, 0);
                    break;
            }


            
        }

        protected override void HandleCollisions()
        {
            if (this.remove)
                return;

            bool tempWallCollision = false;

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && !this.remove
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !((Agent)c.Other.UserData).remove
                    && !((Agent)c.Other.UserData).noCollision
                    && !c.Other.UserData.Equals(this.parent))
                {
                    if (state == StateName.Attacking)
                        ((Agent)c.Other.UserData).TakeKnockback(this.pos + this.center, 3, 100);


                    //((Agent)c.Other.UserData).TakeSkill(this);
                    //hitConnected = true;

                    //if (!piercing)
                        //Remove();
                }
                else if (!wallCollision && !tempWallCollision && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    tempWallCollision = true;
                    if (bounce)
                    {
                        Vector2 movementVector = Helper.GetDirectionVectorFromDegrees(movementDirection);
                        movementVector = -2f * Vector2.Dot(movementVector, c.Contact.Manifold.LocalNormal) * c.Contact.Manifold.LocalNormal + movementVector;

                        movementDirection = (int)Helper.GetDegreesFromVector(movementVector);
                        aimDirection = (int)Helper.GetDegreesFromVector(movementVector);

                        Vector2 velocity = new Vector2((float)(movementVector.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + movementVector.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                            (float)(movementVector.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - movementVector.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));
                        _sound.playCue("ready ping");

                        ParticleStruct ps = ParticleServer.GenerateSpark(new Vector3(pos.X + center.X, height, pos.Y + center.Y), movementVector * 3f);
                        this.EmitParticles(ps, 3);
                    }
                    //else
                        //Remove();
                }

                c = c.Next;
            }

            //if (tempWallCollision)
            wallCollision = tempWallCollision;
        }

        protected override void Die()
        {
            setTargetToPlayer();
            if(this.target is PlayerAgent)
            {
                Item temp = InventoryServer.getRareItem();
                ((PlayerAgent)target).stats.inventory.receiveItem(temp);
                factory.DisplayMessage("You got " + temp.name + "!");
            }
            base.Die();
        }


    }
}
