﻿using System;
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
using Microsoft.Xna.Framework.Audio;
using wickedcrush.inventory;
using wickedcrush.player;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;

        private float walkSpeed = 40f, runSpeed = 75f, boostSpeed = 100f;
        private bool overheating = false, inCharge = false, lockChargeDirection = false, canAttackWhileOverheating = true;
        private int chargeLevel = 0, itemAChargeLevel = 0, itemBChargeLevel = 0;

        public bool busy = false;

        public Player player;

        //pollable gameplay stuff:
        private bool dodgeSuccess = false;

        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, PersistedStats stats, EntityFactory factory, SoundManager sound, String name, Player player)
            : base(w, pos, size, center, solid, factory, stats, sound)
        {
            this.player = player;
            Initialize(name, pos, size, center, solid, controls);


        }

        private void Initialize(String name, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls)
        {
            this.controls = controls;
            this.name = name;

            this.facing = Direction.East;
            movementDirection = (int)facing;

            timers.Add("boostRecharge", new utility.Timer(stats.get("boostRecharge")));

            timers.Add("iFrameTime", new utility.Timer(stats.get("iFrameTime")));
            timers["iFrameTime"].resetAndStart();

            stats.inventory.itemA = ItemServer.getItem("Healthsweed");
            stats.inventory.itemB = ItemServer.getItem("Spellbook: Fireball");

            _sound.addCueInstance("blast off", id + "blast off", false);
            _sound.addCueInstance("charging", id + "charging", false);
            SetupStateMachine();
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("standing", new SpriterPlayer(factory._spriterManager.spriters["neku"].getSpriterData(), 0, factory._spriterManager.loaders["loader1"]));
            sPlayers.Add("boosting", new SpriterPlayer(factory._spriterManager.spriters["neku"].getSpriterData(), 1, factory._spriterManager.loaders["loader1"]));
            //sPlayer.setAnimation("standing_north", 0, 0);
            sPlayer = sPlayers["standing"];
            sPlayer.setFrameSpeed(20);
            
        }

        private void applyStats()
        {
            boostSpeed = 100f * (1 + (float)stats.get("boostSpeedMod") * (0.01f));
        }

        public override void TakeHit(Attack attack)
        {
            if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
            {
                dodgeSuccess = true;
                _sound.playCue("ping3", emitter);
            }
            else
            {
                base.TakeHit(attack);
                _sound.playCue("oof", emitter);
            }
        }

        public bool pollDodgeSuccess()
        {
            if (dodgeSuccess)
            {
                dodgeSuccess = false;
                return true;
            }

            return false;
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
                    c => !((PlayerAgent)c).timers["boostRecharge"].isDone()
                        && ((PlayerAgent)c).timers["boostRecharge"].isActive()
                    || ((PlayerAgent)c).controls.BoostHeld()
                    && !((PlayerAgent)c).overheating
                    && !((PlayerAgent)c).busy,
                    c =>
                    {

                        if (sm.previousControlState != null && sm.previousControlState.name != "boosting")
                        {
                            timers["iFrameTime"].resetAndStart();
                            _sound.playCueInstance(id + "blast off", emitter);
                        }

                        if (((PlayerAgent)c).controls.BoostPressed())
                            timers["boostRecharge"].resetAndStart();

                        UpdateDirection(false);
                        sPlayer = sPlayers["boosting"];
                        UpdateAnimation();
                        BoostForward();
                        stats.addTo("boost", -stats.get("useSpeed"));

                        if (controls.ActionPressed())
                        {
                            stats.addTo("boost", -100);
                            attackForward(new Vector2(36, 36), 6, 70);
                            //_sound.playSound("whsh");
                            _sound.playCue("whsh", emitter);
                        }
                        inCharge = false;

                        //sPlayer.
                    }));


            ctrl.Add("default",
                new State("default",
                    c => true,
                    c =>
                    {
                        UpdateDirection(inCharge && lockChargeDirection);
                        
                        WalkForward();
                        sPlayer = sPlayers["standing"];
                        UpdateAnimation();
                        inCharge = false;

                        UpdateItems();
                        //UpdateItemA();
                        //UpdateItemB();

                        _sound.stopCueInstance(id + "blast off", emitter);


                        if ((canAttackWhileOverheating || !overheating) && !busy)
                        {
                            if (controls.ActionPressed())
                            {
                                stats.addTo("boost", -70);
                                attackForward(new Vector2(36, 36), 5, 50);
                                //_sound.playSound("whsh");
                                _sound.playCue("whsh", emitter);
                            }

                            if (controls.ActionHeld())
                            {
                                inCharge = true;
                                chargeLevel++;

                                if (chargeLevel > 25)
                                    _sound.playCueInstance(id + "charging", emitter);

                                if (chargeLevel == 75)
                                    _sound.playCue("ping2", emitter);
                            }
                            else
                            {
                                if (chargeLevel >= 75)
                                {
                                    stats.addTo("boost", -170);
                                    attackForward(new Vector2(36, 36), 8, 200);
                                    _sound.playCue("smash", emitter);
                                }
                                else if (chargeLevel > 25)
                                {
                                    stats.addTo("boost", -120);
                                    attackForward(new Vector2(36, 36), 6, 100);
                                    _sound.playCue("smash", emitter);
                                }

                                _sound.stopCueInstance(id + "charging", emitter);
                                chargeLevel = 0;
                            }

                            //put below in item update method
                        }

                    }));

            sm = new StateMachine(ctrl);
        }

        private void CancelCharge()
        {
            chargeLevel = 0;
            itemAChargeLevel = 0;
            itemBChargeLevel = 0;

            inCharge = false;
        }

        private void UpdateItems()
        {
            UpdateItemA();
            UpdateItemB();
        }

        private void UpdateItemA()
        {
            if (stats.inventory.itemA == null || busy)
                return;

            if (controls.ItemAPressed())
            {
                stats.inventory.itemA.Press(this);
            }

            if (controls.ItemAHeld())
            {
                stats.inventory.itemA.Hold(this);
            }

            if (controls.ItemAReleased())
            {
                stats.inventory.itemA.Release(this);
            }
        }

        private void UpdateItemB()
        {
            if (stats.inventory.itemB == null || busy)
                return;

            if (controls.ItemBPressed())
            {
                stats.inventory.itemB.Press(this);
            }

            if (controls.ItemBHeld())
            {
                stats.inventory.itemB.Hold(this);
            }

            if (controls.ItemBReleased())
            {
                stats.inventory.itemB.Release(this);
            }

        }

        protected void UpdateDirection(bool strafe)
        {
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            if (magnitude == 0f)
                return;

            Direction temp = (Direction)
                        Helper.degreeToDirection((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));

            //strafe = controls.StrafeHeld();

            if (!strafe)
                facing = temp;

            movementDirection = (int)temp;
        }

        protected void UpdateAnimation()
        {
            //if (sPlayer == null)
                //return;

            switch (facing)
            {
                case Direction.East:
                    sPlayer.setAnimation("east", 0, 0);
                    break;

                case Direction.North:
                    sPlayer.setAnimation("north", 0, 0);
                    break;

                case Direction.South:
                    sPlayer.setAnimation("south", 0, 0);
                    break;

                case Direction.West:
                    sPlayer.setAnimation("west", 0, 0);
                    break;

                case Direction.NorthEast:
                    sPlayer.setAnimation("northeast", 0, 0);
                    break;

                case Direction.NorthWest:
                    sPlayer.setAnimation("northwest", 0, 0);
                    break;

                case Direction.SouthEast:
                    sPlayer.setAnimation("southeast", 0, 0);
                    break;

                case Direction.SouthWest:
                    sPlayer.setAnimation("southwest", 0, 0);
                    break;
            }
        }

        protected void WalkForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );

            if (inCharge)
                speed = walkSpeed;
            else
                speed = runSpeed;

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

        protected override void Dispose()
        {
            base.Dispose();

            _sound.removeCueInstance(id + "blast off");
            _sound.removeCueInstance(id + "charging");

        }
    }


}
