using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.manager.gameplay;
using wickedcrush.player;

namespace wickedcrush.eventscript
{
    public class EventScript
    {
        public List<EventNode> convo;
        public EventNode curr;

        public Stack<EventNode> nodeStack = new Stack<EventNode>();

        public bool done = false;

        public EventScript(List<EventNode> convo)
        {
            this.convo = convo;
            if (convo.Count > 0)
            {
                curr = convo[0];
                //nodeStack.Push(curr);
            }
        }

        public void Process(GameBase game, GameplayManager gm, Player p)
        {
            if (curr == null)
            {
                while (nodeStack.Count > 0 && (nodeStack.Peek().type.Equals(NodeType.Answer) || nodeStack.Peek().type.Equals(NodeType.IntVal) || nodeStack.Peek().next == null))
                {
                    nodeStack.Pop();
                }

                if (nodeStack.Peek().next != null)
                {
                    curr = nodeStack.Peek().next;
                    curr.Process(game, gm, p, nodeStack, curr);
                }
                else
                {
                    done = true;
                    return;
                }
            }
            else
            {
                // process curr
                curr.Process(game, gm, p, nodeStack, curr);
            }


        }
    }
}
