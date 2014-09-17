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
using wickedcrush.utility.trigger;
using wickedcrush.manager.audio;


namespace wickedcrush.entity.physics_entity.agent.trap.triggerable.turret
{
    public class Turret : Triggerable
    {
        //private EntityFactory factory;

        public Turret(World w, Vector2 pos, EntityFactory factory, Direction facing, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize(facing);
        }

        private void Initialize(Direction facing)
        {
            //stats = new PersistedStats(5, 5);
            this.name = "Turret";

            this.facing = facing;
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            
            bodies["body"].BodyType = BodyType.Static;
        }

        

        public override void activate()
        {
            if(!dead)
                fireBolt();
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
