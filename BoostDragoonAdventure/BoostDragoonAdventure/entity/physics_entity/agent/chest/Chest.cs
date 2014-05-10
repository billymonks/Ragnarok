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

namespace wickedcrush.entity.physics_entity.agent.chest
{
    public class Chest : Agent
    {
        private Color testColor = Color.Green;

        private bool opened = false;

        private List<Item> contents;
        private int currency;

        public Chest(World w, Vector2 pos, Vector2 size, Vector2 center, EntityFactory factory)
            : base(w, pos, size, center, true, factory)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.name = "Chest";
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

            bodies.Add("activeArea", BodyFactory.CreateBody(w, pos - (center + new Vector2(20f, 20f))));
            FixtureFactory.AttachRectangle(size.X + 40f, size.Y + 40f, 1f, center + new Vector2(20f, 20f), bodies["activeArea"]);
            bodies["activeArea"].IsSensor = true;
            bodies["activeArea"].BodyType = BodyType.Static;
            bodies["activeArea"].LinearVelocity = Vector2.Zero;

            //bodies.Add("interactArea", BodyFactory.CreateBody(w, pos - new Vector2(
            //bodies["hotspot"].IsSensor = true;


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
                    }));
            ctrl.Add("opened",
                new State("opened",
                    c => true,
                    c =>
                    {
                        testColor = Color.Blue;
                    }));
        }

        private void CheckForOpen()
        {
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if(((PlayerAgent)e).InteractPressed())
                    {
                        //((PlayerAgent)e).stats
                        opened = true;
                    }
                }
            }
        }
    }
}
