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
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d;
using wickedcrush.behavior.state;
using wickedcrush.particle;
using wickedcrush.behavior;
using wickedcrush.entity.physics_entity.agent.action;
using wickedcrush.helper;
using wickedcrush.entity.physics_entity.agent.attack;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Chimera : Agent
    {

        private int tellLength = 2000, counterAttackLength = 1000, jumpTellLength = 1500, counterAttackShotLength = 200;
        private StateName state;
        private bool bounce = false;

        Vector2 startingPoint;

        bool wallCollision = false;

        float initialVelocity = 200f, velocity = 0f;

        int counterShots = 0;

        int instaJumps = 0, instaJumpCount = 0;




        public Chimera(World w, Vector2 pos, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, new Vector2(200f, 200f), new Vector2(100f, 100f), false, factory, sound)
        {
            
            this.stats = stats;
            Initialize();
            
        }

        private void Initialize()
        {
            this.name = "Chimera";
            
            startingPoint = pos;

            movementDirection = 90;
            facing = Direction.South;

            //timers.Add("done_moving", new Timer(moveLength));
            //timers.Add("done_standing", new Timer(standLength));
            addTimer("counter_attack", counterAttackLength);
            addTimer("counter_attack_shots", counterAttackShotLength);
            addTimer("done_tell", tellLength);
            addTimer("jump_tell", jumpTellLength);

            
            timers["counter_attack"].reset();
            timers["counter_attack_shots"].reset();
            timers["done_tell"].reset();
            timers["jump_tell"].reset();
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

            targetable = true;

            //subEntityList.Add("status", new TextEntity(state.ToString(), pos, _sound, factory._game, -1, factory, 2f, 2f, 0f));
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> counterAttackBranch = new Dictionary<String, State>();

            counterAttackBranch.Add("counterattack_over", new State("counterattack_over",
                c => timers["counter_attack"].isDone(),
                c =>
                {
                    state = StateName.Standing;
                    timers["counter_attack_shots"].reset();
                }));

            counterAttackBranch.Add("counterattack", new State("counterattack",
                c => true,
                c =>
                {
                    if (inJump)
                        EndJump();

                    faceTarget();

                    if (timers["counter_attack_shots"].isDone())
                    {
                        useActionSkill(SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -5, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, false));
                    }

                    if (!timers["counter_attack_shots"].isActive())
                    {
                        timers["counter_attack_shots"].resetAndStart();
                    }


                    //if (target.distanceToPosition(initialPosition) < 350)
                    if(distanceToTarget() < 250)
                        Strafe(400f);
                    else
                    {
                        //facePosition(initialPosition);
                        MoveForward(false, 400f);
                    }
                }));

            Dictionary<String, State> staggeredBranch = new Dictionary<String, State>();

            staggeredBranch.Add("staggered",
                new State("staggered",
                    c => true,
                    c =>
                    {
                        if(inJump)
                            EndJump();

                        if (!this.staggered)
                        {
                            velocity = initialVelocity;
                            movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                            //MoveForward(false, velocity);
                            state = StateName.CounterAttack;
                            counterShots = 0;
                            timers["counter_attack"].resetAndStart();
                        }

                        //this.height = (int)MathHelper.Lerp(0, staggerHeight, (float)Math.Sin(((double)stats.get("stagger") / (double)stats.get("staggerDuration")) * Math.PI));

                        float amt = (float)(((float)stats.get("stagger")) / ((float)stats.get("staggerDuration")));

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

            Dictionary<String, State> noTarget = new Dictionary<String, State>();
            noTarget.Add("no_target_staggered",
                new State("no_target_staggered",
                    c => this.staggered,
                    c =>
                    {
                        state = StateName.Staggered;
                        timers["done_tell"].reset();
                    }));

            noTarget.Add("no_target",
                new State("no_target",
                    c => true,
                    c =>
                    {
                        state = StateName.Standing;
                        setTargetToClosestPlayer(true, 360);
                    }));

            Dictionary<String, State> jumpingBranch = new Dictionary<String, State>();
            jumpingBranch.Add("jumping", new State("jumping",
                c => this.inJump,
                c =>
                {

                }));

            jumpingBranch.Add("end_jump", new State("end_jump",
                c => true,
                c =>
                {


                    state = StateName.Landing;

                    if (instaJumpCount == instaJumps)
                    {
                        timers["jump_tell"].resetAndStart();
                    }
                    else
                    {
                        timers["jump_tell"].reset();
                        instaJumpCount++;

                        if (instaJumpCount == 1)
                        {
                            this.StartJump(300, 900, target.pos + target.center);
                            //fireTrackingProjectile(0, target);
                            //fireTrackingProjectile(90, target);
                            //fireTrackingProjectile(180, target);
                            //fireTrackingProjectile(270, target);
                        }
                        else if (instaJumpCount == 2)
                        {
                            this.StartJump(300, 800, target.pos + target.center);
                            //fireTrackingProjectile(0, target);
                            //fireTrackingProjectile(90, target);
                            //fireTrackingProjectile(180, target);
                            //fireTrackingProjectile(270, target);
                        }
                    }
                }));

            Dictionary<String, State> behaviorOne = new Dictionary<String, State>();

            behaviorOne.Add("staggered",
                new State("staggered",
                    c => this.staggered,
                    c =>
                    {
                        state = StateName.Staggered;
                        timers["done_tell"].reset();
                    }));

            behaviorOne.Add("landing",
                new State("landing",
                    c => state == StateName.Landing,
                    c =>
                    {
                        if (!inJump)
                        {
                            factory._gm.camera.ShakeScreen(5f);
                            state = StateName.Standing;
                        }
                        else
                        {
                            state = StateName.Jumping;
                        }
                        //timers["done_tell"].reset();
                    }));

            behaviorOne.Add("begin_jump",
                new State("begin_jump",
                    c => timers["jump_tell"].isDone(),
                    c =>
                    {
                        int phase = 1;
                        if ((((float)this.stats.get("hp")) / ((float)this.stats.get("MaxHP"))) < 0.666f)
                        {
                            phase = 2;
                        }
                        if ((((float)this.stats.get("hp")) / ((float)this.stats.get("MaxHP"))) < 0.333f)
                        {
                            phase = 3;
                        }

                        instaJumpCount = 0;

                        if (phase == 1)
                        {
                            instaJumps = 0;
                            this.StartJump(300, 1000, target.pos + target.center);
                            fireTrackingProjectile(120, target);
                            fireTrackingProjectile(0, target);
                            fireTrackingProjectile(240, target);
                        }
                        else if (phase == 2)
                        {
                            instaJumps = 1;
                            this.StartJump(300, 900, target.pos + target.center);
                            fireTrackingProjectile(0, target);
                            fireTrackingProjectile(90, target);
                            fireTrackingProjectile(180, target);
                            fireTrackingProjectile(270, target);
                        }
                        else
                        {
                            instaJumps = 2;
                            this.StartJump(300, 800, target.pos + target.center);
                            fireTrackingProjectile(0, target);
                            fireTrackingProjectile(60, target);
                            fireTrackingProjectile(120, target);
                            fireTrackingProjectile(180, target);
                            fireTrackingProjectile(240, target);
                            fireTrackingProjectile(300, target);
                        }
                        this.state = StateName.Jumping;
                    }));

            behaviorOne.Add("begin_jump_tell",
                new State("begin_jump_tell",
                    c => !timers["jump_tell"].isActive(),
                    c =>
                    {
                        timers["jump_tell"].resetAndStart();
                        this.state = StateName.Tell;
                    }));

            behaviorOne.Add("jump_tell",
                new State("jump_tell",
                    c => true,
                    c =>
                    {
                        this.state = StateName.Tell;
                    }));

            /*Dictionary<String, State> playerAway = new Dictionary<String, State>();

            playerAway.Add("jump_test",
                new State("jump_test",
                    c => true,
                    c =>
                    {
                        this.StartJump(200, 2000, target.pos + target.center);
                        this.state = StateName.Jumping;
                    }));

            playerAway.Add("staggered_away_branch",
                new State("staggered_away_branch",
                    c => this.staggered,
                    c =>
                    {
                        state = StateName.Staggered;
                        timers["done_tell"].reset();
                    }));

            playerAway.Add("move_to_center",
                new State("move_to_center",
                    c => distanceToPosition(startingPoint) > 30,
                    c => {
                        velocity = initialVelocity;
                        movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                        MoveForward(false, velocity);
                        state = StateName.Moving;
                    }));

            playerAway.Add("arrived_at_center",
                new State("arrived_at_center",
                    c => state == StateName.Moving && distanceToPosition(startingPoint) < 30,
                    c =>
                    {
                        state = StateName.Attacking;
                    }));

            playerAway.Add("in_return",
                new State("in_return",
                    c => state == StateName.Moving,
                    c =>
                    {
                        MoveForward(false, velocity);
                        state = StateName.Moving;
                    }));

            playerAway.Add("attacking_away",
                new State("attacking_away",
                    c => distanceToPosition(startingPoint) <= 30,
                    c =>
                    {
                        state = StateName.Attacking;

                        faceTarget();

                        if (timers["counter_attack"].isDone())
                        {
                            timers["counter_attack"].resetAndStart();

                            useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(600f, 0f), 35, 16, 16, 0, 340, false, 600f, 1000, 1800, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(1f, 1f), new Vector2(300, 0), -20, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        }

                        if (!timers["counter_attack"].isActive())
                        {
                            timers["counter_attack"].resetAndStart();
                        }

                        
                    }));
            Dictionary<String, State> playerCenter = new Dictionary<String, State>();

            playerCenter.Add("jump_test",
                new State("jump_test",
                    c => true,
                    c =>
                    {
                        this.StartJump(200, 2000, target.pos + target.center);
                        this.state = StateName.Jumping;
                    }));

            playerCenter.Add("staggered_center_branch",
                new State("staggered_center_branch",
                    c => this.staggered,
                    c =>
                    {
                        state = StateName.Staggered;
                        timers["done_tell"].reset();
                    }));

            

            playerCenter.Add("get_closer",
                new State("get_closer",
                    c => state != StateName.Moving && distanceToTarget() > 120,
                    c =>
                    {
                        faceTarget();
                        movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                        velocity = initialVelocity;
                        //Strafe(velocity);
                        MoveForward(false, velocity);
                        state = StateName.Tell;
                    }));

            playerCenter.Add("start_spin_attack",
                new State("start_spin_attack",
                    c => distanceToTarget() < 150,
                    c =>
                    {
                        faceTarget();
                        movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                        velocity = 800f;
                        Strafe(velocity);
                        state = StateName.Moving;
                    }));

            playerCenter.Add("cancel_spin",
                new State("cancel_spin",
                    c => state == StateName.Attacking && distanceToTarget() > 300,
                    c =>
                    {
                        state = StateName.Tell;
                    }));

            playerCenter.Add("spin_attack",
                new State("spin_attack",
                    c => state == StateName.Moving,
                    c =>
                    {
                        faceTarget();
                        aimDirection += 180;
                        movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                        velocity = 800f;
                        Strafe(velocity);
                        state = StateName.Moving;

                        if (timers["counter_attack"].isDone())
                        {
                            timers["counter_attack"].resetAndStart();

                            useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 100, 5, 5, 0, 180, true, 300f, 100, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, ((float)random.NextDouble() * 200f) - 100f), -5, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, false), false));
                        }

                        if (!timers["counter_attack"].isActive())
                        {
                            timers["counter_attack"].resetAndStart();
                        }
                    }));

            playerCenter.Add("default_center",
                new State("default_center",
                    c => true,
                    c =>
                    {
                        state = StateName.Tell;
                    }));*/

            

            

            stateTree = new StateTree();

            stateTree.AddBranch("counterAttack", new StateBranch(c => state == StateName.CounterAttack, counterAttackBranch));
            stateTree.AddBranch("staggered", new StateBranch(c => state == StateName.Staggered, staggeredBranch));
            stateTree.AddBranch("no_target", new StateBranch(c => target == null, noTarget));
            stateTree.AddBranch("jumping_branch", new StateBranch(c => state == StateName.Jumping, jumpingBranch));
            stateTree.AddBranch("behavior_one", new StateBranch(c => true, behaviorOne));
            //stateTree.AddBranch("player_center", new StateBranch(c => target.distanceToPosition(initialPosition) < 350, playerCenter));
            //stateTree.AddBranch("player_away", new StateBranch(c => target.distanceToPosition(initialPosition) > 350, playerAway));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (remove)
                return;

            UpdateHpBar();
            UpdateAnimation();

            //((TextEntity)subEntityList["status"]).text = state.ToString();
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
                case StateName.Jumping:
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
                    /*if (state == StateName.Attacking || state == StateName.CounterAttack)
                        ((Agent)c.Other.UserData).TakeKnockback(this.pos + this.center, 3, 100);

                    if (state == StateName.Standing)
                        ((Agent)c.Other.UserData).TakeKnockback(this.pos + this.center, 100);*/

                    if (state == StateName.Landing)
                        ((Agent)c.Other.UserData).TakeKnockback(this.pos + this.center, 30, 100);


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


    }
}