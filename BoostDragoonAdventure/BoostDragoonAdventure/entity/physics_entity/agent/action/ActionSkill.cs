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

namespace wickedcrush.entity.physics_entity.agent.action
{
    public class ActionSkill : Agent
    {
        String skillName;

        int duration;

        public List<KeyValuePair<String, int>> statIncrement;
        List<KeyValuePair<Timer, SkillStruct>> blows = new List<KeyValuePair<Timer, SkillStruct>>();

        public KeyValuePair<int, int> force; //direction, force amount

        protected bool reactToWall = true, piercing = false, ignoreSameParent = true, just_for_show = false, followParent = false;

        private Vector2 velocity;

        GameplayManager gameplay;

        String cue;

        SkillStruct skillStruct;

        //Agent agentParent;

        public ActionSkill(SkillStruct skillStruct, GameBase game, GameplayManager gameplay, Entity parent, Entity actingParent)
            : base(gameplay.w,
            new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.movementDirection)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.movementDirection))),
                        (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.movementDirection)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.movementDirection)))), 
            skillStruct.size, 
            skillStruct.center, 
            false, 
            gameplay.factory, game.soundManager) 
        {
            this.skillStruct = skillStruct;
            this.duration = skillStruct.duration;
            timers.Add("duration", new utility.Timer(duration));
            timers["duration"].resetAndStart();

            skillName = skillStruct.name;
            
            if (null != actingParent)
            {
                this.parent = actingParent;
            } 
            else
            {
                this.parent = parent;
            }
            this.facing = parent.facing;
            this.movementDirection = parent.movementDirection + skillStruct.directionChange;
            this.gameplay = gameplay;

            

            this.statIncrement = skillStruct.statIncrement;

            this.force = new KeyValuePair<int, int>((int)this.facing, skillStruct.force);

            velocity = new Vector2((float)(skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)movementDirection)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)movementDirection))),
                (float)(skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)movementDirection)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)movementDirection))));

            LoadBlows(skillStruct.blows, gameplay);

            cue = skillStruct.cue;

            this.followParent = skillStruct.followParent;

            Initialize();
        }

        protected override void setupBody(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            base.setupBody(w, pos, size, center, solid);
            FixtureFactory.AttachCircle(size.X / 2, 1f, bodies["body"], center);
            bodies["body"].FixedRotation = true;
            bodies["body"].LinearVelocity = Vector2.Zero;
            bodies["body"].BodyType = BodyType.Dynamic;
            bodies["body"].CollisionGroup = (short)CollisionGroup.AGENT;

            bodies["body"].UserData = this;

            if (!solid)
                bodies["body"].IsSensor = true;

            //FixtureFactory.AttachRectangle(1f, 1f, 1f, Vector2.Zero, bodies["hotspot"]);
            //bodies["hotspot"].FixedRotation = true;
            //bodies["hotspot"].LinearVelocity = Vector2.Zero;
            //bodies["hotspot"].BodyType = BodyType.Dynamic;



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
            
            //ParticleStruct ps = new ParticleStruct(new Vector3(this.pos.X + this.center.X, this.height, this.pos.Y + this.center.Y), new Vector3(-0.5f, 2f, -0.5f), new Vector3(1f, 1f, 1f), new Vector3(0, -.1f, 0), 0f, 0f, 1000, "particles", 0, "white_to_blue");
            //particleEmitter.EmitParticles(ps, factory, 3);

            if (cue != "")
            {
                _sound.playCue(cue, emitter); // play activate sound
            }

            SetupActionSkillSpriter();


            //_sound.addCueInstance("hurt", id + "hurt", false);
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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
                SetPos(parent.pos + parent.center);
                
            }

            if (null != parent)
            {
                parent.AddLinearVelocity(new Vector2((float)(skillStruct.parentVelocity.X * Math.Cos(MathHelper.ToRadians((float)this.facing)) + skillStruct.parentVelocity.Y * Math.Sin(MathHelper.ToRadians((float)this.facing))),
                        (float)(skillStruct.parentVelocity.X * Math.Sin(MathHelper.ToRadians((float)this.facing)) - skillStruct.parentVelocity.Y * Math.Cos(MathHelper.ToRadians((float)this.facing)))) * (((float)gameTime.ElapsedGameTime.Milliseconds) / 17f));
            }

            if (skillStruct.particle.HasValue)
            {
                ParticleStruct tempParticle = skillStruct.particle.Value;
                tempParticle.pos = new Vector3(pos.X, this.height, pos.Y);
                this.EmitParticles(tempParticle, 1);
            }

            if (timers["duration"].isDone())
                this.remove = true;

            //just_for_show = true;
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

        protected void SetupActionSkillSpriter()
        {
            if (skillStruct.spriterName == "")
            {
                this.visible = false;
                return;
            }

            sPlayers.Add("actionskill", new SpriterPlayer(factory._spriterManager.spriters[skillStruct.spriterName].getSpriterData(), skillStruct.spriterEntityIndex, factory._spriterManager.spriters[skillStruct.spriterName].loader));

            sPlayers["actionskill"].setAnimation(skillStruct.spriterAnimationName, 0, 0);
            bodySpriter = sPlayers["actionskill"];

            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);

            bodySpriter.setScale((((float)size.X) / 10f) * (2f / factory._gm.camera.zoom));
            height = 10;

            bodySpriter.setAngle(-(float)(this.movementDirection % 360));
        }

        protected override void HandleCollisions()
        {
            if (just_for_show || this.remove)
                return;

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
                    && !this.remove
                    && c.Other.UserData != null
                    && c.Other.UserData is Agent
                    && !((Agent)c.Other.UserData).noCollision
                    && !c.Other.UserData.Equals(this.parent))
                {
                    if (this.parent != null
                        && ((Agent)c.Other.UserData).parent != null
                        && ((Agent)c.Other.UserData).parent.Equals(this.parent)
                        && ignoreSameParent)
                        break;

                    
                    
                    
                    
                    
                    
                    
                    
                    ((Agent)c.Other.UserData).TakeSkill(this);

                    if (!piercing)
                        this.remove = true;
                }
                else if (reactToWall && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    this.remove = true;
                }

                c = c.Next;
            }
        }

        protected override void Dispose()
        {
            base.Dispose();
        }
    }
}
