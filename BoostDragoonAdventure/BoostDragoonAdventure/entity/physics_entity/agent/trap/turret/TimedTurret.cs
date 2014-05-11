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


namespace wickedcrush.entity.physics_entity.agent.trap.turret
{
    public class TimedTurret : Turret
    {
        private EntityFactory factory;

        public TimedTurret(World w, Vector2 pos, EntityFactory factory, Direction facing)
            : base(w, pos, factory, facing)
        {
            
        }

        protected override void Initialize(Vector2 pos, Vector2 size, Vector2 center, Direction facing)
        {
            stats = new PersistedStats(5, 5);
            this.name = "TimedTurret";

            this.facing = facing;

            //triggers.Add("TurretTrigger", new TimedTrigger(this, this, new Timer(1600)));
            //triggers["TurretTrigger"].repeat = true;

        }
    }
}
