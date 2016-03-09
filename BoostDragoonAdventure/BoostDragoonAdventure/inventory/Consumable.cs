using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;
using wickedcrush.controls;

namespace wickedcrush.inventory
{

    public class Consumable : Item
    {
        protected ItemAction action;

        public Consumable(String name, String desc, ItemAction action) : base(name, desc)
        {
            this.action = action;

            this.type = ItemType.Consumable;

            this.value = 250;
        }

        public void Use(Agent a)
        {
            //if (a.stats.inventory.getItemCount(this) <= 0)
                //return;

            action(a, this);
            a.stats.inventory.removeItem(this);
        }
    }
}
