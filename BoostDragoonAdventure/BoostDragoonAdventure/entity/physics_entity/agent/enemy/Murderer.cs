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

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Murderer : Agent
    {
        private Color testColor = Color.Green;

        private const int attackTellLength = 750, postAttackLength = 1200, navigationResetLength = 500;

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

            SetupStateMachine();
            
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            bodies.Add("activeArea", BodyFactory.CreateBody(w, pos - new Vector2(300f, 300f)));
            FixtureFactory.AttachCircle(600f, 1f, bodies["activeArea"], center);
            bodies["activeArea"].IsSensor = true;
            bodies["activeArea"].BodyType = BodyType.Dynamic;
            bodies["activeArea"].LinearVelocity = Vector2.Zero;
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
                    }));
            ctrl.Add("chase",
                new State("chase",
                    c => distanceToTarget() > 40,
                    c =>
                    {
                        if (timers["navigation"].isDone())
                        {
                            createPathToTarget();
                            timers["navigation"].resetAndStart();
                        }

                        FollowPath();

                        testColor = Color.Green;
                    }));
            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        if(target==null)
                            setTargetToClosestPlayer();
                        else if(distanceToTarget() < 40)
                            timers["attack_tell"].resetAndStart();
                        
                    }));
            sm = new StateMachine(ctrl);
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            //if (navigator != null)
            //{
                //navigator.DebugDraw(wTex, gd, spriteBatch, f);
            //}

            spriteBatch.Draw(wTex, bodies["body"].Position, null, testColor, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, pos+center, null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            
            DrawName(spriteBatch, f);

            DebugDrawHealth(wTex, aTex, gd, spriteBatch, f, c);
        }
    }
}
