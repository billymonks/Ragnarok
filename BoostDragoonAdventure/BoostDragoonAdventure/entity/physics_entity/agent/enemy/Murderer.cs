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

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Murderer : Agent
    {
        private Color testColor = Color.Green;

        public Murderer(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory)
            : base(w, pos, size, center, solid, factory)
        {
            Initialize();
            this.stats = stats;
        }

        private void Initialize()
        {
            stats = new PersistedStats(10, 10, 5);
            this.name = "Murderer";

            timers.Add("navigation", new Timer(500));
            timers["navigation"].start();

            timers.Add("attack_tell", new Timer(1000));
            timers.Add("post_attack", new Timer(1000));

            SetupStateMachine();
            
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
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
                        testColor = Color.Yellow;
                        if(timers["attack_tell"].isDone())
                        {
                            attackForward();
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
                        if(distanceToTarget() < 40)
                            timers["attack_tell"].resetAndStart();
                        
                    }));
            sm = new StateMachine(ctrl);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DebugDraw(Texture2D tex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c)
        {
            spriteBatch.Draw(tex, body.Position, null, testColor, body.Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            //spriteBatch.Draw(tex, hotSpot.WorldCenter, null, Color.Yellow, hotSpot.Rotation, Vector2.Zero, new Vector2(1f, 1f), SpriteEffects.None, 0f);
            spriteBatch.DrawString(f, name, pos, Color.Black);
        }
    }
}
