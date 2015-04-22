using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using wickedcrush.utility;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using FarseerPhysics.Dynamics;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.helper;
using FarseerPhysics.Factories;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Murderer : Agent
    {
        private Color testColor = Color.Green;

        private const int attackTellLength = 500, postAttackLength = 900, navigationResetLength = 500, attackRange = 30;

        public Murderer(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, PersistedStats stats, SoundManager sound)
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
            ctrl.Add("staggered",
                new State("staggered",
                    c => ((Murderer)c).staggered,
                    c =>
                    {
                        testColor = Color.White;
                        ResetAllTimers();

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("post_attack",
                new State("post_attack",
                    c => ((Murderer)c).timers["post_attack"].isActive(),
                    c =>
                    {
                        testColor = Color.Violet;
                        if (timers["post_attack"].isDone())
                        {
                            timers["post_attack"].reset();
                        }

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("attack_tell",
                new State("attack_tell",
                    c => ((Murderer)c).timers["attack_tell"].isActive(),
                    c =>
                    {
                        if (!sm.previousControlState.name.Equals("attack_tell"))
                        {
                            faceTarget();
                        }

                        testColor = Color.Yellow;
                        if(timers["attack_tell"].isDone())
                        {
                            attackForward(new Vector2(48, 48), 3, 30);
                            testColor = Color.Red;
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

                        testColor = Color.Green;

                        _sound.setGlobalVariable("InCombat", 1f);
                    }));
            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        if(target==null)
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

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            //if (navigator != null)
            //{
                //navigator.DebugDraw(wTex, gd, spriteBatch, f);
            //}

            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, pos + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            
            DrawName(spriteBatch, f, camera);

            DebugDrawHealth(wTex, aTex, gd, spriteBatch, f, c, camera);
        }
    }
}
