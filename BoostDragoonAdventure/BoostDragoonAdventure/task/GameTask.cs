using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.task
{
    public delegate void ActionDelegate(GameBase g); //might change from object to something more specific if it turns out this is bad
    public delegate bool ConditionDelegate(GameBase g);

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

        public bool testCondition(GameBase g)
        {
            return condition(g);
        }

        public void runAction(GameBase g)
        {
            action(g);
        }
    }
}
