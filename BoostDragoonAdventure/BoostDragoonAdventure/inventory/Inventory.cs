﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.inventory
{
    public class Inventory
    {

        private Dictionary<Weapon, int> weapons;
        private Dictionary<Consumable, int> consumables;
        private Dictionary<Part, int> parts;
        public int currency;

        public Weapon equippedWeapon;

        public bool changed = false;

        public Inventory()
        {
            weapons = new Dictionary<Weapon, int>();
            consumables = new Dictionary<Consumable, int>();
            parts = new Dictionary<Part, int>();
            currency = 0;
        }

        public void nextWeapon(Agent a)
        {
            List<Weapon> weaponList = weapons.Keys.ToList<Weapon>();

            if (equippedWeapon == null)
            {
                equippedWeapon = weaponList[0];
                return;
            }

            equippedWeapon.Cancel(a);

            int index = weaponList.IndexOf(equippedWeapon) + 1;

            if (index >= weaponList.Count)
                index = 0;

            equippedWeapon = weaponList[index];
        }

        public void prevWeapon(Agent a)
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


        public void receiveItem(Item i)
        {
            if (i is Weapon)
            {
                if (weapons.ContainsKey((Weapon)i))
                {
                    weapons[(Weapon)i]++;
                }
                else
                {
                    weapons.Add((Weapon)i, 1);
                    changed = true;
                }
            }
            else if (i is Consumable)
            {
                if (consumables.ContainsKey((Consumable)i))
                {
                    consumables[(Consumable)i]++;
                }
                else
                {
                    consumables.Add((Consumable)i, 1);
                    changed = true;
                }
            }
            else if (i is Part)
            {
                if (parts.ContainsKey((Part)i))
                {
                    parts[(Part)i]++;
                }
                else
                {
                    parts.Add((Part)i, 1);
                    changed = true;
                }
            }
            
        }

        public void receiveItem(Item i, int count)
        {
            if (i is Weapon)
            {
                if (weapons.ContainsKey((Weapon)i))
                {
                    weapons[(Weapon)i] += count;
                }
                else
                {
                    weapons.Add((Weapon)i, count);
                    changed = true;
                }
            }
            else if (i is Consumable)
            {
                if (consumables.ContainsKey((Consumable)i))
                {
                    consumables[(Consumable)i] += count;
                }
                else
                {
                    consumables.Add((Consumable)i, count);
                    changed = true;
                }
            }
            else if (i is Part)
            {
                if (parts.ContainsKey((Part)i))
                {
                    parts[(Part)i] += count;
                }
                else
                {
                    parts.Add((Part)i, count);
                    changed = true;
                }
            }
            
        }

        public int getItemCount(Item i)
        {
            if (i is Weapon)
            {
                if (weapons.ContainsKey((Weapon)i))
                    return weapons[(Weapon)i];
            }
            else if (i is Consumable)
            {
                if (consumables.ContainsKey((Consumable)i))
                    return consumables[(Consumable)i];
            }
            else if (i is Part)
            {
                if (parts.ContainsKey((Part)i))
                    return parts[(Part)i];
            }

            return 0;
        }

        public void removeItem(Item i)
        {
            if (i is Weapon)
            {
                if (weapons.ContainsKey((Weapon)i))
                {
                    weapons[(Weapon)i]--;
                    if (weapons[(Weapon)i] <= 0)
                    {
                        weapons.Remove((Weapon)i);
                        changed = true;
                    }
                }
            }
            else if (i is Consumable)
            {
                if (consumables.ContainsKey((Consumable)i))
                {
                    consumables[(Consumable)i]--;
                    if (consumables[(Consumable)i] <= 0)
                    {
                        consumables.Remove((Consumable)i);
                        changed = true;
                    }
                }
            }
            else if (i is Part)
            {
                if (parts.ContainsKey((Part)i))
                {
                    parts[(Part)i]--;
                    if (parts[(Part)i] <= 0)
                    {
                        parts.Remove((Part)i);
                        changed = true;
                    }
                }
            }
        }

        public void removeItem(Item i, int count)
        {
            if (i is Weapon)
            {
                weapons[(Weapon)i] -= count;
                if (weapons[(Weapon)i] <= 0)
                {
                    weapons.Remove((Weapon)i);
                    changed = true;
                }
            }
            else if (i is Consumable)
            {
                consumables[(Consumable)i] -= count;
                if (consumables[(Consumable)i] <= 0)
                {
                    consumables.Remove((Consumable)i);
                    changed = true;
                }
            }
            else if (i is Part)
            {
                parts[(Part)i] -= count;
                if (parts[(Part)i] <= 0)
                {
                    parts.Remove((Part)i);
                    changed = true;
                }
            }
        }

        public void removeAllOfItem(Item i)
        {
            if (i is Weapon)
            {
                if (weapons.ContainsKey((Weapon)i))
                {
                    weapons.Remove((Weapon)i);
                    changed = true;
                }
            }
            else if (i is Consumable)
            {
                if (consumables.ContainsKey((Consumable)i))
                {
                    consumables.Remove((Consumable)i);
                    changed = true;
                }
            }
            else if (i is Part)
            {
                if (parts.ContainsKey((Part)i))
                {
                    parts.Remove((Part)i);
                    changed = true;
                }
            }
            
        }

        public void clearInventory()
        {
            weapons.Clear();
            consumables.Clear();
            parts.Clear();
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

        public bool SellItem(Item i, int count)
        {
            if (getItemCount(i) > count)
            {
                addCurrency(i.value * count);
                removeItem(i, count);

                return true;
                //play cha-ching.wav
            }
            else
            {
                //play buzzer.wav, shouldn't be able to ask to sell for more than you have
                return false;
            }
        }
    }
}
