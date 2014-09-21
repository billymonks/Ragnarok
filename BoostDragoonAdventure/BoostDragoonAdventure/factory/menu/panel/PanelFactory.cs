using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.inventory;
using wickedcrush.menu.panel;

namespace wickedcrush.factory.menu.panel
{
    public class PanelFactory
    {
        public PanelFactory()
        {

        }

        public InventoryPanel getInventory(Inventory inventory)
        {
            return new InventoryPanel(inventory);
        }
    }
}
