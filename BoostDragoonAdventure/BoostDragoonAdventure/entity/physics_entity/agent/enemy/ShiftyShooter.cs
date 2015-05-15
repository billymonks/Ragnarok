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

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class ShiftyShooter : Agent
    {
        

        private const int moveLength = 750, standLength = 1000, standToShootLength = 500, attackRange = 700;

        private StateName state;

        public ShiftyShooter(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound)
            : base(w, pos, size, center, solid, factory, sound)
        {
            Initialize();
            this.stats = stats;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "ShiftyShooter";

            //timers.Add("navigation", new Timer(navigationResetLength));
            //timers["navigation"].start();

            movementDirection = 0;
            facing = (Direction)0;

            timers.Add("done_moving", new Timer(moveLength));
            timers.Add("done_standing", new Timer(standLength));
            timers.Add("shoot", new Timer(standToShootLength));

            startingFriction = 0.3f;
            stoppingFriction = 0.1f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            state = StateName.Moving;
            timers["done_moving"].resetAndStart();

            //timers["done_standing"].resetAndStart();

            activeRange = 300f;
            this.speed = 70f;

            SetupStateMachine();

            InitializeHpBar();
            UpdateHpBar();
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            
            ctrl.Add("standing",
                new State("standing",
                    c => state==StateName.Standing,
                    c =>
                    {
                        if (null == stateTree.previousControlState || !stateTree.previousControlState.name.Equals("standing"))
                        {
                            //factory.addActionSkill(SkillServer.GenerateSkillStruct(20, 0, 70, 8, 8, 0, 45, true), this);
                            //shoot in direction indicated

                            //decide direction to move
                            c.movementDirection += 90;
                            

                            if (target == null)
                                setTargetToClosestPlayer();
                            else if (distanceToTarget() < attackRange)
                            {
                                faceTarget();
                                
                            }
                            //start timer to change
                            faceTarget();
                        }





                        //if (target == null)
                            //setTargetToClosestPlayer();
                        //else if (distanceToTarget() < attackRange)
                        //{
                            
                            //timers["attack_tell"].resetAndStart();
                            //_sound.setGlobalVariable("InCombat", 1f);
                        //}
                        //else
                        //{
                            //_sound.setGlobalVariable("InCombat", 1f);
                        //}

                    }));
            ctrl.Add("moving",
                new State("moving",
                    c => state == StateName.Moving,
                    c =>
                    {
                        if (null==stateTree.previousControlState || !stateTree.previousControlState.name.Equals("standing"))
                        {
                            //timers["done_moving"].resetAndStart();
                        }
                        MoveForward(false, 70f);
                        faceTarget();
                        //faceTarget();
                    }
                    ));

            
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateHpBar();
            UpdateAnimation();

            if (timers["done_moving"].isDone())
            {
                timers["done_moving"].reset();
                state = StateName.Standing;
                timers["done_standing"].resetAndStart();
                if (target != null)
                {
                    timers["shoot"].resetAndStart();
                }
            }

            if (timers["done_standing"].isDone())
            {
                timers["done_standing"].reset();
                state = StateName.Moving;
                timers["done_moving"].resetAndStart();
            }

            if (timers["shoot"].isDone())
            {
                timers["shoot"].reset();
                //_sound.playCue("whsh", emitter);
                //fireAimedProjectile(Helper.degreeConversion(angleToEntity(target)));
                factory.addActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), 0, 100, 16, 8, 0, 30, true, 600f, 1200), this, null, Helper.degreeConversion(angleToEntity(target)));
            }
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);

        }

        protected void UpdateAnimation()
        {


            
            String bad = "";

            if (timers["falling"].isActive())
            {
                bodySpriter.setAnimation("fall_000", 0, 0);
                return;
            }

            faceTarget();
            facing = Helper.constrainDirection(facing);

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

            //bad = "south_stand_000";

            bodySpriter.setAnimation(bad, 0, 0);
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            //if (navigator != null)
            //{
            //navigator.DebugDraw(wTex, gd, spriteBatch, f);
            //}

            //spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            //spriteBatch.Draw(aTex, pos + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);

            DrawName(spriteBatch, f, camera);

            DebugDrawHealth(wTex, aTex, gd, spriteBatch, f, c, camera);
        }
    }
}
