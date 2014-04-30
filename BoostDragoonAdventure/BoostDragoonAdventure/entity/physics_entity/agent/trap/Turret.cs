using System;
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

        public Turret(World w, Vector2 pos, EntityFactory factory, Direction facing)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory)
        {
            Initialize(pos, size, center, facing);

            this.factory = factory;
        }

        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, Direction facing)
        {
            stats = new PersistedStats(5, 5, 5);
            this.name = "Turret";

            this.facing = facing;

            timers.Add("shot", new Timer(1600));
            timers["shot"].start();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (timers["shot"].isDone())
            {
                fireShot();
                timers["shot"].resetAndStart();
            }
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, bodies["body"]);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Static;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;

            bodies["body"].UserData = this;

            if (!solid)
                bodies["body"].IsSensor = true;

            FixtureFactory.AttachRectangle(1f, 1f, 1f, center, bodies["hotspot"]);
            bodies["hotspot"].FixedRotation = true;
            bodies["hotspot"].LinearVelocity = Vector2.Zero;
            bodies["hotspot"].BodyType = BodyType.Static;

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
                this,
                1,
                1);

            //timer = new Timer(600);

        }
    }
}
