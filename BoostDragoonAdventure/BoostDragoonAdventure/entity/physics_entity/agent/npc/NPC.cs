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

namespace wickedcrush.entity.physics_entity.agent.npc
{
    public class NPC : Agent
    {
        GameBase _game;
        String dialog;

        public NPC(Vector2 pos, Vector2 size, String dialog, GameBase game, GameplayManager gameplayManager)
            : base(-1, gameplayManager.w, pos, size, size / 2f, true, gameplayManager.factory, game.soundManager)
        {
            _game = game;
            this.dialog = dialog;
            Initialize();
        }

        private void Initialize()
        {
            this.name = "NPC";
            immortal = true;
            activeRange = size.X*1.5f;
            SetupStateMachine();

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
            foreach (Entity e in proximity)
            {
                if (e is PlayerAgent)
                {
                    if (((PlayerAgent)e).InteractPressed())
                    {
                        //factory.createDialog(dialog, new Vector2(200f, 200f));
                        factory.StartEvent(2);
                    }
                }
            }
        }

    }
}
