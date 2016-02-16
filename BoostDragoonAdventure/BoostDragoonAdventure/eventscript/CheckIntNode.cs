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

            if (next != null)
            {
                script.nodeStack.Push(this);
            }

            //bool fallThrough = true;

            foreach (EventNode node in children)
            {
                if (node is IntValNode && ((IntValNode)node).val == val)
                {
                    //script.nodeStack.Push(this);
                    script.curr = node.children[0];
                    //fallThrough = false;
                }
            }

            //if (fallThrough = true)
            //{
                //script.curr = next;
            //}

            //script.curr = next;

            //gm.factory.DisplayMessage(text);
        }
    }
}
