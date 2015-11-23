using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using wickedcrush.inventory;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.stats
{
    public class PersistedStats
    {
        public Dictionary<String, int> numbers;

        public Inventory inventory; 

        public PersistedStats()
        {
            numbers = new Dictionary<String, int>();
            inventory = new Inventory();
        }

        public PersistedStats(int maxHP, int currentHP)
        {
            numbers = new Dictionary<String, int>();
            inventory = new Inventory();

            set("maxHP", maxHP);
            set("hp", currentHP);
            set("staggerLimit", 100);
            set("staggerDuration", 50);
            set("stagger", 0);
            
        }

        public int get(String key)
        {
            if(numbers.ContainsKey(key))
                return numbers[key];

            return 0;
            //throw new InvalidOperationException("That number... " + key + "... does not exist!!! I cannot return it! Sorry.");
        }

        public void set(String key, int value)
        {
            if (numbers.ContainsKey(key))
            {
                numbers[key] = value;
            }
            else
            {
                numbers.Add(key, value);
            }
        }

        public bool numbersContainsKey(String key)
        {
            return numbers.ContainsKey(key);
        }
        
        public void addTo(String key, int number)
        {
            if (checkExists(key))
                numbers[key] += number;
            else
                numbers.Add(key, number);
        }

        public bool requirementMet(String key, int value)
        {
            if (!numbersContainsKey(key))
                return false;

            return (get(key) > value);
                
        }

        // -1: number smaller than value, 0: number equal to value, 1: number greater than value
        public int compare(String key, int value) 
        {
            if (!checkExists(key))
                return -2;

            return get(key).CompareTo(value);
        }

        // -1: first smaller than second, 0: first equal to second, 1: first greater than second
        public int compare(String key, String otherKey)
        {
            if (!checkExists(key) || !checkExists(otherKey))
                return -2;

            return get(key).CompareTo(get(otherKey));
        }

        private bool checkExists(String key)
        {
            if (numbers.ContainsKey(key))
            {
                return true;
            }
            return false;
            //throw new InvalidOperationException("That number... " + key + "... does not exist!!! Sorry.");
        }
    }
}
