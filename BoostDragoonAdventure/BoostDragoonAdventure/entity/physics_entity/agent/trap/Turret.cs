﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.stats;
using FarseerPhysics.Factories;
using wickedcrush.utility;


namespace wickedcrush.entity.physics_entity.agent.trap
{
    public class Turret : Agent
    {
        private EntityFactory factory;
        private Timer timer;

        public Turret(World w, Vector2 pos, EntityFactory factory, Direction facing)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true)
        {
            Initialize(pos, size, center, facing);

            this.factory = factory;
        }

        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, Direction facing)
        {
            stats = new PersistedStats(5, 5, 5);
            this.name = "Turret";

            this.facing = facing;

            timer = new Timer(1600);
            timer.start();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer.Update(gameTime);

            if (timer.isDone())
            {
                fireShot();
                timer.resetAndStart();
            }
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, body);
            body.FixedRotation = true;
            body.LinearVelocity = Vector2.Zero;
            body.BodyType = BodyType.Static;
            body.CollisionGroup = (short)CollisionGroup.AGENT;

            body.UserData = this;

            if (!solid)
                body.IsSensor = true;

            FixtureFactory.AttachRectangle(1f, 1f, 1f, center, hotSpot);
            hotSpot.FixedRotation = true;
            hotSpot.LinearVelocity = Vector2.Zero;
            hotSpot.BodyType = BodyType.Static;

        }


        //private void fireShot(object source, ElapsedEventArgs e)
        private void fireShot()
        {

            factory.addBolt(
                new Vector2(
                        (float)(pos.X + center.X + size.X * Math.Cos(MathHelper.ToRadians((float)facing))), //x component of pos
                        (float)(pos.Y + center.Y + size.Y * Math.Sin(MathHelper.ToRadians((float)facing)))), //y component of pos
                new Vector2(10f, 10f),
                new Vector2(5f, 5f),
                this);

            //timer = new Timer(600);

        }
    }
}
