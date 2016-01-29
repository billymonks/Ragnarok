using Com.Brashmonkey.Spriter.player;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;
using wickedcrush.map.path;
using wickedcrush.utility;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.enemy
{
    public class Cactus : Attack
    {
        Stack<PathNode> patrol;
        //private float speed = 60f;
        private int navigationResetLength = 500, spikeAnimationTimer = 500;
        float angle = 0f;

        public Cactus(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, EntityFactory factory, SoundManager sound, Stack<PathNode> patrol, int damage, int force)
            : base(w, pos, size, center, false, damage, force, sound, factory)
        {
            Initialize();
            //this.stats = stats;
            this.patrol = patrol;
        }

        private void Initialize()
        {
            //stats = new PersistedStats(10, 10, 5);
            this.name = "Cactus";

            timers.Add("navigation", new Timer(navigationResetLength));
            timers["navigation"].start();

            timers.Add("spike_animation", new Timer(spikeAnimationTimer));

            //timers.Add("falling", new Timer(500));

            startingFriction = 0.5f;
            stoppingFriction = 0.2f; //1+ is friction city, 1 is a lotta friction, 0.1 is a little slippery, 0.01 is quite slip

            this.airborne = true;
            this.immortal = false;

            activeRange = 300f;
            this.speed = 100f;

            SetupStateMachine();

            AddLight(new PointLightStruct(new Vector4(0.1f, 0.85f, 0.05f, 1f), 0.2f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 40f));
            //factory._gm.scene.AddLight(light);

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

            //bodySpriter.setAngle(angle);
            //angle += gameTime.ElapsedGameTime.Milliseconds;

            //UpdateHpBar();
            //UpdateAnimation();

            if(timers["spike_animation"].isActive())
            {
                bodySpriter.setAnimation("spikey", 0, 0);
            } else if(timers["spike_animation"].isDone())
            {
                bodySpriter.setAnimation("plain", 0, 0);
                timers["spike_animation"].reset();
            }

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

                        FollowPath(false);
                    }));

            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("body", new SpriterPlayer(factory._spriterManager.spriters["cactus"].getSpriterData(), 0, factory._spriterManager.spriters["cactus"].loader));
            sPlayers.Add("face", new SpriterPlayer(factory._spriterManager.spriters["cactus"].getSpriterData(), 1, factory._spriterManager.spriters["cactus"].loader));

            bodySpriter = sPlayers["body"];
            overlaySpriter = sPlayers["face"];
            
            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);
            bodySpriter.setScale((((float)size.X) / 100f) * (2f / factory._gm.camera.zoom));

            overlaySpriter.setFrameSpeed(60);
            overlaySpriter.setScale((((float)size.X) / 100f) * (2f / factory._gm.camera.zoom));
            overlaySpriter.setAnimation("love", 0, 0);
            //height = 10;

        }

        protected override void HandleCollisions()
        {
            var c = bodies["body"].ContactList;
            while (c != null)
            {
                

                if (c.Contact.IsTouching
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !(c.Other.UserData is Cactus)
                    && !((Agent)c.Other.UserData).noCollision)
                {
                    //factory.DisplayMessage("!");
                    ((Agent)c.Other.UserData).TakeHit(this, true);
                    timers["spike_animation"].resetAndStart();
                }

                //if (c.Other.UserData is Chimera || !((Chimera)c.Other.UserData).airborne)
                //{
                    //this.Die();
                //}


                c = c.Next;
            }
        }
    }
}
