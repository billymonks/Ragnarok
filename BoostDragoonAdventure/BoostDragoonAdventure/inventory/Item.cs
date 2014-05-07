using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public delegate void ItemAction(Agent a);

    public class Item
    {
        public String name;

        protected ItemAction action;

        public Item(String name, ItemAction action)
        {
            this.name = name;
            this.action = action;
        }

        public void useItem(Agent a)
        {
            action(a);
        }
    }
}
