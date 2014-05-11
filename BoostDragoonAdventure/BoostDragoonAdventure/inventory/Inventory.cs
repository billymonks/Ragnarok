﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public class Inventory
    {

        private Dictionary<Item, int> inventory;
        private int currency;

        public Inventory()
        {
            inventory = new Dictionary<Item, int>();
            currency = 0;
        }

        public void receiveItem(Item i)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i]++;
            }
            else
            {
                inventory.Add(i, 1);
            }
        }

        public void receiveItem(Item i, int count)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i] += count;
            }
            else
            {
                inventory.Add(i, count);
            }
        }

        public void useItem(Item i, Agent a)
        {
            if (inventory.ContainsKey(i) && inventory[i] > 0)
            {
                i.useItem(a);
                removeItem(i);
            }
        }

        public void removeItem(Item i)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i]--;
                if (inventory[i] <= 0)
                    inventory.Remove(i);
            }
        }

        public void removeItem(Item i, int count)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i] -= count;
                if (inventory[i] <= 0)
                    inventory.Remove(i);
            }
        }

        public void removeAllOfItem(Item i)
        {
            if (inventory.ContainsKey(i))
                inventory.Remove(i);
        }
    }
}