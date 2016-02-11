using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.utility;
using wickedcrush.entity.physics_entity.agent.action;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using wickedcrush.manager.gameplay;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.entity.holder
{
    public class BlowReleaser : Entity
    {
        List<KeyValuePair<Timer, SkillStruct>> blows;
        GameplayManager gameplay;

        Entity actingParent;

        

        public BlowReleaser(Entity parent, Entity actingParent, List<KeyValuePair<Timer, SkillStruct>> orphanedBlows, SoundManager sound, GameplayManager gameplay) : 
            base(parent.pos, Vector2.Zero, Vector2.Zero, sound)
        {
            blows = orphanedBlows;
            //blows = new List<KeyValuePair<Timer, SkillStruct>>(orphanedBlows);
            
            this.gameplay = gameplay;
            this.actingParent = actingParent;
            this.facing = parent.facing;
            this.movementDirection = parent.movementDirection;
            this.aimDirection = parent.aimDirection;


            //for (int i = 0; i < orphanedBlows.Count; i++)
            //{
                //Timer t = new Timer(i * 20);
                //t.resetAndStart();
                //blows.Add(new KeyValuePair<Timer, SkillStruct>(t, orphanedBlows[i].Value ));
            //}
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = blows.Count - 1; i >= 0; i--)
            {
                blows[i].Key.Update(gameTime);
                if (blows[i].Key.isDone())
                {
                    gameplay.factory.addActionSkill(blows[i].Value, this, actingParent);
                    blows.Remove(blows[i]);
                    
                }
            }

            if (blows.Count == 0)
            {
                Remove();
            }
        }
    }
}
