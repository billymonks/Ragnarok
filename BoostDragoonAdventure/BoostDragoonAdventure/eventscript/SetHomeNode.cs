using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class SetHomeNode : EventNode
    {
        String map;
        public SetHomeNode(String map, EventNode parent)
        {
            type = NodeType.SetHome;
            this.map = map;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            p.getAgent().stats.set("home", map);

            //gm.factory.DisplayMessage(text);
        }
    }
}
