using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public delegate void ItemAction(Agent a);

    public enum ItemType
    {
        Consumable = 0,
        UsesFuel = 1,
        UsesAmmo = 2
    }

    public class Item
    {
        public String name;
        protected ItemAction action;
        public ItemType type;

        public int fuelCost = 0;
        public Item ammoType;

        public Item(String name, ItemType type, ItemAction action)
        {
            this.name = name;
            this.type = type;
            this.action = action;
        }

        public Item(String name, ItemType type, ItemAction action, int fuelCost)
        {
            this.name = name;
            this.type = type;
            this.action = action;

            this.fuelCost = fuelCost;
        }

        public Item(String name, ItemType type, ItemAction action, Item ammoType)
        {
            this.name = name;
            this.type = type;
            this.action = action;

            this.ammoType = ammoType;
        }

        public void useItem(Agent a)
        {
            switch(type)
            {
                case ItemType.Consumable:
                        action(a);
                        a.stats.inventory.removeItem(this);
                    break;
                case ItemType.UsesAmmo:
                    if(a.stats.inventory.getItemCount(ammoType) > 0)
                    {
                        action(a);
                        a.stats.inventory.removeItem(ammoType);
                    }
                    break;
                case ItemType.UsesFuel:
                    if (a.stats.get("boost") >= fuelCost)
                    {
                        action(a);
                        a.stats.addTo("boost", -fuelCost);
                    }
                    break;
            }
        }
    }
}
