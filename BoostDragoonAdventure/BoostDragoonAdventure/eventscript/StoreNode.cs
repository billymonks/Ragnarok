using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.inventory;
using wickedcrush.screen.menu;

namespace wickedcrush.eventscript
{
    public class StoreNode : EventNode
    {
        public String text;

        public List<Item> storeItems;

        public StoreNode(EventNode parent, int storeId)
        {
            type = NodeType.Equip;
            this.parent = parent;

            storeItems = new List<Item>();

            if(storeId == 0)
            {
                storeItems.Add(InventoryServer.getWeapon("Longsword"));
                storeItems.Add(InventoryServer.getWeapon("Scattershot"));
                storeItems.Add(InventoryServer.getWeapon("Rifle"));
                storeItems.Add(InventoryServer.getWeapon("Spellbook: Fireball"));
                storeItems.Add(InventoryServer.getWeapon("Spellbook: Etheral Surge"));
            } else if(storeId == 1)
            {
                storeItems.Add(InventoryServer.getPart("KR101 Core"));
                storeItems.Add(InventoryServer.getPart("Dinky Chamber"));
                storeItems.Add(InventoryServer.getPart("Pro-grade Chamber"));
                storeItems.Add(InventoryServer.getPart("Dinky Belt"));
                storeItems.Add(InventoryServer.getPart("Dinky Carburetor"));
                storeItems.Add(InventoryServer.getPart("Dinky Piston"));
                storeItems.Add(InventoryServer.getPart("Dinky Crankshaft B"));
                storeItems.Add(InventoryServer.getPart("Dinky Pump B"));
                
            } else if(storeId == 2)
            {
                storeItems.Add(InventoryServer.getConsumable("Pizza"));
                storeItems.Add(InventoryServer.getConsumable("Canned Pizza"));
                storeItems.Add(InventoryServer.getConsumable("Pizza Rolls"));
                storeItems.Add(InventoryServer.getConsumable("Pizza Pie"));
            }

        }

        public override void Process(GameBase game, manager.gameplay.GameplayManager gm, player.Player p, EventScript script)
        {
            base.Process(game, gm, p, script);

            script.curr = next;

            game.screenManager.AddScreen(new StoreMenuScreen(game, gm, p, storeItems));
        }
    }
}
