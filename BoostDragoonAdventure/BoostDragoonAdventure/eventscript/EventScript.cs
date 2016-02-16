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
            Reset();
        }

        public void Process(GameBase game, GameplayManager gm, Player p)
        {
            if (curr == null)
            {
                while (nodeStack.Count > 0 && (nodeStack.Peek().type.Equals(NodeType.Answer) || nodeStack.Peek().type.Equals(NodeType.IntVal) || nodeStack.Peek().next == null))
                {
                    nodeStack.Pop();
                }

                if (nodeStack.Count > 0 && nodeStack.Peek().next != null)
                {
                    curr = nodeStack.Pop().next;
                    curr.Process(game, gm, p, this);
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
                curr.Process(game, gm, p, this);
            }


        }

        public void Reset()
        {
            done = false;
            if (convo.Count > 0)
            {
                curr = convo[0];
            }
        }
    }
}
