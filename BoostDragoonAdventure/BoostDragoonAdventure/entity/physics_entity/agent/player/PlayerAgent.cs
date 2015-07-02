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
using Microsoft.Xna.Framework.Audio;
using wickedcrush.inventory;
using wickedcrush.player;
using Com.Brashmonkey.Spriter.player;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;

        private float walkSpeed = 40f, runSpeed = 75f, boostSpeed = 120f;
        private bool overheating = false, inCharge = false, lockChargeDirection = false, canAttackWhileOverheating = true;
        private int chargeLevel = 0;

        public bool busy = false;

        public Player player;

        //pollable gameplay stuff:
        private bool dodgeSuccess = false;

        private Vector2 unitVector = Vector2.Zero;

        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, PersistedStats stats, EntityFactory factory, SoundManager sound, String name, Player player)
            : base(w, pos, size, center, solid, factory, stats, sound)
        {
            this.player = player;
            Initialize(name, pos, size, center, solid, controls);

            this.stats = stats;
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

            //stats.inventory.itemA = ItemServer.getItem("Healthsweed");
            //stats.inventory.itemB = ItemServer.getItem("Spellbook: Fireball");

            _sound.addCueInstance("blast off", id + "blast off", false);
            _sound.addCueInstance("charging", id + "charging", false);
            SetupStateMachine();

            InitializeHpBar();
            UpdateHpBar();
        }

        

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you", new SpriterPlayer(factory._spriterManager.spriters["you"].getSpriterData(), 0, factory._spriterManager.spriters["you"].loader));
            bodySpriter = sPlayers["you"];
            bodySpriter.setFrameSpeed(20);

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;
        }

        private void applyStats()
        {
            boostSpeed = 100f * (1 + (float)stats.get("boostSpeedMod") * (1f));
        }

        public override void TakeHit(Attack attack)
        {
            if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
            {
                dodgeSuccess = true;
                stats.addTo("boost", 300);
                timers["iFrameTime"].resetAndStart();
                _sound.playCue("ping3", emitter);
            }
            else
            {
                base.TakeHit(attack);
                _sound.playCue("oof", emitter);
            }
        }

        public override void TakeSkill(action.ActionSkill action)
        {
            if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
            {
                dodgeSuccess = true;
                stats.addTo("boost", 300);
                timers["iFrameTime"].resetAndStart();
                _sound.playCue("ping3", emitter);
            }
            else
            {
                base.TakeSkill(action);
            }
            
        }

        public bool pollDodgeSuccess()
        {
            if (dodgeSuccess)
            {
                factory.addText("Perfect Dodge!", pos + center, 1000);
                ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, 0, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-2f, -0.3f, -2f), new Vector3(4f, 0.6f, 4f), new Vector3(0, 0, 0), 0f, 0f, 1000, "particles", 0, "white_to_yellow");
                particleEmitter.EmitParticles(ps, this.factory, 10);
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
            if (remove)
                return;

            if (stats.compare("boost", "maxBoost") == -1)
                stats.addTo("boost", stats.get("fillSpeed"));

            if (stats.compare("boost", "maxBoost") == 1)
                stats.set("boost", stats.get("maxBoost"));

            if (stats.get("boost") >= 200)
            {
                overheating = false;
                //stats.set("boost", stats.get("maxBoost"));
            }

            if (stats.get("boost") <= 0)
            {
                factory.addText("Overheating!!", this.pos, 600);
                overheating = true;
                stats.set("boost", 0);
            }

            UpdateHpBar();
            UpdateAnimation();
            applyStats();

            
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();

            ctrl.Add("falling",
                new State("falling",
                    c => ((PlayerAgent)c).timers["falling"].isActive(),
                    c =>
                    {
                        this.height-=3;
                    }));

            ctrl.Add("boosting",
                new State("boosting",
                    c => !((PlayerAgent)c).timers["boostRecharge"].isDone()
                        && ((PlayerAgent)c).timers["boostRecharge"].isActive()
                    || ((PlayerAgent)c).controls.BoostHeld()
                    && !((PlayerAgent)c).overheating
                    && !((PlayerAgent)c).busy,
                    c =>
                    {
                        ParticleStruct ps;
                        if (stateTree.previousControlState != null && stateTree.previousControlState.name != "boosting")
                        {
                            timers["iFrameTime"].resetAndStart();
                            _sound.playCueInstance(id + "blast off", emitter);

                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_yellow");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_orange");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                        }

                        //if (((PlayerAgent)c).controls.BoostPressed())
                            //timers["boostRecharge"].resetAndStart();

                        UpdateDirection(false);
                        BoostForward();
                        stats.addTo("boost", -stats.get("useSpeed"));

                        UpdateItems();

                        
                        inCharge = false;

                        if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
                        {
                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, 20, this.pos.Y + this.center.Y), Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, 0, 0, 1000, "particles", 2, "red_small");
                            //particleEmitter.EmitParticles(ps, this.factory, 1);

                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-unitVector.X, 1f, -unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, "particles", 0, "white_to_orange");
                            particleEmitter.EmitParticles(ps, this.factory, 1);
                        }
                        else
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-unitVector.X, 1f, -unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, "particles", 0, "pink_to_red");
                            particleEmitter.EmitParticles(ps, this.factory, 1);
                            factory._gm.camera.ShakeScreen(1f);
                        }

                        factory._gm.camera.targetLooseness = 5f;
                        
                        
                    }));

            ctrl.Add("reverse_boosting",
                new State("reverse_boosting",
                    c => !((PlayerAgent)c).timers["boostRecharge"].isDone()
                        && ((PlayerAgent)c).timers["boostRecharge"].isActive()
                    || ((PlayerAgent)c).controls.ReverseBoostHeld()
                    && !((PlayerAgent)c).overheating
                    && !((PlayerAgent)c).busy,
                    c =>
                    {
                        ParticleStruct ps;
                        if (stateTree.previousControlState != null && stateTree.previousControlState.name != "reverse_boosting")
                        {
                            timers["iFrameTime"].resetAndStart();
                            _sound.playCueInstance(id + "blast off", emitter);

                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_yellow");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_orange");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                        }

                        //if (((PlayerAgent)c).controls.ReverseBoostPressed())
                            //timers["boostRecharge"].resetAndStart();

                        UpdateDirection(false);
                        BoostBackward();
                        stats.addTo("boost", -stats.get("useSpeed"));

                        UpdateItems();


                        inCharge = false;

                        if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X + unitVector.X, 0, this.pos.Y + this.center.Y + unitVector.Y), Vector3.Zero, new Vector3(unitVector.X, 1f, unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, "particles", 0, "white_to_orange");
                        }
                        else
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X + unitVector.X, 0, this.pos.Y + this.center.Y + unitVector.Y), Vector3.Zero, new Vector3(unitVector.X, 1f, unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), 0f, 0f, 1000, "particles", 0, "pink_to_red");
                            factory._gm.camera.ShakeScreen(1f);
                        }
                        particleEmitter.EmitParticles(ps, this.factory, 1);

                        factory._gm.camera.targetLooseness = 5f;
                        

                    }));


            ctrl.Add("default",
                new State("default",
                    c => true,
                    c =>
                    {
                        UpdateDirection(inCharge && lockChargeDirection);
                        
                        WalkForward();
                        //bodySpriter = sPlayers["standing"];
                        
                        inCharge = false;

                        UpdateItems();
                        

                        _sound.stopCueInstance(id + "blast off");

                        factory._gm.camera.targetLooseness = 50f;
                        

                    }));

            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        private void UpdateItems()
        {
            UpdateItemA();
            UpdateItemB();
            UpdateItemC();
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

        private void UpdateItemC()
        {
            if (stats.inventory.itemC == null || busy)
                return;

            if (controls.ItemCPressed())
            {
                stats.inventory.itemC.Press(this);
            }

            if (controls.ItemCHeld())
            {
                stats.inventory.itemC.Hold(this);
            }

            if (controls.ItemCReleased())
            {
                stats.inventory.itemC.Release(this);
            }

        }

        protected void UpdateDirection(bool strafe)
        {
            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            if (magnitude == 0f)
                return;

            Direction temp = (Direction)
                        Helper.radiansToDirection((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));

            //strafe = controls.StrafeHeld();

            if (!strafe)
                facing = temp;

            movementDirection = (int)temp;
        }

        protected void UpdateAnimation()
        {
            
            //if (sPlayer == null)
                //return;

            String bad = "";

            if (timers["falling"].isActive())
            {
                bodySpriter.setAnimation("fall_000", 0, 0);
                drawShadow = false;
                return;
            }

            drawShadow = true;

            switch (facing)
            {
                case Direction.East:
                    bad += "east";
                    
                    //bodySpriter.setAnimation("east", 0, 0);
                    break;

                case Direction.North:
                    bad += "north";
                    //bodySpriter.setAnimation("north", 0, 0);
                    break;

                case Direction.South:
                    bad += "south";
                    //bodySpriter.setAnimation("south", 0, 0);
                    break;

                case Direction.West:
                    bad += "west";
                    //bodySpriter.setAnimation("west", 0, 0);
                    break;

                case Direction.NorthEast:
                    bad += "northeast";
                    //bodySpriter.setAnimation("northeast", 0, 0);
                    break;

                case Direction.NorthWest:
                    bad += "northwest";
                    //bodySpriter.setAnimation("northwest", 0, 0);
                    break;

                case Direction.SouthEast:
                    bad += "southeast";
                    //bodySpriter.setAnimation("southeast", 0, 0);
                    break;

                case Direction.SouthWest:
                    bad += "southwest";
                    //bodySpriter.setAnimation("southwest", 0, 0);
                    break;
            }

            if (bodies["body"].LinearVelocity.Length() >= 1f)
            {
                bad += "_run_000";
                shadowSpriter.setAnimation("moving", 0, 0);
            }
            else
            {
                bad += "_stand_000";
                shadowSpriter.setAnimation("still", 0, 0);
            }

            bodySpriter.setAnimation(bad, 0, 0);
        }

        protected void WalkForward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            float magnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));

            unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection))
            );

            if (inCharge)
                speed = walkSpeed;
            else
                speed = runSpeed;

            v += unitVector * magnitude * speed * startingFriction;

            bodies["body"].LinearVelocity += v;

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

            bodies["body"].LinearVelocity += v;

            airborne = true;
        }

        protected void BoostBackward()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            Vector2 unitVector = new Vector2(
                (float)Math.Cos(MathHelper.ToRadians((float)movementDirection + 180)),
                (float)Math.Sin(MathHelper.ToRadians((float)movementDirection + 180))
            );

            v += unitVector * boostSpeed;

            bodies["body"].LinearVelocity += v;

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
            _sound.stopCueInstance(id + "blast off");
            _sound.stopCueInstance(id + "charging");
            _sound.removeCueInstance(id + "blast off");
            _sound.removeCueInstance(id + "charging");

        }
    }


}
