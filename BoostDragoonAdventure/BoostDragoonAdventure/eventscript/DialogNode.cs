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

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            gm.factory.DisplayMessage(text);
        }
    }
}
