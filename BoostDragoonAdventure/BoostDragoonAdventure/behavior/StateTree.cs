using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.entity;
using wickedcrush.behavior.state;

namespace wickedcrush.behavior
{
    public class StateTree
    {
        public Dictionary<String, StateBranch> tree;

        public State currentControlState;
        public State previousControlState;

        public StateTree()
        {
            tree = new Dictionary<String, StateBranch>();

            currentControlState = null;
            previousControlState = null;
        }

        public StateTree(Dictionary<String, StateBranch> tree)
        {
            this.tree = tree;

            currentControlState = null;
            previousControlState = null;
        }

        public void AddBranch(String str, StateBranch b)
        {
            tree.Add(str, b);
        }

        public void Update(GameTime gameTime, Entity e)
        {
            foreach (KeyValuePair<String, StateBranch> branch in tree)
            {
                if (branch.Value.testBranchCondition(e))
                {
                    branch.Value.Update(gameTime, e, this);
                }
            }

        }
    }
}
