using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class CheckIntNode : EventNode
    {
        public String key;

        public CheckIntNode(String key, EventNode parent)
        {
            type = NodeType.CheckInt;
            this.key = key;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            int val = -1;

            if (p.getStats().numbersContainsKey(key))
            {
                val = p.getStats().getNumber(key);
            }

            bool childFound = false;

            foreach (EventNode node in children)
            {
                if (node is IntValNode && ((IntValNode)node).val == val)
                {
                    
                    script.curr = node.children[0];

                    childFound = true;
                    
                }
            }

            if (childFound)
            {
                script.nodeStack.Push(this);
            }
            else
            {
                script.curr = script.curr.next;
            }
            
        }
    }
}
