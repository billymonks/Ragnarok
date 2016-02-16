using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class DialogNode : EventNode
    {
        public String text;

        public DialogNode(String text, EventNode parent)
        {
            type = NodeType.Dialog;
            this.text = text;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, Stack<EventNode> nodeStack, EventNode curr)
        {
            base.Process(game, gm, p, nodeStack, curr);

            gm.factory.DisplayMessage(text);

            curr = next;
        }
    }
}
