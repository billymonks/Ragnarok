using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using Microsoft.Xna.Framework;

namespace wickedcrush.utility.trigger
{
    public delegate bool ConditionDelegate(Agent a);

    public class Trigger
    {
        public Agent parent;

        public List<ITriggerable> targets { get; set; }
        public ConditionDelegate condition;
        public bool repeat, done;

        public Trigger(Agent parent, ITriggerable target, ConditionDelegate condition)
        {
            this.parent = parent;

            targets = new List<ITriggerable>();
            targets.Add(target);

            this.condition = condition;
            
            repeat = false;
            done = false;
        }

        public Trigger(Agent parent, List<ITriggerable> targets, ConditionDelegate condition)
        {
            this.parent = parent;
            this.targets = targets;
            this.condition = condition;

            repeat = false;
            done = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (done)
                return;

            if (testCondition())
            {
                activate();

                if (!repeat)
                    done = true;
            }
        }

        public void addTarget(ITriggerable target)
        {
            if (!targets.Contains(target))
                targets.Add(target);
        }

        public void removeTarget(ITriggerable target)
        {
            if (targets.Contains(target))
                targets.Remove(target);
        }

        protected bool testCondition()
        {
            return condition(parent);
        }

        protected void activate()
        {
            foreach (ITriggerable target in targets)
                target.activate(); //switch to action delegate? prolly not, keep it simple
        }
    }
}
