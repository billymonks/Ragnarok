using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public class Inventory
    {

        private Dictionary<Item, int> inventory;
        public int currency;

        public Item itemA, itemB, itemC;

        public bool changed = false;

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
                changed = true;
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
                changed = true;
            }
        }

        public int getItemCount(Item i)
        {
            if(inventory.ContainsKey(i))
                return inventory[i];

            return 0;
        }

        public void pressItem(Item i, Agent a)
        {
            i.Press(a);
        }

        public void holdItem(Item i, Agent a)
        {
            i.Hold(a);
        }

        public void releaseItem(Item i, Agent a)
        {
            i.Release(a);
        }

        public void removeItem(Item i)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i]--;
                if (inventory[i] <= 0)
                {
                    inventory.Remove(i);
                    changed = true;
                }
            }
        }

        public void removeItem(Item i, int count)
        {
            if (inventory.ContainsKey(i))
            {
                inventory[i] -= count;
                if (inventory[i] <= 0)
                {
                    inventory.Remove(i);
                    changed = true;
                }
            }
        }

        public void removeAllOfItem(Item i)
        {
            if (inventory.ContainsKey(i))
            {
                inventory.Remove(i);
                changed = true;
            }
        }

        public void clearInventory()
        {
            inventory.Clear();
            changed = true;
        }

        public void addCurrency(int number)
        {
            currency += number;
            if (currency < 0)
                currency = 0;
        }

        public List<Item> GetItemList()
        {
            changed = false;
            return inventory.Keys.ToList<Item>();
        }
    }
}
