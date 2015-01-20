using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using FarseerPhysics.Factories;
using wickedcrush.behavior.state;
using wickedcrush.entity.physics_entity.agent.chest;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.display._3d;
using wickedcrush.behavior;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.screen;
using wickedcrush.manager.gameplay;

namespace wickedcrush.entity.physics_entity.agent.npc
{
    public class TerminalNPC : Agent
    {
        GameBase _game;

        public TerminalNPC(Vector2 pos, GameBase game, GameplayManager gameplayManager)
            : base(gameplayManager.w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, gameplayManager.factory, game.soundManager)
        {
            this._game = game;
            Initialize();
        }

        private void Initialize()
        {
            this.name = "Terminal";
            immortal = true;
            activeRange = 40f;
            SetupStateMachine();
            
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            //base.setupBody(w, pos, size, center, solid);
            bodies = new Dictionary<String, Body>();
            bodies.Add("body", BodyFactory.CreateBody(w, pos - center));
            bodies.Add("hotspot", BodyFactory.CreateBody(w, pos));
            bodies["hotspot"].IsSensor = true;

            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, bodies["body"]);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Static;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;
            bodies["body"].UserData = this;
            bodies["body"].IsSensor = false;

            FixtureFactory.AttachRectangle(1f, 1f, 1f, Vector2.Zero, bodies["hotspot"]);
            bodies["hotspot"].FixedRotation = true;
            bodies["hotspot"].LinearVelocity = Vector2.Zero;
            bodies["hotspot"].BodyType = BodyType.Static;
            bodies["hotspot"].IsSensor = true;

        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        CheckForUse();
                    }));
            sm = new StateMachine(ctrl);
        }

        private void CheckForUse()
        {
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if (((PlayerAgent)e).InteractPressed())
                    {
                        _game.screenManager.AddScreen(new EditorScreen(_game, ((PlayerAgent)e).player));
                    }
                }
            }
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, Color.Green, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            //spriteBatch.Draw(aTex, pos + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);

            DrawName(spriteBatch, f, camera);
        }
    }
}
