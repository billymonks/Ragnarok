using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using wickedcrush.screen.menu;

namespace wickedcrush.eventscript
{
    public class SolidBackgroundNode : EventNode
    {
        public Color color;

        ColorDisplayScreen screen;

        public SolidBackgroundNode(Color color, EventNode parent)
        {
            type = NodeType.SolidBG;
            this.color = color;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            //gm.factory.DisplayMessage(text);
            script.nodeStack.Push(this);

            screen = gm.factory.DisplayColor(color);

            if(children != null && children.Count > 0)
                script.curr = children[0];
        }

        public override void Unload(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Unload(game, gm, p, script);
            game.screenManager.RemoveScreen(screen);
            
        }
    }
}
