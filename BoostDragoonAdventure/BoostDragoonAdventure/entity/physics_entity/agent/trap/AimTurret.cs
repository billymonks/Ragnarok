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
using Com.Brashmonkey.Spriter.player;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.trap
{
    public class AimTurret : Agent
    {

        public int fireSpeed = 500, rank;

        public AimTurret(int id, World w, Vector2 pos, EntityFactory factory, Direction facing, SoundManager sound, int rank)
            : base(id, w, pos, new Vector2(20f, 20f), new Vector2(10f, 10f), true, factory, sound)
        {
            Initialize(facing, rank);
        }

        private void Initialize(Direction facing, int rank)
        {
            //stats = new PersistedStats(5, 5);
            this.name = "AimTurret";
            this.rank = rank;
            this.facing = facing;

            timers.Add("firing", new Timer(fireSpeed * rank));
            

            activeRange = 400f;

            targetable = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            target = null;
            setTargetToClosestPlayer(true, 360);

            faceTarget();

            if (target == null)
            {
                timers["firing"].reset();
            }
            else if (timers["firing"].isDone())
            {
                fireAimedProjectile(Helper.degreeConversion(angleToEntity(target)));
                _sound.playCue("whsh", emitter);
                timers["firing"].resetAndStart();
                _sound.setGlobalVariable("InCombat", 1f);
                angledSpriters["turretFront"].player.setAnimation("teal", 0, 0);
            }
            else if (!timers["firing"].isActive())
            {
                timers["firing"].resetAndStart();
                _sound.setGlobalVariable("InCombat", 1f);
            }
            else
            {
                if(timers["firing"].getPercent() > 0.75f)
                {
                    angledSpriters["turretFront"].player.setAnimation("red", 0, 0);

                    Vector2 sparkPos = Helper.GetDirectionVectorFromDegrees((float)aimDirection) * 25f;

                    EmitParticles(ParticleServer.GenerateSpark(new Vector3(pos.X + center.X + sparkPos.X, 10f,pos.Y + center.Y + sparkPos.Y), Helper.GetDirectionVectorFromDegrees((float)aimDirection)), 1);
                }
                else if (timers["firing"].getPercent() > 0.45f)
                {
                    angledSpriters["turretFront"].player.setAnimation("pink", 0, 0);
                }
                _sound.setGlobalVariable("InCombat", 1f);
            }

            
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);

            bodies["body"].BodyType = BodyType.Static;
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("cursor", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 2, factory._spriterManager.spriters["all"].loader));
            sPlayers.Add("hud", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 4, factory._spriterManager.spriters["all"].loader));


            bodySpriter = sPlayers["cursor"];
            bodySpriter.setAnimation("hover", 0, 0);
            bodySpriter.setFrameSpeed(20);

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;

            AddAngledElement("turretFront", "shapes", "teal", 0, new Vector3(30f, 10f, 0f), 0, 1f, 0f, Vector3.Zero, 0);
        }



        public override void Remove()
        {
            base.Remove();
        }
    }
}
