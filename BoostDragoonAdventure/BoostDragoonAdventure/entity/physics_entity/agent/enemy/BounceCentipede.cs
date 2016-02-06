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
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.inventory;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class BounceCentipede : Agent
    {

        private int standLength = 300, moveLength = 1000, tellLength = 300, attackLength = 500;
        private StateName state;
        private bool bounce = false;

        Vector2 startingPoint;

        bool wallCollision = false;

        public BounceCentipede(World w, Vector2 pos, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(-1, w, pos, new Vector2(200f, 200f), new Vector2(100f, 100f), true, factory, sound)
        {
            
            this.stats = stats;
            Initialize();
        }

        private void Initialize()
        {
            this.name = "Razor Centipede";
            
            startingPoint = pos;

            movementDirection = 90;
            facing = Direction.South;

            timers.Add("done_moving", new Timer(moveLength));
            timers.Add("done_standing", new Timer(standLength));
            timers.Add("done_tell", new Timer(tellLength));
            timers.Add("done_attack", new Timer(attackLength));

            startingFriction = 0.3f;
            stoppingFriction = 0.1f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            state = StateName.Standing;
            //timers["done_moving"].resetAndStart();

            timers["done_standing"].resetAndStart();

            activeRange = 300f;
            this.speed = 70f;

            killValue = 2500;

            SetupStateMachine();

            //InitializeHpBar();
            //UpdateHpBar();
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            
            ctrl.Add("standing",
                new State("standing",
                    c => state==StateName.Standing,
                    c =>
                    {
                        if (target == null)
                            setTargetToClosestPlayer(true, 360);


                        bounce = false;

                    }));
            ctrl.Add("moving",
                new State("moving",
                    c => state == StateName.Moving,
                    c =>
                    {
                        if (null == stateTree.previousControlState || !stateTree.previousControlState.name.Equals("moving"))
                        {
                            //timers["done_moving"].resetAndStart();
                        }
                        if (target == null)
                        {
                            setTargetToClosestPlayer(true, 360);
                        }
                        else
                        {
                            movementDirection = (int)MathHelper.ToDegrees(angleToPos(startingPoint));
                        }
                        MoveForward(false, 70f);
                        bounce = false;
                    }
                    ));
            ctrl.Add("tell",
                new State("tell",
                    c => state == StateName.Tell,
                    c =>
                    {
                        if (null == stateTree.previousControlState || !stateTree.previousControlState.name.Equals("tell"))
                        {
                            
                        }
                        //MoveForward(false, 70f);
                        if (target == null)
                            setTargetToClosestPlayer(true, 360);
                        else
                        {
                            faceTarget();
                            movementDirection = (int)MathHelper.ToDegrees(angleToEntity(target));
                        }

                        
                        bounce = false;
                        //faceTarget();
                    }
                    ));

            ctrl.Add("attacking",
                new State("attacking",
                    c => state == StateName.Attacking,
                    c =>
                    {
                        if (null == stateTree.previousControlState || !stateTree.previousControlState.name.Equals("attacking"))
                        {
                            
                        }
                        MoveForward(false, 170f);
                        //bounce = true;

                    }
                    ));

            
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (remove)
                return;

            //UpdateHpBar();
            UpdateAnimation();

            if (timers["done_standing"].isDone())
            {
                timers["done_standing"].reset();
                state = StateName.Moving;
                timers["done_moving"].resetAndStart();
            }

            if (timers["done_moving"].isDone())
            {
                timers["done_moving"].reset();
                state = StateName.Tell;
                timers["done_tell"].resetAndStart();
            }

            if (timers["done_tell"].isDone())
            {
                timers["done_tell"].reset();
                state = StateName.Attacking;
                timers["done_attack"].resetAndStart();
            }

            if (timers["done_attack"].isDone())
            {
                timers["done_attack"].reset();
                state = StateName.Standing;
                timers["done_standing"].resetAndStart();
            }

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
                    //if (c.Other.UserData is Cactus)
                    //{
                        //((Cactus)c.Other.UserData).stats.set("hp", 0);
                    //}

                    if (this.parent != null
                        && ((Agent)c.Other.UserData).parent != null
                        && ((Agent)c.Other.UserData).parent.Equals(this.parent))
                        break;


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
            if (this.target is PlayerAgent)
            {
                Item temp = InventoryServer.getRareItem();
                ((PlayerAgent)target).stats.inventory.receiveItem(temp);
                factory.DisplayMessage("You got " + temp.name + "!");
            }
            base.Die();
        }


    }
}
