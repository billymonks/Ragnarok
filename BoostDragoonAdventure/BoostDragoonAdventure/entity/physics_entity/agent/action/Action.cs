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

        List<KeyValuePair<String, int>> statIncrement;
        List<KeyValuePair<Timer, ActionSkill>> blows;

        KeyValuePair<int, float> force; //direction, force amount

        protected bool reactToWall = false, piercing = true, ignoreSameParent = true;

        public ActionSkill(Vector2 pos, Vector2 size, Vector2 center, int duration, GameBase game, GameplayManager gameplay) 
            : base(gameplay.w, pos, size, center, false, gameplay.factory, game.soundManager) 
        {
            timers.Add("duration", new utility.Timer(duration));
            timers["duration"].resetAndStart();

        }

        protected void LoadBlows(List<KeyValuePair<int, ActionSkill>> blows)
        {
            foreach(KeyValuePair<int, ActionSkill> blow in blows)
            {
                this.blows.Add(new KeyValuePair<Timer, ActionSkill>(new Timer(blow.Key), blow.Value));
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

                    ((Agent)c.Other.UserData).TakeHit(this);

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
    }
}
