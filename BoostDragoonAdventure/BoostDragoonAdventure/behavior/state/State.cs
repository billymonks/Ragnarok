using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity;

namespace wickedcrush.behavior.state
{
    public delegate void ActionDelegate(Entity e); //might change from object to something more specific if it turns out this is bad
    public delegate bool ConditionDelegate(Entity e);

    public class State
    {
        public String name;
        public ActionDelegate action;
        public ConditionDelegate condition;

        public State(String n, ConditionDelegate c, ActionDelegate a)
        {
            name = n;
            condition = c;
            action = a;
        }

        public bool testCondition(Entity e)
        {
            return condition(e);
        }

        public void runAction(Entity e)
        {
            action(e);
        }
    }
}
