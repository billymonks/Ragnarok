using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.utility.trigger
{
    public delegate bool ConditionDelegate(TriggerBase a);

    public class Trigger
    {
        public TriggerBase parent;

        public List<ITriggerable> targets { get; set; }
        public ConditionDelegate condition;
        public bool repeat, done;

        public Trigger(TriggerBase parent, ITriggerable target, ConditionDelegate condition)
        {
            this.parent = parent;

            targets = new List<ITriggerable>();
            targets.Add(target);

            this.condition = condition;
            
            repeat = false;
            done = false;
        }

        public Trigger(TriggerBase parent, List<ITriggerable> targets, ConditionDelegate condition)
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

        public void addTargets(List<ITriggerable> targets)
        {
            foreach (ITriggerable t in targets)
            {
                addTarget(t);
            }
        }

        public void removeTarget(ITriggerable target)
        {
            if (targets.Contains(target))
                targets.Remove(target);
        }

        public void clearTargets()
        {
            targets.Clear();
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
