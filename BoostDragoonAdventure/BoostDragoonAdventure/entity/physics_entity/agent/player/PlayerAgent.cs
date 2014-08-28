using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using wickedcrush.controls;
using wickedcrush.helper;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.stats;
using wickedcrush.entity.physics_entity.agent.attack;
using wickedcrush.factory.entity;
using wickedcrush.behavior.state;
using wickedcrush.behavior;
using wickedcrush.manager.audio;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;
        
        
        private float boostSpeed = 100f;
        private bool overheating = false;
        private int chargeLevel = 0;
        
        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, PersistedStats stats, EntityFactory factory, SoundManager sound)
            : base(w, pos, size, center, solid, factory, stats, sound)
        {
            Initialize(pos, size, center, solid, controls);

            
        }

        private void Initialize(Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls)
        {
            this.controls = controls;
            this.name = "Player";

            this.facing = Direction.East;
            movementDirection = facing;

            _sound.addSound("blast off", "bfxr/blast off");
            _sound.addSound("whsh", "bfxr/whsh");
            _sound.addSound("whsh2", "bfxr/Randomize6");
            _sound.addSound("ded", "bfxr/Randomize10");
            _sound.addSound("smash", "bfxr/smash");
            _sound.addSound("charge", "bfxr/charging");
            _sound.addSound("ping", "bfxr/ping");
            _sound.addAmbient("blast off", id + "blast off", false);
            _sound.addAmbient("charge", id + "charge", false);
            SetupStateMachine();
        }

        private void applyStats()
        {
            boostSpeed = 100f * (1 + (float)stats.get("boostSpeedMod") * (0.01f));
        }

        
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (stats.compare("boost", "maxBoost") == -1)
                stats.addTo("boost", stats.get("fillSpeed"));

            if (stats.compare("boost", "maxBoost") >= 0)
            {
                overheating = false;
                stats.set("boost", stats.get("maxBoost"));
            }

            if (stats.get("boost") <= 0)
            {
                overheating = true;
                stats.set("boost", 0);
            }
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();
            ctrl.Add("boosting",
                new State("boosting",
                    c => ((PlayerAgent)c).controls.BoostHeld()
                    && !((PlayerAgent)c).overheating,
                    c =>
                    {
                        if(sm.previousControlState.name != "boosting")
                            _sound.playAmbient(id + "blast off");
                        
                        UpdateDirection();
                        BoostForward();
                        stats.addTo("boost", -stats.get("useSpeed"));

                        if (controls.ActionPressed())
                        {
                            attackForward(new Vector2(36, 36), 2, 50);
                            _sound.playSound("whsh");
                        }
                    }));
            

            ctrl.Add("default",
                new State("default",
                    c => true,
                    c =>
                    {
                        UpdateDirection();
                        WalkForward();

                        _sound.stopAmbientSound(id + "blast off");

                        if (controls.ActionPressed())
                        {
                            attackForward(new Vector2(36, 36), 1, 30);
                            _sound.playSound("whsh");
                        }

                        if (controls.ActionHeld())
                        {
                            chargeLevel++;

                            if (chargeLevel > 25) 
                                _sound.playAmbient(id + "charge");

                            if (chargeLevel == 100)
                                _sound.playSound("ping");
                        }
                        else
                        {
                            if (chargeLevel > 100)
                            {
                                attackForward(new Vector2(36, 36), 3, 100);
                                _sound.playSound("smash");
                            } else if (chargeLevel > 25)
                            {
                                attackForward(new Vector2(36, 36), 2, 100);
                                _sound.playSound("smash");
                            }

                            _sound.stopAmbientSound(id + "charge");
                            chargeLevel = 0;
                        }
                        
                    }));

            sm = new StateMachine(ctrl);
        }

        protected void UpdateDirection()
        {
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            if (magnitude == 0f)
                return;

            Direction temp = (Direction)
                        Helper.degreeConversion((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));

            strafe = controls.StrafeHeld();

            if (!strafe)
                facing = temp;

            movementDirection = temp;
        }

        protected void WalkForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;
            
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );
            speed = 75f;
            v += unitVector * magnitude * speed * startingFriction;

            bodies["body"].LinearVelocity = v;

            airborne = false;
        }

        protected void BoostForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );

            v += unitVector * boostSpeed;

            bodies["body"].LinearVelocity = v;

            airborne = true;
        }
        #endregion

        public bool InteractPressed()
        {
            return controls.InteractPressed();
        }
        
    }
        
}
