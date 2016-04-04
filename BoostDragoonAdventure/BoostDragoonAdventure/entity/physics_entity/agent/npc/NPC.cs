using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.gameplay;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.behavior;
using wickedcrush.behavior.state;
using wickedcrush.entity.physics_entity.agent.player;
using wickedcrush.screen;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.npc
{
    public class NPC : Agent
    {
        GameBase _game;
        int eventId;

        public NPC(Vector2 pos, Vector2 size, int eventId, GameBase game, GameplayManager gameplayManager)
            : base(eventId, gameplayManager.w, pos, size, size / 2f, true, gameplayManager.factory, game.soundManager)
        {
            _game = game;
            this.eventId = eventId;
            Initialize();
        }

        private void Initialize()
        {
            this.name = "NPC";
            immortal = true;
            activeRange = size.X*1.5f;
            SetupStateMachine();
            //SetupSpriterPlayer();
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            bodies = new Dictionary<String, Body>();
            bodies.Add("body", BodyFactory.CreateBody(w, pos - center));

            FixtureFactory.AttachRectangle(size.X, size.Y, 1f, center, bodies["body"]);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Static;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;
            bodies["body"].UserData = this;
            bodies["body"].IsSensor = false;


        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);
            drawBody = false;

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;

            float torsoScale = (float)(random.NextDouble() - 0.25) * 4f;
            float armWidth = (float)(random.NextDouble() - 0) * 3f;
            float legWidth = (float)(random.NextDouble() - 0) * 2f;
            float armStance = (float)(random.NextDouble() - 0.5) * 4f;
            float legStance = (float)(random.NextDouble() - 0.5) * 4f;
            float spineStance = (float)(random.NextDouble() - 0.25) * 4f;

            if (kid == 4)
            {
                AddAngledElement("Magazine", "shapes", "grey", 0, new Vector3(0f, 5f, 0f) * 1, 0, 1, 0f, new Vector3(0f, 4f, -3f) * 0);
            }
            else
            {
                InitializeHumanoidSprites(torsoScale, armWidth, legWidth, armStance, legStance, spineStance);
                SetCostume(2);
            }

        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("idle",
                new State("idle",
                    c => true,
                    c =>
                    {
                        CheckForUse();
                    }));
            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        private void CheckForUse()
        {
            bool interacted = false;

            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if (((PlayerAgent)e).InteractPressed())
                    {
                        //factory.createDialog(dialog, new Vector2(200f, 200f));
                        faceEntity(e);
                        interacted = true;
                        factory.StartEvent(eventId);
                    }
                }
            }

            if (interacted)
            {
                targetBobAmount = 1f;
            }
            else
            {
                targetBobAmount = 0.1f;
            }
        }

    }
}
