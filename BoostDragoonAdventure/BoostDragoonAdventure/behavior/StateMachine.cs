using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.behavior.state;
using wickedcrush.entity;
using Microsoft.Xna.Framework;

namespace wickedcrush.behavior
{
    public class StateMachine
    {
        public State currentControlState;
        public State previousControlState;
        public Dictionary<String, State> control;

        public StateMachine(Dictionary<String, State> ctrl)
        {
            control = ctrl;
            currentControlState = null;
            previousControlState = null;
        }

        public void Update(GameTime gameTime, Entity e)
        {
            updateControlState(gameTime, e);
            executeControlState(gameTime, e);
        }

        private void updateControlState(GameTime gameTime, Entity e)
        {
            foreach (KeyValuePair<String, State> st in control)
            {
                if (st.Value.testCondition(e))
                {
                    currentControlState = st.Value;
                    return;
                }
            }
        }

        private void executeControlState(GameTime gameTime, Entity e)
        {
            currentControlState.runAction(e);
            previousControlState = currentControlState;
        }
    }
}
