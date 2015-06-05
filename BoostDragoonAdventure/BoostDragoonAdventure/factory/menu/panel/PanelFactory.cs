using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.inventory;
using wickedcrush.menu.panel;
using wickedcrush.display.spriter;

namespace wickedcrush.factory.menu.panel
{
    public class PanelFactory
    {
        public SpriterManager _sm;
        public PanelFactory(SpriterManager sm)
        {
            _sm = sm;
        }

        public InventoryPanel getInventory(Inventory inventory)
        {
            return new InventoryPanel(inventory);
        }
    }
}
