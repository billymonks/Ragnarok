using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity.physics_entity.agent.trap.trigger;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.entity.physics_entity.agent.trap.triggerable;

namespace wickedcrush.utility.trigger
{
    public delegate bool ConditionDelegate(TriggerBase a);

    public class Trigger
    {
        public TriggerBase parent;

        public List<Triggerable> targets { get; set; }
        public ConditionDelegate condition;
        public bool repeat, done;

        public Trigger(TriggerBase parent, Triggerable target, ConditionDelegate condition)
        {
            this.parent = parent;

            targets = new List<Triggerable>();
            targets.Add(target);

            this.condition = condition;
            
            repeat = false;
            done = false;
        }

        public Trigger(TriggerBase parent, List<Triggerable> targets, ConditionDelegate condition)
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

        public void addTarget(Triggerable target)
        {
            if (!targets.Contains(target))
                targets.Add(target);
        }

        public void addTargets(List<Triggerable> targets)
        {
            foreach (Triggerable t in targets)
            {
                addTarget(t);
            }
        }

        public void removeTarget(Triggerable target)
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
            foreach (Triggerable target in targets)
                target.activate(); //switch to action delegate? prolly not, keep it simple
        }
    }
}
