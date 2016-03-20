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
using wickedcrush.screen;
using wickedcrush.screen.menu;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.player
{
    public class PlayerAgent : Agent
    {
        #region Variables
        protected Controls controls;

        private float walkSpeed = 40f, runSpeed = 75f, fillSpeed = 3f;
        private bool inCharge = false, lockChargeDirection = false, canAttackWhileOverheating = true;
        private int chargeLevel = 0;

        public bool busy = false;

        public Player player;

        //pollable gameplay stuff:
        private bool dodgeSuccess = false;

        private Vector2 unitVector = Vector2.Zero;

        int standHeight = 0, maxHeight = 10, tempHeight = 0;
        
        protected bool targetLock = false;

        bool updateStats = false;

        #endregion

        #region Initialization
        public PlayerAgent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls, PersistedStats stats, EntityFactory factory, SoundManager sound, String name, Player player)
            : base(-1, w, pos, size, center, solid, factory, stats, sound)
        {
            this.player = player;
            Initialize(name, pos, size, center, solid, controls);

            this.stats = stats;

            applyStats();

            if (controls is GamepadControls)
                factory._game.settings.controlMode = utility.config.ControlMode.Gamepad;
        }

        private void Initialize(String name, Vector2 pos, Vector2 size, Vector2 center, bool solid, Controls controls)
        {
            this.controls = controls;
            this.name = name;

            this.facing = Direction.East;
            movementDirection = (int)facing;
            aimDirection = (int)facing;

            //timers.Add("boostRecharge", new utility.Timer(stats.get("boostRecharge")));
            //timers.Add("boostLift", new utility.Timer(stats.get("boostRecharge")));
            timers.Add("boostRecharge", new utility.Timer(150));
            timers.Add("boostLift", new utility.Timer(stats.getNumber("boostRecharge")));
            timers["boostRecharge"].resetAndStart();
            timers["boostLift"].resetAndStart();

            timers.Add("iFrameTime", new utility.Timer(stats.getNumber("iFrameTime")));
            timers["iFrameTime"].resetAndStart();



            _sound.addCueInstance("blast off", id + "blast off", false);
            _sound.addCueInstance("charging", id + "charging", false);
            _sound.addCueInstance("VP_Jet1", id + "VP_Jet1", true);
            SetupStateMachine();

            //InitializeHpBar();
            //UpdateHpBar();

            killValue = 0;

            AddLight(new PointLightStruct(new Vector4(1f, 0.65f, 0.5f, 1f), 0.9f, new Vector4(1f, 0.65f, 0.5f, 1f), 0f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 700f));
            //factory._gm.scene.AddLight(light);

            //factory._gm.cursor.SetPlayerPos(this.pos + this.center);
        }

        

        

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();
            sPlayers.Add("you_pink", new SpriterPlayer(factory._spriterManager.spriters["you_pink"].getSpriterData(), 0, factory._spriterManager.spriters["you_pink"].loader));
            bodySpriter = sPlayers["you_pink"];
            bodySpriter.setFrameSpeed(20);
            drawBody = false;

            sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
            shadowSpriter = sPlayers["shadow"];
            shadowSpriter.setAnimation("still", 0, 0);
            shadowSpriter.setFrameSpeed(20);
            drawShadow = true;

            InitializeHumanoidSprites(1f, 1f, 1f, 1f, 1f, 1f);
        }

        private void applyStats()
        {
            stats.ApplyStats();

            physicalDMG = stats.inventory.gear.GetGearStat(GearStat.PhysicalDMG) * 5;
            etheralDMG = stats.inventory.gear.GetGearStat(GearStat.EtheralDMG) * 5;
            potency = stats.inventory.gear.GetGearStat(GearStat.Potency);

            boostSpeed = 100f + ((float)stats.getNumber("boostSpeedMod") * (15f));
            fillSpeed = 4f + ((float)stats.getNumber("fillSpeed") * 2f);
        }

        public override void TakeHit(Attack attack)
        {
            if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
            {
                dodgeSuccess = true;

                int hpConversion = stats.getNumber("HPConversion");
                int epConversion = stats.getNumber("EPConversion");

                stats.addTo("hp", hpConversion);
                stats.addTo("boost", epConversion);

                if (hpConversion > 0)
                {
                    factory.addText("+" + hpConversion.ToString(), pos + center - new Vector2(-50, 0), 1000, Color.Fuchsia, ((float)hpConversion) / 100f, new Vector2(0f, 1f));
                }

                if (epConversion > 0)
                {
                    factory.addText("+" + epConversion.ToString(), pos + center - new Vector2(50, 0), 1000, Color.Green, ((float)epConversion) / 100f, new Vector2(0f, 1f));
                }

                stats.EnforceMaxStats();

                timers["iFrameTime"].resetAndStart();
                _sound.playCue("ping3", emitter);
            }
            else
            {
                base.TakeHit(attack);
                _sound.playCue("oof", emitter);
                factory._gm.activateFreezeFrame();
            }
        }

        public override void TakeSkill(action.ActionSkill action)
        {
            //float dmgAmount = 0f;
            if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
            {
                dodgeSuccess = true;

                int hpConversion = stats.getNumber("HPConversion");
                int epConversion = stats.getNumber("EPConversion");

                stats.addTo("hp", hpConversion);
                stats.addTo("boost", epConversion);

                if (hpConversion > 0)
                {
                    factory.addText("+" + hpConversion.ToString(), pos + center - new Vector2(-10, -20), 200, Color.LightSeaGreen, ((float)hpConversion) / 100f, new Vector2(0f, 1f));
                }

                if (epConversion > 0)
                {
                    factory.addText("+" + epConversion.ToString(), pos + center - new Vector2(10, -20), 200, Color.AliceBlue, ((float)epConversion) / 100f, new Vector2(0f, 1f));
                }

                stats.EnforceMaxStats();

                timers["iFrameTime"].resetAndStart();
                _sound.playCue("ping3", emitter);
                action.StealParent(this);
            }
            else
            {
                //ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-1.5f, 3f, -1.5f), new Vector3(3f, 3f, 3f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, 12, "all", 3, "hit", dmgAmount / 16f);
                //particleEmitter.EmitParticles(ps, this.factory, 3);

                FlashColor(Color.Red, 240);

                factory._gm.activateFreezeFrame();

                base.TakeSkill(action);
            }
            
        }

        public bool pollDodgeSuccess()
        {
            if (dodgeSuccess)
            {
                factory.addText("Perfect Dodge!", pos + center, 1000);
                ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), Vector3.Zero, new Vector3(-2f, -0.3f, -2f), new Vector3(4f, 0.6f, 4f), new Vector3(0, 0, 0), 0f, 0f, 1000, 32, "particles", 0, "white_to_yellow");
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

            if(updateStats)
            {
                applyStats();
                updateStats = false;
            }

            if (stats.compare("boost", "maxBoost") == -1 && weaponInUse == null && !controls.BoostHeld() && !controls.ReverseBoostHeld())
                stats.addTo("boost", (int)fillSpeed);

            if (stats.compare("boost", "maxBoost") == 1)
                stats.set("boost", stats.getNumber("maxBoost"));

            if (stats.compare("hp", "MaxHP") == 1)
                stats.set("hp", stats.getNumber("MaxHP"));

            if (stats.getNumber("boost") > 0)
            {
                overheating = false;
                targetSpeed = runSpeed;
            }
            else
            {
                targetSpeed = walkSpeed;
                //walkSpeed
            }

            if (stats.getNumber("boost") <= 0)
            {
                if (overheating == false)
                {
                    factory.addText("Overheating!!", this.pos, 600);
                    overheating = true;
                    if (weaponInUse != null)
                    {
                        weaponInUse.Release(this);
                    }
                }
                
                particleEmitter.EmitParticles(ParticleServer.GenerateSmoke(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y)), factory, 1);
                
                //stats.set("boost", 0);
            }

            //UpdateHpBar();
            UpdateAnimation();

            
            

            if (factory._game.settings.controlMode == utility.config.ControlMode.MouseAndKeyboard)
            {
                PollCursor();
            }

            if (controls.LaunchMenuPressed() && weaponInUse == null)
            {
                LaunchMenu();
            }
            //factory._game.diag += "Player Pos: " + pos.X + ", " + pos.Y + "\n";
        }

        private void PollCursor()
        {
            factory._gm.cursor.SetPlayerPos(this.pos + this.center);

            //if (target == null)
            //{
                //targetLock = false;
                //factory._gm.cursor.targetLock = false;

                //if (controls.LockOnPressed() && factory._gm.cursor.cursorTarget != null)
                //{
                    //target = factory._gm.cursor.cursorTarget;
                    //targetLock = true;
                    //factory._gm.cursor.targetLock = true;
                //}

            //}
            //else
            //{
                //if (controls.LockOnPressed())
                //{
                    //targetLock = false;
                    //factory._gm.cursor.targetLock = false;
                    //target = null;
                //}
            //}

            if (targetLock)
            {
                faceEntity(target);
                aimDirection = Helper.degreeConversion(angleToEntity(target));
            }
            else
            {
                faceEntity(factory._gm.cursor);
                aimDirection = Helper.degreeConversion(angleToEntity(factory._gm.cursor));
            }
        }

        private void SetupStateMachine()
        {
            Dictionary<String, State> ctrl = new Dictionary<String, State>();

            ctrl.Add("falling",
                new State("falling",
                    c => timers["falling"].isActive() || timers["falling"].isDone(),
                    c =>
                    {
                        this.height-=3;
                    }));

            ctrl.Add("boosting",
                new State("boosting",
                    c => timers["boostRecharge"].isDone() 
                        && controls.BoostHeld()
                        && !overheating
                        && !busy,
                    c =>
                    {
                        ParticleStruct ps;
                        if (stateTree.previousControlState != null && !stateTree.previousControlState.name.Contains("boosting"))
                        {
                            timers["boostLift"].resetAndStart();
                            if(timers["iFrameTime"].isDone())
                                timers["iFrameTime"].resetAndStart();
                            //_sound.playCueInstance(id + "blast off", emitter);
                            _sound.playCue("blast off");
                            _sound.playCueInstance(id + "VP_Jet1", emitter);
                            tempHeight = height;

                            SetCostume(1);

                        }

                        this.height = standHeight + (int)(Math.Sin(timers["boostLift"].getPercent() * Math.PI / 2.0) * maxHeight);


                        UpdateDirection(false);
                        BoostForward();
                        
                        UpdateHumanoidSpriteOffsets(1f, 1f, 1f, 1f, 2f, 3f);
                        stats.addTo("boost", -stats.getNumber("useSpeed"));

                        UpdateItems();

                        
                        inCharge = false;

                        if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
                        {
                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, 20, this.pos.Y + this.center.Y), Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, 0, 0, 1000, "particles", 2, "red_small");
                            //particleEmitter.EmitParticles(ps, this.factory, 1);

                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, this.height, this.pos.Y + this.center.Y - unitVector.Y), new Vector3(0f, 0f, 0f), new Vector3(-unitVector.X * 3, 1f, -unitVector.Y * 3), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), -driftDirection + 210, 0f, 1000, 6, "particles", 0, "white_to_orange", boostSpeed / 1000f, boostSpeed / 1000f);
                            
                            //CreateParticleClone(50, factory);
                        }
                        else
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, this.height, this.pos.Y + this.center.Y - unitVector.Y), new Vector3(0f, 0f, 0f), new Vector3(-unitVector.X * 3, 1f, -unitVector.Y * 3), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), -driftDirection + 210, 0f, 1000, 6, "particles", 0, "pink_to_red", boostSpeed / 1000f, boostSpeed / 1000f);
                            
                            factory._gm.camera.ShakeScreen(1f);
                            SetCostume(0);
                        }

                        particleEmitter.EmitParticles(ps, this.factory, 1);

                        factory._gm.camera.targetLooseness = 5f;
                        
                        
                    }));

            ctrl.Add("reverse_boosting",
                new State("reverse_boosting",
                    c => timers["boostRecharge"].isDone() 
                    && controls.ReverseBoostHeld()
                    && !overheating
                    && !busy,
                    c =>
                    {
                        ParticleStruct ps;
                        if (stateTree.previousControlState != null && !stateTree.previousControlState.name.Contains("boosting"))
                        {
                            timers["boostLift"].resetAndStart();
                            if (timers["iFrameTime"].isDone())
                                timers["iFrameTime"].resetAndStart();
                            //_sound.playCueInstance(id + "blast off", emitter);
                            _sound.playCue("blast off");
                            _sound.playCueInstance(id + "VP_Jet1", emitter);
                            tempHeight = height;
                            SetCostume(1);

                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_yellow");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                            //ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X - unitVector.X, 0, this.pos.Y + this.center.Y - unitVector.Y), Vector3.Zero, new Vector3(-0.6f, 1f, -0.6f), new Vector3(1.2f, 1f, 1.2f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_orange");
                            //particleEmitter.EmitParticles(ps, this.factory, 5);
                        }

                        this.height = standHeight + (int)(Math.Sin(timers["boostLift"].getPercent() * Math.PI / 2.0) * maxHeight);

                        //if (((PlayerAgent)c).controls.ReverseBoostPressed())
                            //timers["boostRecharge"].resetAndStart();

                        UpdateDirection(false);
                        BoostBackward();
                        
                        UpdateHumanoidSpriteOffsets(1f, 1f, 1f, 1f, 0.5f, -0.75f);
                        stats.addTo("boost", -stats.getNumber("useSpeed"));

                        UpdateItems();


                        inCharge = false;

                        if (timers["iFrameTime"].isActive() && !timers["iFrameTime"].isDone())
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X + unitVector.X, this.height, this.pos.Y + this.center.Y + unitVector.Y), new Vector3(0f, 0f, 0f), new Vector3(unitVector.X, 1f, unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), driftDirection + 180f, 0f, 1000, 6, "particles", 0, "white_to_orange", boostSpeed / 1000f, boostSpeed / 1000f);
                            //CreateParticleClone(50, factory);
                        }
                        else
                        {
                            ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X + unitVector.X, height, this.pos.Y + this.center.Y + unitVector.Y), new Vector3(0f, 0f, 0f), new Vector3(unitVector.X, 1f, unitVector.Y), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0, -.03f, 0), driftDirection + 180f, 0f, 1000, 6, "particles", 0, "pink_to_red", boostSpeed / 1000f, boostSpeed / 1000f);
                            factory._gm.camera.ShakeScreen(1f);
                            SetCostume(0);
                        }
                        particleEmitter.EmitParticles(ps, this.factory, 1);

                        factory._gm.camera.targetLooseness = 5f;
                        

                    }));


            ctrl.Add("default",
                new State("default",
                    c => true,
                    c =>
                    {
                        if (stateTree.previousControlState != null && stateTree.previousControlState.name.Contains("boosting"))
                        {
                            timers["boostRecharge"].resetAndStart();
                            tempHeight = height;
                            SetCostume(0);
                        }

                        this.height = standHeight + (int)(Math.Cos(timers["boostRecharge"].getPercent() * Math.PI / 2.0) * tempHeight);

                        //if (timers["boostRecharge"].isDone() && timers["boostRecharge"].isActive())
                        //{
                          //  _sound.playCue("Hit_Hurt20");
                        //}

                        UpdateDirection(inCharge && lockChargeDirection);
                        UpdateHumanoidSpriteOffsets(1f, 1f, 1f, 1f, 1f, 1f);

                        if (height > 0)
                        {
                            AirDrift();
                        }
                        else
                        {
                            WalkForward();
                        }

                        //WalkForward();
                        
                        inCharge = false;

                        UpdateItems();

                        this.facing = Helper.constrainDirection((Direction)aimDirection);

                        _sound.stopCueInstance(id + "blast off");
                        _sound.stopCueInstance(id + "VP_Jet1");

                        factory._gm.camera.targetLooseness = 50f;
                        
                        

                    }));

            stateTree = new StateTree();
            stateTree.AddBranch("default", new StateBranch(c => true, ctrl));
        }

        private void UpdateItems()
        {
            if (stats.inventory.equippedWeapon != null)
            {
                if (weaponInUse == null && controls.WeaponScrollUp())
                {
                    stats.inventory.prevWeapon(this);
                    factory.addText(stats.inventory.equippedWeapon.name, this.pos, 300);
                    _sound.playCue("abraisive laser");
                }
                else if (weaponInUse == null && controls.WeaponScrollDown())
                {
                    stats.inventory.nextWeapon(this);
                    factory.addText(stats.inventory.equippedWeapon.name, this.pos, 300);
                    _sound.playCue("abraisive laser");
                }

                UpdateEquippedWeapon();
            }
            //UpdateItemB();
            //UpdateItemC();
        }

        private void UpdateEquippedWeapon()
        {
            if (stats.inventory.equippedWeapon == null || busy)
                return;

            if (controls is KeyboardControls && factory._game.settings.controlMode == utility.config.ControlMode.MouseAndKeyboard)
            {
                if ( ((KeyboardControls)controls).LeftMousePress())
                {
                    stats.inventory.equippedWeapon.Press(this);
                }

                if ( ((KeyboardControls)controls).LeftMouseHold())
                {
                    stats.inventory.equippedWeapon.Hold(this);
                }

                if ( ((KeyboardControls)controls).LeftMouseRelease())
                {
                    stats.inventory.equippedWeapon.Release(this);
                }
            }
            else
            {
                if (controls.WeaponPressed())
                {
                    stats.inventory.equippedWeapon.Press(this);
                }

                if (controls.WeaponHeld())
                {
                    stats.inventory.equippedWeapon.Hold(this);
                }

                if (controls.WeaponReleased())
                {
                    stats.inventory.equippedWeapon.Release(this);
                }
            }



        }

        protected void UpdateDirection(bool strafe)
        {
            float lStickMagnitude = Math.Max(Math.Abs(controls.LStickYAxis()), Math.Abs(controls.LStickXAxis()));
            float rStickMagnitude = Math.Max(Math.Abs(controls.RStickYAxis()), Math.Abs(controls.RStickXAxis()));
            if (rStickMagnitude > 0f)
            {
                aimDirection = (int)MathHelper.ToDegrees((float)Math.Atan2(-controls.RStickYAxis(), controls.RStickXAxis()));
                facing = Helper.constrainDirection((Direction)aimDirection);
            }
            else if (!strafe && (factory._game.settings.controlMode != utility.config.ControlMode.MouseAndKeyboard) && lStickMagnitude > 0f)
            {
                aimDirection = movementDirection;
                facing = Helper.constrainDirection((Direction)aimDirection);
            }

            

            if (lStickMagnitude == 0f)
                return;

            Direction temp = (Direction)
                        Helper.radiansToDirection((float)Math.Atan2(controls.LStickYAxis(), controls.LStickXAxis()));


            if (!strafe)
                facing = temp;

            movementDirection = (int)temp;



            
        }

        protected void UpdateAnimation()
        {
            
            //if (sPlayer == null)
                //return;

            String bad = "";

            if (timers["falling"].isActive() || timers["falling"].isDone())
            {
                bodySpriter.setAnimation("fall", 0, 0);
                drawShadow = false;
                return;
            }

            drawShadow = true;

            switch (facing)
            {
                case Direction.East:
                    bad += "east";
                    bodySpriter.setFlipX(1);
                    //bodySpriter.setAnimation("east", 0, 0);
                    break;

                case Direction.North:
                    bad += "north";
                    bodySpriter.setFlipX(1);
                    //bodySpriter.setAnimation("north", 0, 0);
                    break;

                case Direction.South:
                    bad += "south";
                    bodySpriter.setFlipX(1);
                    //bodySpriter.setAnimation("south", 0, 0);
                    break;

                case Direction.West:
                    bad += "east";
                    bodySpriter.setFlipX(-1);
                    //bodySpriter.setAnimation("west", 0, 0);
                    break;

                case Direction.NorthEast:
                    bad += "northwest";
                    bodySpriter.setFlipX(-1);
                    //bodySpriter.setAnimation("northeast", 0, 0);
                    break;

                case Direction.NorthWest:
                    bad += "northwest";
                    bodySpriter.setFlipX(1);
                    //bodySpriter.setAnimation("northwest", 0, 0);
                    break;

                case Direction.SouthEast:
                    bad += "southeast";
                    bodySpriter.setFlipX(1);
                    //bodySpriter.setAnimation("southeast", 0, 0);
                    break;

                case Direction.SouthWest:
                    bad += "southeast";
                    bodySpriter.setFlipX(-1);
                    //bodySpriter.setAnimation("southwest", 0, 0);
                    break;
            }

            if (bodies["body"].LinearVelocity.Length() >= 1f)
            {
                bad += "_run";
                shadowSpriter.setAnimation("moving", 0, 0);
                targetBobAmount = 1f;
                //if(!timers["bob"].isActive())
                //{
                    //timers["bob"].resetAndStart();
                //}
            }
            else
            {
                bad += "_stand";
                shadowSpriter.setAnimation("still", 0, 0);
                targetBobAmount = 0f;
                //if (timers["bob"].isActive())
                //{
                    //timers["bob"].reset();
                //}
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

            //if (inCharge)
                //speed = walkSpeed;
            //else
                //speed = runSpeed;

            v += (unitVector * magnitude * speed * startingFriction);

            bodies["body"].LinearVelocity += v;

            if(airborne)
                _sound.playCue("VP_Jet2");

            airborne = false;
        }

        protected void AirDrift()
        {
            Vector2 v = bodies["body"].LinearVelocity;

            //driftDirection = Helper.GetDegreesFromVector();

            //Vector2 unitVector = new Vector2(
                //(float)Math.Cos(MathHelper.ToRadians(driftDirection)),
                //(float)Math.Sin(MathHelper.ToRadians(driftDirection))
            //);

            if (v.LengthSquared() < 1e-8)
                v = Vector2.Zero;
            else
                v.Normalize();

            v *= boostSpeed;

            bodies["body"].LinearVelocity += v;

            airborne = true;
        }

        
        #endregion

        public bool InteractPressed()
        {
            return controls.InteractPressed();
        }

        public override void TakeKnockback(Vector2 pos, int dmg, float force)
        {
            base.TakeKnockback(pos, dmg, force);

            factory._gm.activateFreezeFrame();
        }

        public override void TakeKnockback(Vector2 pos, float force)
        {
            base.TakeKnockback(pos, force);

            factory._gm.activateFreezeFrame();
        }

        public override void TakeHit(Attack attack, bool knockback)
        {
            base.TakeHit(attack, knockback);

            factory._gm.activateFreezeFrame();
        }

        protected override void Dispose()
        {
            base.Dispose();
            _sound.stopCueInstance(id + "blast off");
            _sound.stopCueInstance(id + "VP_Jet1");
            _sound.stopCueInstance(id + "charging");
            _sound.removeCueInstance(id + "blast off");
            _sound.removeCueInstance(id + "charging");
            _sound.removeCueInstance(id + "VP_Jet1");

        }

        protected void LaunchMenu()
        {
            factory._game.screenManager.AddScreen(new GameplayMenuScreen(factory._game, factory._gm, player));
            updateStats = true;
        }
    }


}
