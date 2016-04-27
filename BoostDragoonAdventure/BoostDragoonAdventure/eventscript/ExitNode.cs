using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.eventscript
{
    public class ExitNode : EventNode
    {
        public ExitNode(EventNode parent)
        {
            type = NodeType.Exit;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            game.playerManager.saveAllPlayers();
            game.Exit();

            //gm.factory.DisplayMessage(text);
        }
    }
}
