using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.inventory;

namespace wickedcrush.eventscript
{
    public class ItemGetNode : EventNode
    {
        public String item;

        public ItemGetNode(String item, EventNode parent)
        {
            type = NodeType.ItemGet;
            this.item = item;
            this.parent = parent;
        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            Weapon w = InventoryServer.getWeapon(item);

            p.getStats().inventory.receiveItem(w);

            if (p.getStats().inventory.equippedWeapon == null)
            {
                p.getStats().inventory.equippedWeapon = w;
                w.Equip(p.getAgent());
            }

            script.curr = next;

            //gm.factory.DisplayMessage(text);
        }
    }
}
