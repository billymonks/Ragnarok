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

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class ShiftyShooter : Agent
    {
        

        private int moveLength = 750, moveVariation = 500, standLength = 1000, standVariation = 500, standToShootLength = 500, attackRange = 700, 
            spreadDuration = 10, blowCount = 4, blowPerSpread = 4, scatterCount = 1, spread = 360, blowDuration = 600, blowReleaseDelay = 500;

        private float blowVelocity = 100f, skillVelocity = 200f, movementSpeed = 70f;

        private bool readyToStart = false;

        private StateName state;

        Vector4 diffuse = new Vector4(0.8f, 0.55f, 0.85f, 1f), hostileDiffuse = new Vector4(1f, 0.35f, 0.35f, 1f);

        public ShiftyShooter(int id, World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound, 
            int spreadDuration, int blowCount, int blowPerSpread, int scatterCount, int spread, float blowVelocity, int blowDuration, int blowReleaseDelay,
            int moveLength, int standLength, int standToShootLength, float skillVelocity)
            : base(id, w, pos, size, center, solid, factory, sound)
        {
            Initialize(spreadDuration, blowCount, blowPerSpread, scatterCount, spread, blowVelocity, blowDuration, blowReleaseDelay, moveLength, standLength, standToShootLength, skillVelocity);
            this.stats = stats;
        }

        private void Initialize(int spreadDuration, int blowCount, int blowPerSpread, int scatterCount, int spread, float blowVelocity, int blowDuration, int blowReleaseDelay,
            int moveLength, int standLength, int standToShootLength, float skillVelocity)
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "ShiftyShooter";

            //timers.Add("navigation", new Timer(navigationResetLength));
            //timers["navigation"].start();

            this.spreadDuration = spreadDuration;
            this.blowCount = blowCount;
            this.blowPerSpread = blowPerSpread;
            this.scatterCount = scatterCount;
            this.spread = spread;
            this.blowVelocity = blowVelocity;
            this.blowDuration = blowDuration;
            this.blowReleaseDelay = blowReleaseDelay;
            this.moveLength = moveLength;
            this.standLength = standLength;
            this.standToShootLength = standToShootLength;
            this.skillVelocity = skillVelocity;

            movementDirection = 0;
            facing = (Direction)0;

            timers.Add("done_moving", new Timer(moveLength + random.Next(moveVariation)));
            timers.Add("done_standing", new Timer(standLength + random.Next(standVariation)));
            timers.Add("shoot", new Timer(standToShootLength));
            timers.Add("ready_to_start", new Timer(300 + random.Next(300)));

            timers["ready_to_start"].resetAndStart();

            startingFriction = 0.3f;
            stoppingFriction = 0.1f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            state = StateName.Moving;
            timers["done_moving"].resetAndStart();

            //timers["done_standing"].resetAndStart();

            activeRange = 300f;
            this.speed = 70f;

            SetupStateMachine();

            //InitializeHpBar();
            //UpdateHpBar();

            killValue = 175;

            AddLight( new PointLightStruct(new Vector4(0.8f, 0.55f, 0.85f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 0f));

            targetable = true;

            aimDirection = random.Next(360);
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("delayed_start",
               new State("delayed_start",
                   c => !readyToStart,
                   c =>
                   {
                       if (timers["ready_to_start"].isDone())
                       {
                           readyToStart = true;
                           timers["done_moving"].resetAndStart();
                       }
                   }));

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
                            

                            
                            //else if (distanceToTarget() < attackRange)
                            //{
                            //    faceTarget();
                                
                           // }
                            //start timer to change
                            faceTarget();
                        }
                        bobAmount = 0.1f;


                    }));
            ctrl.Add("moving",
                new State("moving",
                    c => state == StateName.Moving,
                    c =>
                    {
                        if (null==stateTree.previousControlState || stateTree.previousControlState.name.Equals("standing"))
                        {
                            double movementRand = random.NextDouble();
                            //timers["done_moving"].resetAndStart();
                            if (movementRand > 0.5)
                            {
                                faceEntity(target, 0);
                            } else {
                                if (movementRand > 0.65)
                                {
                                    faceEntity(target, 90);
                                } else if (movementRand > 0.8) {
                                    faceEntity(target, -90);
                                }
                                else
                                {
                                    faceEntity(target, 180);
                                }
                            }
                        }
                        if ((GetEntitiesInRay(new Vector2(
                            this.pos.X + this.center.X + (float)Math.Cos(MathHelper.ToRadians(movementDirection)) * movementSpeed,
                            this.pos.Y + this.center.Y + (float)Math.Sin(MathHelper.ToRadians(movementDirection)) * movementSpeed)).Count == 0)
                            && RayWalkable(new Vector2(
                            this.pos.X + this.center.X + (float)Math.Cos(MathHelper.ToRadians(movementDirection)) * movementSpeed,
                            this.pos.Y + this.center.Y + (float)Math.Sin(MathHelper.ToRadians(movementDirection)) * movementSpeed)))
                            MoveForward(false, movementSpeed);

                        bobAmount = 1f;
                        //faceTarget();
                        //faceTarget();
                    }
                    ));

            Dictionary<String, State> unprovokedBranch = new Dictionary<String, State>();

            unprovokedBranch.Add("awaken",
                new State("awaken",
                    c => target != null && distanceToTarget() < 300,
                    c =>
                    {
                        hostile = true;
                    }));

            unprovokedBranch.Add("just_chillin",
                new State("just_chillin",
                    c => true,
                    c =>
                    {
                        bobAmount = 0f;
                        this.state = StateName.Standing;
                    }));


            stateTree = new StateTree();

            stateTree.AddBranch("unprovoked", new StateBranch(c => !hostile, unprovokedBranch));

            //stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (remove)
                return;

            if (hostile)
                _sound.setGlobalVariable("InCombat", 1f);

            //UpdateHpBar();
            UpdateAnimation();

            if (target == null)
                setTargetToClosestPlayer(true, 360);

            if (timers["done_moving"].isDone())
            {
                timers["done_moving"].reset();

                timers["done_moving"].setInterval(moveLength + random.Next(moveVariation));

                state = StateName.Standing;
                timers["done_standing"].resetAndStart();
                if (target != null)
                {
                    timers["shoot"].resetAndStart();
                    targetLight.PointLightRange = 60f;
                    light.DiffuseColor = hostileDiffuse;
                }
                else
                {
                    targetLight.PointLightRange = 0f;
                }
            }

            if (timers["done_standing"].isDone())
            {
                timers["done_standing"].reset();

                timers["done_standing"].setInterval(standLength + random.Next(standVariation));

                state = StateName.Moving;
                timers["done_moving"].resetAndStart();

                targetLight.PointLightRange = 30f;
            }

            if (timers["shoot"].isDone())
            {
                timers["shoot"].reset();
                //_sound.playCue("whsh", emitter);
                //fireAimedProjectile(Helper.degreeConversion(angleToEntity(target)));
                if (!hasLineOfSightToEntity(target))
                {
                    target = null;
                    light.DiffuseColor = diffuse;
                }

                if (target != null)
                {
                    faceTarget();
                    this.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(skillVelocity, 0f),
                    spreadDuration, blowCount, blowPerSpread, scatterCount, spread, false, blowVelocity, blowDuration, 30, 1f, new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 500, 10, "particles", 0, "white_to_green")), "", 0, "",
                    SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(250f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, false), true));
                    /*factory.addActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(skillVelocity, 0f), 
                    spreadDuration, blowCount, blowPerSpread, scatterCount, spread, false, blowVelocity, blowDuration, blowReleaseDelay, 1f, new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 500, "particles", 0, "white_to_green")), "", 0, "",
                    SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(250f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, false), true), 
                    this, null, Helper.degreeConversion(angleToEntity(target)));*/
                }
            }

            
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

            float torsoScale = (float)(random.NextDouble() + 0.5) * 2f;
            float armWidth = (float)(random.NextDouble() + 0.5) * 1f;
            float legWidth = (float)(random.NextDouble() - 1) * 1f + torsoScale;
            float armStance = (float)(random.NextDouble()) * 3f;
            float legStance = (float)(random.NextDouble() - 0) * 3f;
            float spineStance = (float)(random.NextDouble() + 0.5) * 1f;


            InitializeShootSprites(torsoScale, armWidth, legWidth, armStance, legStance, spineStance);
        }

        protected void UpdateAnimation()
        {


            
            String bad = "";

            if (timers["falling"].isActive() || timers["falling"].isDone())
            {
                bodySpriter.setAnimation("fall", 0, 0);
                return;
            }

            //faceTarget();
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
                bad += "_run";
            }
            else
            {
                bad += "_stand";
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

        protected void InitializeShootSprites(float torsoScale, float armWidth, float legWidth, float armStance, float legStance, float spineStance)
        {
            float tempHeight = 0;
            float sScale = size.X / 24f;

            tempHeight += 10f;
            AddAngledElement("LeftFoot", "shapes", "pink", 0, new Vector3(-15f * legStance, tempHeight, -20f * legWidth) * sScale, 0, 1f * sScale, 0f, new Vector3(-7f, -7f, 1f) * sScale, 1);
            AddAngledElement("RightFoot", "shapes", "pink", 0, new Vector3(-15f * legStance, tempHeight, 20f * legWidth) * sScale, 0, 1f * sScale, 0f, new Vector3(7f, 7f, -1f) * sScale, 1);

            //tempHeight += 10f;
            AddAngledElement("LeftKnee", "shapes", "pink", 0, new Vector3(10f * legStance, tempHeight, -25f * legWidth) * sScale, 0, 0.8f * sScale, 0f, new Vector3(-6f, 5f, 0f) * sScale, 1);
            AddAngledElement("RightKnee", "shapes", "pink", 0, new Vector3(10f * legStance, tempHeight, 25f * legWidth) * sScale, 0, 0.8f * sScale, 0f, new Vector3(6f, -5f, 0f) * sScale, 1);

            //tempHeight += 10;
            AddAngledElement("LeftLeg", "shapes", "pink", 0, new Vector3(1f * legStance, tempHeight, -17f * legWidth) * sScale, 0, 0.8f * sScale, 0f, new Vector3(-5f, -3f, 0f) * sScale, 1);
            AddAngledElement("RightLeg", "shapes", "pink", 0, new Vector3(1f * legStance, tempHeight, 17f * legWidth) * sScale, 0, 0.8f * sScale, 0f, new Vector3(5f, 3f, 0f) * sScale, 1);


            AddAngledElement("Crotch", "shapes", "violet", 0, new Vector3(10f * spineStance * torsoScale, tempHeight, 0f) * sScale, 0, 0.75f * sScale * torsoScale, 0f, new Vector3(0f, 4f, -3f) * sScale, 2);


            //AddAngledElement("RightButt", "shapes", "grey", 0, new Vector3(-5f * spineStance * torsoScale, tempHeight, 10f * torsoScale) * sScale, 0, 1.5f * sScale * torsoScale, 0f, new Vector3(0f, 5f, -1f) * sScale * torsoScale);
            //AddAngledElement("LeftButt", "shapes", "grey", 0, new Vector3(-5f * spineStance * torsoScale, tempHeight, -10f * torsoScale) * sScale, 0, 1.5f * sScale * torsoScale, 0f, new Vector3(0f, 5f, 1f) * sScale * torsoScale);

            tempHeight += 10f * torsoScale;
            AddAngledElement("Torso", "shapes", "violet", 0, new Vector3(-5f, tempHeight, 0f) * sScale, 0, 1.5f * sScale * torsoScale, 0f, new Vector3(0f, 10f, 0f) * sScale * torsoScale, 2);

            tempHeight -= 10;
            //AddAngledElement("LeftShoulder", "shapes", "teal", 0, new Vector3(5f * armStance, tempHeight, -25f * armWidth - 5f * torsoScale) * sScale, 0, 1.5f * sScale, 0f, new Vector3(0f, 10f, 5f) * sScale * torsoScale);
            //AddAngledElement("RightShoulder", "shapes", "teal", 0, new Vector3(5f * armStance, tempHeight, 25f * armWidth + 5f * torsoScale) * sScale, 0, 1.5f * sScale, 0f, new Vector3(0f, 10f, -5f) * sScale * torsoScale);

            //AddAngledElement("LeftElbow", "shapes", "teal", 0, new Vector3(15f * armStance, tempHeight, -5f) * sScale, 0, .75f * sScale, 0f, new Vector3(0f, 20f, 0f) * sScale);
            //AddAngledElement("RightElbow", "shapes", "teal", 0, new Vector3(15f * armStance, tempHeight, -5f) * sScale, 0, .75f * sScale, 0f, new Vector3(0f, 20f, 0f) * sScale);


            //AddAngledElement("LeftHand", "shapes", "pink", 0, new Vector3(25f * armStance, tempHeight, -5f) * sScale, 0, 1f * sScale, 0f, new Vector3(0f, 20f, 0f) * sScale);
            //AddAngledElement("RightHand", "shapes", "pink", 0, new Vector3(25f * armStance, tempHeight, -5f) * sScale, 0, 1f * sScale, 0f, new Vector3(0f, 20f, 0f) * sScale);

            //tempHeight -= 20f;
            AddAngledElement("Head", "shapes", "grey", 0, new Vector3(30f * spineStance, tempHeight, 0f) * sScale, 0, 1.8f * sScale, 0f, new Vector3(5f, 10f, 0f) * sScale, 2);

            tempHeight -= 80f;

            AddAngledElement("Face", "cactus", "flirt", 1, new Vector3(30f * spineStance + 1.5f, tempHeight, 0f) * sScale, 0, 0.4f * sScale, 0f, new Vector3(5f, 10f, 0f) * sScale, 2);
        }
    }
}
