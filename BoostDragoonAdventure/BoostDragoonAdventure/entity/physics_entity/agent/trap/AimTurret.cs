using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using wickedcrush.helper;
using wickedcrush.manager.audio;
using wickedcrush.utility;

namespace wickedcrush.entity.physics_entity.agent.trap
{
    public class AimTurret : Agent
    {
        public AimTurret(World w, Vector2 pos, EntityFactory factory, Direction facing, SoundManager sound)
            : base(w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize(facing);
        }

        private void Initialize(Direction facing)
        {
            //stats = new PersistedStats(5, 5);
            this.name = "AimTurret";

            this.facing = facing;

            timers.Add("firing", new Timer(500));
            timers["firing"].resetAndStart();

            activeRange = 200f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            target = null;
            setTargetToClosestPlayer();

            faceTarget();

            if(target!=null && timers["firing"].isDone())
            {
                fireAimedProjectile(Helper.degreeConversion(angleToEntity(target)));
                timers["firing"].resetAndStart();
            }
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            bodies["body"].BodyType = BodyType.Static;
        }



        public override void Remove()
        {
            base.Remove();
        }
    }
}
