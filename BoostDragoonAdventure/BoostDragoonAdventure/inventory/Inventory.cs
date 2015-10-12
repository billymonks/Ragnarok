using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public class Inventory
    {

        private Dictionary<Weapon, int> weapons;
        public int currency;

        public Weapon equippedWeapon;

        public bool changed = false;

        public Inventory()
        {
            weapons = new Dictionary<Weapon, int>();
            currency = 0;
        }

        public void nextItem(Agent a)
        {
            

            List<Weapon> itemList = weapons.Keys.ToList<Weapon>();

            if (equippedWeapon == null)
            {
                equippedWeapon = itemList[0];
                return;
            }

            equippedWeapon.Cancel(a);

            int index = itemList.IndexOf(equippedWeapon) + 1;

            if (index >= itemList.Count)
                index = 0;

            equippedWeapon = itemList[index];
        }

        public void prevItem(Agent a)
        {
            List<Weapon> itemList = weapons.Keys.ToList<Weapon>();

            if (equippedWeapon == null)
            {
                equippedWeapon = itemList[0];
                return;
            }

            equippedWeapon.Cancel(a);

            int index = itemList.IndexOf(equippedWeapon) - 1;

            if (index < 0)
                index = itemList.Count-1;

            equippedWeapon = itemList[index];
        }

        public void receiveItem(Weapon i)
        {
            if (weapons.ContainsKey(i))
            {
                weapons[i]++;
            }
            else
            {
                weapons.Add(i, 1);
                changed = true;
            }
        }

        public void receiveItem(Weapon i, int count)
        {
            if (weapons.ContainsKey(i))
            {
                weapons[i] += count;
            }
            else
            {
                weapons.Add(i, count);
                changed = true;
            }
        }

        public int getItemCount(Weapon i)
        {
            if(weapons.ContainsKey(i))
                return weapons[i];

            return 0;
        }

        public void removeItem(Weapon i)
        {
            if (weapons.ContainsKey(i))
            {
                weapons[i]--;
                if (weapons[i] <= 0)
                {
                    weapons.Remove(i);
                    changed = true;
                }
            }
        }

        public void removeItem(Weapon i, int count)
        {
            if (weapons.ContainsKey(i))
            {
                weapons[i] -= count;
                if (weapons[i] <= 0)
                {
                    weapons.Remove(i);
                    changed = true;
                }
            }
        }

        public void removeAllOfItem(Weapon i)
        {
            if (weapons.ContainsKey(i))
            {
                weapons.Remove(i);
                changed = true;
            }
        }

        public void clearInventory()
        {
            weapons.Clear();
            changed = true;
        }

        public void addCurrency(int number)
        {
            currency += number;
            if (currency < 0)
                currency = 0;
        }

        public List<Weapon> GetItemList()
        {
            changed = false;
            return weapons.Keys.ToList<Weapon>();
        }
    }
}
