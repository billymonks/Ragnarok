using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.behavior.state;
using wickedcrush.entity;
using Microsoft.Xna.Framework;

namespace wickedcrush.behavior
{
    public class StateBranch
    {
        public ConditionDelegate branchCondition;

        
        public Dictionary<String, State> control;

        public StateBranch(ConditionDelegate branchCondition, Dictionary<String, State> ctrl)
        {
            this.branchCondition = branchCondition;
            control = ctrl;
            
            
        }

        public void Update(GameTime gameTime, Entity e, StateTree parent)
        {
            updateControlState(gameTime, e, parent);
            executeControlState(gameTime, e, parent);
        }

        private void updateControlState(GameTime gameTime, Entity e, StateTree parent)
        {
            foreach (KeyValuePair<String, State> st in control)
            {
                if (st.Value.testCondition(e))
                {
                    parent.currentControlState = st.Value;
                    return;
                }
            }
        }

        private void executeControlState(GameTime gameTime, Entity e, StateTree parent)
        {
            parent.currentControlState.runAction(e);
            parent.previousControlState = parent.currentControlState;
        }

        public bool testBranchCondition(Entity e)
        {
            return branchCondition(e);
        }
    }
}
