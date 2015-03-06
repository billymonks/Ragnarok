using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework;
using wickedcrush.utility;
using Com.Brashmonkey.Spriter.player;

namespace wickedcrush.entity.physics_entity.agent.action
{
    public class ActionSkill : Agent
    {
        String skillName;

        int duration;

        public List<KeyValuePair<String, int>> statIncrement;
        List<KeyValuePair<Timer, SkillStruct>> blows = new List<KeyValuePair<Timer, SkillStruct>>();

        public KeyValuePair<int, int> force; //direction, force amount

        protected bool reactToWall = false, piercing = true, ignoreSameParent = true, just_for_show = false;

        private Vector2 velocity;

        GameplayManager gameplay;

        String cue;

        public ActionSkill(SkillStruct skillStruct, GameBase game, GameplayManager gameplay, Entity parent, Entity actingParent)
            : base(gameplay.w,
            new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.facing)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.facing))),
                        (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.facing)) - skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.facing)))), 
            skillStruct.size, 
            skillStruct.center, 
            false, 
            gameplay.factory, game.soundManager) 
        {
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
            this.gameplay = gameplay;

            bodySpriter.setAngle(-(float)this.parent.facing);

            this.statIncrement = skillStruct.statIncrement;

            this.force = new KeyValuePair<int, int>((int)this.facing, skillStruct.force);

            velocity = new Vector2((float)( skillStruct.velocity.X * Math.Cos(MathHelper.ToRadians((float)parent.facing)) + skillStruct.velocity.Y * Math.Sin(MathHelper.ToRadians((float)parent.facing))),
                (float) (skillStruct.velocity.X * Math.Sin(MathHelper.ToRadians((float)parent.facing)) - skillStruct.velocity.Y * Math.Cos(MathHelper.ToRadians((float)parent.facing))));

            LoadBlows(skillStruct.blows, gameplay);

            cue = skillStruct.cue;

            Initialize();
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

            if (cue != "")
            {
                _sound.playCue(cue, emitter); // play activate sound
            }

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
            sPlayers.Add("actionskill", new SpriterPlayer(factory._spriterManager.spriters["all"].getSpriterData(), 3, factory._spriterManager.loaders["loader1"]));

            bodySpriter = sPlayers["actionskill"];
            //sPlayer.setAnimation("whitetored", 0, 0);
            bodySpriter.setFrameSpeed(60);
            
            bodySpriter.setScale(((float)size.X) / 10f);
            height = 10;

        }

        protected override void HandleCollisions()
        {
            if (just_for_show)
                return;

            var c = bodies["body"].ContactList;
            while (c != null)
            {
                if (c.Contact.IsTouching
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
