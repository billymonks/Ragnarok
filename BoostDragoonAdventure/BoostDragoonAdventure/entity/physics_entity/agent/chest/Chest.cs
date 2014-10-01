using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.behavior.state;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.inventory;
using wickedcrush.behavior;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.manager.audio;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.chest
{
    public class Chest : Agent
    {
        private Color testColor = Color.Green;

        private bool opened = false;

        private List<Item> contents;
        private int currency;

        public Chest(World w, Vector2 pos, EntityFactory factory, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.name = "Chest";
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

            //bodies.Add("activeArea", BodyFactory.CreateBody(w, pos - (center + new Vector2(20f, 20f))));
            //FixtureFactory.AttachRectangle(size.X + 40f, size.Y + 40f, 1f, center + new Vector2(20f, 20f), bodies["activeArea"]);
            //bodies["activeArea"].IsSensor = true;
            //bodies["activeArea"].BodyType = BodyType.Static;
            //bodies["activeArea"].LinearVelocity = Vector2.Zero;



        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("closed",
                new State("closed",
                    c => !((Chest)c).opened,
                    c =>
                    {
                        testColor = Color.Green;
                        CheckForOpen();
                    }));
            ctrl.Add("opened",
                new State("opened",
                    c => true,
                    c =>
                    {
                        testColor = Color.Blue;
                    }));
            sm = new StateMachine(ctrl);
        }

        private void CheckForOpen()
        {
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if(((PlayerAgent)e).InteractPressed())
                    {
                        opened = true;
                        ((PlayerAgent)e).stats.inventory.receiveItem(ItemServer.getRandomItem());
                    }
                }
            }
        }

        public override void DebugDraw(Texture2D wTex, Texture2D aTex, GraphicsDevice gd, SpriteBatch spriteBatch, SpriteFont f, Color c, Camera camera)
        {
            spriteBatch.Draw(wTex, bodies["body"].Position - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, bodies["body"].Rotation, Vector2.Zero, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(aTex, pos + center - new Vector2(camera.cameraPosition.X, camera.cameraPosition.Y), null, testColor, MathHelper.ToRadians((float)facing), center, size / new Vector2(aTex.Width, aTex.Height), SpriteEffects.None, 0f);
            
            DrawName(spriteBatch, f, camera);
        }
    }
}
