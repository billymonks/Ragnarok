using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.task
{
    public delegate void ActionDelegate(Game g); //might change from object to something more specific if it turns out this is bad
    public delegate bool ConditionDelegate(Game g);

    public class GameTask
    {
        public ActionDelegate action;
        public ConditionDelegate condition;
        public bool passThrough;

        public GameTask(ConditionDelegate c, ActionDelegate a)
        {
            condition = c;
            action = a;
        }

        public bool testCondition(Game g)
        {
            return condition(g);
        }

        public void runAction(Game g)
        {
            action(g);
        }
    }
}
