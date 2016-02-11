using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework;
using wickedcrush.utility;
using Com.Brashmonkey.Spriter.player;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using wickedcrush.particle;
using wickedcrush.helper;
using wickedcrush.display._3d;

namespace wickedcrush.entity.physics_entity.agent.action
{
    public class ActionSkill : Agent
    {
        String skillName;

        int duration;

        public List<KeyValuePair<String, int>> statIncrement;
        List<KeyValuePair<Timer, SkillStruct>> blows = new List<KeyValuePair<Timer, SkillStruct>>();

        public KeyValuePair<int, int> force; //direction, force amount

        protected bool reactToWall, piercing = false, ignoreSameParent = true, just_for_show = false, followParent = false, aimed=false, hitConnected = false, bounce = true;

        private Vector2 velocity;

        //int aimDirection;

        GameplayManager gameplay;

        String cue;

        public SkillStruct skillStruct;

        bool rootSkill = false;

        bool wallCollision = false;

        int maxHeight = 25, tempHeight = 0;


        public ActionSkill(SkillStruct skillStruct, GameBase game, GameplayManager gameplay, Entity parent, Entity actingParent)
            : base(-1, gameplay.w,
            new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange))),
                        (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)))), 
            skillStruct.size, 
            skillStruct.center,
            !skillStruct.followParent, 
            gameplay.factory, game.soundManager) 
        {
            //this.aimDireciton = parent.movementDirection + skillStruct.directionChange;
            this.skillStruct = skillStruct;
            this.duration = skillStruct.duration;
            timers.Add("duration", new utility.Timer(duration));
            timers["duration"].resetAndStart();

            timers.Add("particle_emission", new utility.Timer(10));
            timers["particle_emission"].resetAndStart();

            skillName = skillStruct.name;

            piercing = skillStruct.piercing;

            if(parent!=null)
                height = parent.skillHeight;
            
            if (null != actingParent)
            {
                this.parent = actingParent;
            } 
            else
            {
                this.parent = parent;
                rootSkill = true;
            }
            this.facing = parent.facing;
            this.movementDirection = parent.aimDirection + skillStruct.directionChange;
            this.aimDirection = parent.aimDirection + skillStruct.directionChange;
            this.gameplay = gameplay;

            

            this.statIncrement = skillStruct.statIncrement;

            this.force = new KeyValuePair<int, int>((int)this.facing, skillStruct.force);

            velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                (float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));

            LoadBlows(skillStruct.blows, gameplay);

            cue = skillStruct.cue;

            this.followParent = skillStruct.followParent;

            if (followParent)
                reactToWall = false;
            else
            {
                reactToWall = true;
                this.bounce = skillStruct.bounce;
            }

            Initialize();
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            //FixtureFactory.AttachCircle(size.X / 2, 1f, bodies["body"], center);
            //bodies["body"].FixedRotation = true;
            //bodies["body"].LinearVelocity = Vector2.Zero;
            //bodies["body"].BodyType = BodyType.Dynamic;
            //bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;

            //bodies["body"].UserData = this;

            //if (!solid)
                //bodies["body"].IsSensor = false;




        }

        protected void LoadBlows(List<KeyValuePair<int, SkillStruct>> blows, GameplayManager gameplay)
        {
            foreach (KeyValuePair<int, SkillStruct> blow in blows)
            {
                Timer t = new Timer(blow.Key);
                t.resetAndStart();
                this.blows.Add(new KeyValuePair<Timer, SkillStruct>(t, blow.Value));
            }
        }

        private void Initialize()
        {
            airborne = true;
            immortal = true;
            this.name = "ActionSkill";

            AddLight(new PointLightStruct(new Vector4(1f, 0.3f, 0.3f, 1f), 0.4f, new Vector4(1f, 0.5f, 0.5f, 1f), 0.8f, new Vector3(pos.X + center.X, 30f, pos.Y + center.Y), 50f));


            if (cue != "")
            {
                _sound.playCue(cue, parent.emitter); // play activate sound
            }

            SetupActionSkillSpriter(this.skillStruct);


            //_sound.addCueInstance("hurt", id + "hurt", false);
            
        }

        public void ReInitialize(SkillStruct skillStruct, GameBase game, GameplayManager gameplay, Entity parent, Entity actingParent)
        {
            this.skillStruct = skillStruct;
            
            hitThisTick = false;
            remove = false;
            dead = false;
            reactToWall = true; piercing = false; ignoreSameParent = true; just_for_show = false; followParent = false; aimed = false; hitConnected = false; bounce = true;
            //Agent Initialize:
            this.kid = -1;
            this.pos = new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange))),
                        (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange))));
            this.size = skillStruct.size;
            this.center = skillStruct.center;
            initialPosition = pos;
            
            _spriterManager = factory._spriterManager;
            
            //Initialize(new PersistedStats(5, 5), factory);
            //this.factory = factory;
            //this.stats = stats;

            //stats.set("staggerDistance", 0);

            timers.Clear();
            triggers.Clear();
            proximity.Clear();
            hudSpriters.Clear();
            angledSpriters.Clear();
            blows = new List<KeyValuePair<Timer, SkillStruct>>();
            this.name = "Agent";

            //this.particleEmitter = new ParticleEmitter(factory._particleManager);

            SetupSpriterPlayer();

            timers.Add("falling", new Timer(500));
            timers["falling"].reset(); //???

            timers.Add("bob", new Timer(bobTimer));
            timers["bob"].resetAndStart();
            timers["bob"].autoLoop = true;

            skillHeight = 15;

            //PhysicsEntity Initialize:
            _w = gameplay.w;
            //setupBody(_w, pos, skillStruct.size,
                        //skillStruct.center, !skillStruct.followParent);
            reCreateBody(_w, pos, 
                        skillStruct.size, 
                        skillStruct.center, !skillStruct.followParent);

            //ActionSkill Initialize:
            //this.skillStruct = skillStruct;
            this.duration = skillStruct.duration;
            timers.Add("duration", new utility.Timer(duration));
            timers["duration"].resetAndStart();

            timers.Add("particle_emission", new utility.Timer(10));
            timers["particle_emission"].resetAndStart();

            skillName = skillStruct.name;

            piercing = skillStruct.piercing;

            if (parent != null)
                height = parent.skillHeight;

            if (null != actingParent)
            {
                this.parent = actingParent;
            }
            else
            {
                this.parent = parent;
                rootSkill = true;
            }
            this.facing = parent.facing;
            this.movementDirection = parent.aimDirection + skillStruct.directionChange;
            this.aimDirection = parent.aimDirection + skillStruct.directionChange;
            this.gameplay = gameplay;



            this.statIncrement = skillStruct.statIncrement;

            this.force = new KeyValuePair<int, int>((int)this.facing, skillStruct.force);

            velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                (float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));

            LoadBlows(skillStruct.blows, gameplay);

            cue = skillStruct.cue;

            this.followParent = skillStruct.followParent;

            if (followParent)
                reactToWall = false;
            else
            {
                reactToWall = true;
                this.bounce = skillStruct.bounce;
            }


            Initialize();
            
        }

        protected void reCreateBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            bodies.Add("body", BodyFactory.CreateBody(w, pos - center));
            //bodies.Add("hotspot", BodyFactory.CreateBody(w, pos));

            if (!solid)
                bodies["body"].IsSensor = true;

            FixtureFactory.AttachCircle(size.X / 2, 1f, bodies["body"], center);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Dynamic;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;

            bodies["body"].UserData = this;

            if (!solid)
                bodies["body"].IsSensor = true;
        }

        protected void releaseBlows()
        {
            gameplay.factory.addBlowReleaser(this, this.parent, blows);
            /*for (int i = blows.Count - 1; i >= 0; i--)
            {
                    gameplay.factory.addActionSkill(blows[i].Value, this, this.parent);
                    blows.Remove(blows[i]);
            }*/
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (bodies.ContainsKey("body"))
                bodies["body"].LinearVelocity = velocity;

            for (int i = blows.Count - 1; i >= 0; i--)
            {
                blows[i].Key.Update(gameTime);
                if (blows[i].Key.isDone())
                {
                    gameplay.factory.addActionSkill(blows[i].Value, this, this.parent);
                    blows.Remove(blows[i]);
                }
            }

            if (followParent)
            {
                skillStruct.pos += skillStruct.velocity;
                if (aimed)
                {
                    SetPos(new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)aimDirection + skillStruct.directionChange)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)aimDirection + skillStruct.directionChange))),
                            (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)aimDirection + skillStruct.directionChange)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)aimDirection + skillStruct.directionChange)))));

                    this.facing = Helper.constrainDirection((Direction)aimDirection + skillStruct.directionChange);

                    if (bodySpriter != null)
                        bodySpriter.setAngle(-(float)((int)parent.facing % 360));
                }
                else
                {
                    SetPos(new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange))),
                            (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.aimDirection + skillStruct.directionChange)))));

                    this.facing = Helper.constrainDirection(parent.facing + skillStruct.directionChange);

                    if (bodySpriter != null)
                        bodySpriter.setAngle(-(float)((int)(parent.facing + skillStruct.directionChange) % 360));

                }
            }
            else if (bodySpriter != null)
            {
                this.height = (int)(MathHelper.Lerp((float)tempHeight, 0f, timers["duration"].getPercent()) + (Math.Sin(timers["duration"].getPercent() * Math.PI) * maxHeight));
                bodySpriter.setAngle(-(float)(aimDirection % 360));
            }

            if (null != parent)
            {
                parent.AddLinearVelocity(new Vector2((float)(skillStruct.parentVelocity.X * Math.Cos(MathHelper.ToRadians((float)(parent.facing + skillStruct.directionChange))) + skillStruct.parentVelocity.Y * Math.Sin(MathHelper.ToRadians((float)(parent.facing + skillStruct.directionChange)))) * (((float)gameTime.ElapsedGameTime.Milliseconds) / 17f),
                        (float)(skillStruct.parentVelocity.X * Math.Sin(MathHelper.ToRadians((float)(parent.facing + skillStruct.directionChange))) * (((float)gameTime.ElapsedGameTime.Milliseconds) / 17f) - skillStruct.parentVelocity.Y * Math.Cos(MathHelper.ToRadians((float)(parent.facing + skillStruct.directionChange))))) * (((float)gameTime.ElapsedGameTime.Milliseconds) / 17f));
            }

            if (timers["particle_emission"].isDone() && skillStruct.particle.HasValue)
            {
                ParticleStruct tempParticle = skillStruct.particle.Value;
                tempParticle.pos = new Vector3(pos.X+center.X, this.height, pos.Y+center.Y);
                this.EmitParticles(tempParticle, 1);
                timers["particle_emission"].resetAndStart();
            }

            if (timers["duration"].isDone() || (hitConnected && !followParent))
                Remove();

            velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                (float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));


            skillHeight = height;
            //this.height = (int)(MathHelper.Lerp((float)tempHeight, 0f, timers["duration"].getPercent()) + (Math.Sin(timers["duration"].getPercent() * Math.PI) * maxHeight));

            
        }

        public void PlayTakeSound()
        {
            
            //_sound.playCueInstance(id + "hurt", emitter);

            _sound.playCue("hurt", emitter);
        }

        protected override void SetupSpriterPlayer()
        {
            sPlayers = new Dictionary<string, SpriterPlayer>();

            

        }

        protected void SetupActionSkillSpriter(SkillStruct _skillStruct)
        {
            if (_skillStruct.spriterName == "")
            {
                this.visible = false;
                return;
            }
            else
            {
                this.visible = true;
            }

            sPlayers.Add("actionskill", new SpriterPlayer(factory._spriterManager.spriters[_skillStruct.spriterName].getSpriterData(), _skillStruct.spriterEntityIndex, factory._spriterManager.spriters[_skillStruct.spriterName].loader));

            sPlayers["actionskill"].setAnimation(_skillStruct.spriterAnimationName, 0, 0);
            bodySpriter = sPlayers["actionskill"];

            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);

            bodySpriter.setScale((((float)size.X) / 1f) * (2f / factory._gm.camera.zoom));

            //height = 15;
            //if (parent != null)
                //height = parent.height;
            
            tempHeight = height;

            bodySpriter.setAngle(-(float)(this.movementDirection % 360));

            if (!followParent)
            {
                sPlayers.Add("shadow", new SpriterPlayer(factory._spriterManager.spriters["shadow"].getSpriterData(), 0, factory._spriterManager.spriters["shadow"].loader));
                shadowSpriter = sPlayers["shadow"];
                shadowSpriter.setAnimation("still", 0, 0);
                shadowSpriter.setFrameSpeed(20);
                drawShadow = true;
            }
        }

        protected override void HandleCollisions()
        {
            if (just_for_show || this.remove || hitConnected)
                return;

            bool tempWallCollision = false;

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && !this.remove
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !((Agent)c.Other.UserData).remove
                    && !((Agent)c.Other.UserData).noCollision
                    && !c.Other.UserData.Equals(this.parent))
                {
                    if (this.parent != null
                        && ((Agent)c.Other.UserData).parent != null
                        && ((Agent)c.Other.UserData).parent.Equals(this.parent)
                        && ignoreSameParent)
                    {
                        if (bounce)
                        {
                            //Vector2 movementVector = Helper.GetDirectionVectorFromDegrees(movementDirection);
                            //movementVector = -2f * Vector2.Dot(movementVector, c.Contact.Manifold.LocalNormal) * c.Contact.Manifold.LocalNormal + movementVector;

                            //movementDirection = (int)Helper.GetDegreesFromVector(movementVector);
                            //aimDirection = (int)Helper.GetDegreesFromVector(movementVector);

                            //velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                                //(float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));
                            //_sound.playCue("ready ping");

                            //ParticleStruct ps = ParticleServer.GenerateSpark(new Vector3(pos.X + center.X, height, pos.Y + center.Y), movementVector * 3f);
                            //this.EmitParticles(ps, 3);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    
                    ((Agent)c.Other.UserData).TakeSkill(this);
                    //hitConnected = true;

                    if (!(c.Other.UserData is ActionSkill) || !piercing)
                    {
                        hitConnected = true;
                        if (!followParent)
                        {
                            Remove();
                        }
                    }
                    
                }
                else if (!wallCollision && !tempWallCollision && reactToWall && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    tempWallCollision = true;
                    if (bounce)
                    {
                        Vector2 movementVector = Helper.GetDirectionVectorFromDegrees(movementDirection);
                        movementVector = -2f * Vector2.Dot(movementVector, c.Contact.Manifold.LocalNormal) * c.Contact.Manifold.LocalNormal + movementVector;
                        
                        movementDirection = (int)Helper.GetDegreesFromVector(movementVector);
                        aimDirection = (int)Helper.GetDegreesFromVector(movementVector);

                        velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                            (float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));
                        _sound.playCue("ready ping");

                        ParticleStruct ps = ParticleServer.GenerateSpark(new Vector3(pos.X+center.X, height, pos.Y+center.Y), movementVector*3f);
                        this.EmitParticles(ps, 3);
                    }
                    else
                        Remove();
                }

                c = c.Next;
            }

            //if (tempWallCollision)
            wallCollision = tempWallCollision;
        }

        public override void TakeSkill(ActionSkill action)
        {
            if (!piercing)
            {
                this.hitConnected = true;
                if(!followParent)
                {
                    this.Remove();
                }
            }

            if (!action.piercing)
            {
                action.hitConnected = true;
                if (!action.followParent)
                {
                    action.Remove();
                }
            }
                
        }

        public void StealParent(Entity e)
        {
            parent.weaponInUse = null;
            this.parent = e;
        }

        protected override void Dispose()
        {
            //if (parent is Agent && parent.weaponInUse != null)
                //parent.weaponInUse.Equip((Agent)parent);

            parent.weaponInUse = null;
            base.Dispose();
        }

        public override void Remove()
        {
            releaseBlows();

            if (rootSkill)
            {
                if (parent is Agent && parent.weaponInUse!=null)
                    parent.weaponInUse.Equip((Agent)parent);
                parent.weaponInUse = null;

            }
            this.remove = true;
            base.Remove();
        }
    }
}
