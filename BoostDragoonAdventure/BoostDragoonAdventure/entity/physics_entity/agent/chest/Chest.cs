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
using wickedcrush.particle;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.chest
{
    public class Chest : Agent
    {
        private Color testColor = Color.Green;

        private bool opened = false;

        private List<Weapon> contents;
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
            //bodies.Add("hotspot", BodyFactory.CreateBody(w, pos));
            //bodies["hotspot"].IsSensor = true;

            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, bodies["body"]);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Static;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;
            bodies["body"].UserData = this;
            bodies["body"].IsSensor = false;

        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("closed",
                new State("closed",
                    c => !((Chest)c).opened,
                    c =>
                    {
                        bodySpriter.setAnimation("closed", 0, 0);
                        testColor = Color.Green;
                        CheckForOpen();

                    }));
            ctrl.Add("opened",
                new State("opened",
                    c => true,
                    c =>
                    {
                        bodySpriter.setAnimation("open", 0, 0);
                        testColor = Color.Blue;
                    }));
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("chest", new SpriterPlayer(factory._spriterManager.spriters["chest"].getSpriterData(), 0, factory._spriterManager.spriters["chest"].loader));
            bodySpriter = sPlayers["chest"];
            bodySpriter.setFrameSpeed(20);

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
                        Item temp = InventoryServer.getRandomItem();
                        //factory.addText("You got " + temp.name + "!", this.pos + this.center, 3000);
                        factory.DisplayMessage("You got " + temp.name + "!");
                        ((PlayerAgent)e).stats.inventory.receiveItem(temp);

                        ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - 5, 20, this.pos.Y + this.center.Y - 5), new Vector3(10, 0, 10), new Vector3(-0.5f, -1f, -0.5f), new Vector3(1f, 2f, 1f), new Vector3(0, .03f, 0), 0f, 0f, 1000, 100, "particles", 0, "white_to_blue");
                        EmitParticles(ps, 10);
                        ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - 5, 20, this.pos.Y + this.center.Y - 5), new Vector3(10, 0, 10), new Vector3(-0.5f, -1f, -0.5f), new Vector3(1f, 2f, 1f), new Vector3(0, .03f, 0), 0f, 0f, 1000, 100, "particles", 0, "white_to_yellow");
                        EmitParticles(ps, 10);
                        ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - 5, 20, this.pos.Y + this.center.Y - 5), new Vector3(10, 0, 10), new Vector3(-0.5f, -1f, -0.5f), new Vector3(1f, 2f, 1f), new Vector3(0, .03f, 0), 0f, 0f, 1000, 100, "particles", 0, "white_to_orange");
                        EmitParticles(ps, 10);

                        //((PlayerAgent)e).stats.inventory.addCurrency(100);
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
