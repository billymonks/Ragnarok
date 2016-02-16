using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class SetIntNode : EventNode
    {
        public String key;
        public int val;

        public SetIntNode(String key, int val, EventNode parent)
        {
            type = NodeType.SetInt;
            this.key = key;
            this.val = val;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            p.getStats().set(key, val);

            script.curr = next;

            //gm.factory.DisplayMessage(text);
        }
    }
}
