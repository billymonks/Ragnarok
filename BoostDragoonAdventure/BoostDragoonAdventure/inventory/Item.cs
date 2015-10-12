using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.controls;

namespace wickedcrush.inventory
{
    public delegate void ItemAction(Agent a, Item i);

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

        public Item(
            String name,
            ItemAction action
            )
        {
            this.name = name;
            this.action = action;

        }

        public Item(Item i)
        {
            this.name = i.name;
            this.action = i.action;
        }

        public void Use(Agent a)
        {
            //if (a.stats.inventory.getItemCount(this) <= 0)
                //return;

            action(a, this);
        }
    }
}
