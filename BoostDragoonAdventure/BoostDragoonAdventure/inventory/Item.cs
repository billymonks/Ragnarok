using Microsoft.Xna.Framework;
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
        UsesAmmo = 2,
        UsesFuelCharge = 3
    }

    public class Item
    {
        public String name;
        protected ItemAction action;
        public ItemType type;

        public int fuelCost = 0;
        public int maxFuelCost = 0;
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
            this.maxFuelCost = fuelCost;
        }

        public Item(String name, ItemType type, ItemAction action, int fuelCost, int maxFuelCost)
        {
            this.name = name;
            this.type = type;
            this.action = action;

            this.fuelCost = fuelCost;
            this.maxFuelCost = maxFuelCost;
        }

        public Item(String name, ItemType type, ItemAction action, Item ammoType)
        {
            this.name = name;
            this.type = type;
            this.action = action;

            this.ammoType = ammoType;
        }

        private void useItem(Agent a)
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

        public void useItem(Agent a, int charge)
        {
            if(type==ItemType.UsesFuelCharge)
            {
                int calculatedCost = (int)MathHelper.Lerp((float)fuelCost, (float)maxFuelCost, (float)charge / 100f);

                if (a.stats.get("boost") >= calculatedCost)
                {
                    action(a);
                    a.stats.addTo("boost", -calculatedCost);
                }
            }
            else { useItem(a); }
        }
    }
}
