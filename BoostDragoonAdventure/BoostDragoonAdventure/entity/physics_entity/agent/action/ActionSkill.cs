using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.manager.gameplay;
using Microsoft.Xna.Framework;
using wickedcrush.utility;

namespace wickedcrush.entity.physics_entity.agent.action
{
    public class ActionSkill : Agent
    {
        String skillName;

        int duration;

        public List<KeyValuePair<String, int>> statIncrement;
        List<KeyValuePair<Timer, SkillStruct>> blows = new List<KeyValuePair<Timer, SkillStruct>>();

        public KeyValuePair<int, int> force; //direction, force amount

        protected bool reactToWall = false, piercing = true, ignoreSameParent = true;

        private Vector2 velocity = new Vector2(0f, 0f);

        GameplayManager gameplay;

        public ActionSkill(SkillStruct skillStruct, GameBase game, GameplayManager gameplay, Agent parent)
            : base(gameplay.w,
            new Vector2((float)(parent.pos.X + parent.center.X + skillStruct.pos.X * Math.Cos(MathHelper.ToRadians((float)parent.facing)) + skillStruct.pos.Y * Math.Sin(MathHelper.ToRadians((float)parent.facing))),
                        (float)(parent.pos.Y + parent.center.Y + skillStruct.pos.X * Math.Sin(MathHelper.ToRadians((float)parent.facing)) + skillStruct.pos.Y * Math.Cos(MathHelper.ToRadians((float)parent.facing)))), 
            skillStruct.size, 
            skillStruct.center, 
            false, 
            gameplay.factory, game.soundManager) 
        {
            this.duration = skillStruct.duration;
            timers.Add("duration", new utility.Timer(duration));
            timers["duration"].resetAndStart();

            skillName = skillStruct.name;
            this.parent = parent;
            this.facing = parent.facing;
            this.gameplay = gameplay;

            this.statIncrement = skillStruct.statIncrement;

            this.force = new KeyValuePair<int, int>((int)this.facing, skillStruct.force);

            LoadBlows(skillStruct.blows, gameplay);
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

        private void Initialize(int damage, int force)
        {
            airborne = true;
            immortal = true;
            this.name = "ActionSkill";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = blows.Count - 1; i >= 0; i--)
            {
                blows[i].Key.Update(gameTime);
                if (blows[i].Key.isDone())
                {
                    gameplay.factory.addActionSkill(blows[i].Value, this);
                    blows.Remove(blows[i]);
                }
            }

            if (timers["duration"].isDone())
                this.Remove();
        }

        protected override void HandleCollisions()
        {
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
                        Remove();
                }
                else if (reactToWall && c.Contact.IsTouching && c.Other.UserData is LayerType && ((LayerType)c.Other.UserData).Equals(LayerType.WALL))
                {
                    Remove();
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
