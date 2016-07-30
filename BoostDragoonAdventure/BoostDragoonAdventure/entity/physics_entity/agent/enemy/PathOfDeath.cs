using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.map.path;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using wickedcrush.factory.entity;
using wickedcrush.stats;
using wickedcrush.manager.audio;
using wickedcrush.utility;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.particle;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class PathOfDeath : Attack
    {
        Stack<PathNode> patrol;
        //private float speed = 60f;
        private int navigationResetLength = 500;
        float angle = 0f;

        public PathOfDeath(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound, Stack<PathNode> patrol, int damage, int force)
            : base(-1, w, pos, size, center, false, damage, force, sound, factory)
        {
            Initialize();
            //this.stats = stats;
            this.patrol = patrol;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "PathOfDeath";

            timers.Add("navigation", new Timer(navigationResetLength));
            timers["navigation"].start();

            //timers.Add("falling", new Timer(500));

            startingFriction = 0.5f;
            stoppingFriction = 0.2f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            this.airborne = true;
            this.immortal = true;

            activeRange = 300f;
            this.targetSpeed = 300f;

            AddLight( new PointLightStruct(new Vector4(0.8f, 0.65f, 0.3f, 1f), 1f, new Vector4(0.8f, 0.65f, 0.3f, 1f), 0.3f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 500f));
            //factory._gm.scene.AddLight(light);

            SetupStateMachine();

            //InitializeHpBar();

            //item = ItemServer.getItem("Longsword");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (remove)
            {
                return;
            }

            bodySpriter.setAngle(angle);
            angle += gameTime.ElapsedGameTime.Milliseconds;

            //UpdateHpBar();
            //UpdateAnimation();

            EmitParticles(ParticleServer.GenerateSpark(new Vector3(this.pos.X + this.center.X, 10f, this.pos.Y + this.center.Y), new Vector2(1f, 1f)), 1);

            movementDirection = (int)facing;

        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("patrol",
                new State("patrol",
                    c => true,
                    c =>
                    {
                        if (path == null || path.Count == 0)
                        {
                            path = new Stack<PathNode>(patrol);
                        }

                        //this.speed = walkSpeed;
                        if (hostile)
                            _sound.setGlobalVariable("InCombat", 1f);

                        FollowPath(false);
                    }));
            
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("fireballz", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 5, factory._spriterManager.spriters["all"].loader));

            bodySpriter = sPlayers["fireballz"];
            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);
            bodySpriter.setScale((((float)size.X) / 100f) * (2f / factory._gm.camera.zoom));
            height = 10;

        }

        protected override void HandleCollisions()
        {
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !((Agent)c.Other.UserData).noCollision)
                {

                    ((Agent)c.Other.UserData).TakeHit(this, true);

                }
                

                c = c.Next;
            }
        }
    }
}
