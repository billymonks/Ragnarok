using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using wickedcrush.inventory;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.stats
{
    public class PersistedStats //totally could have been struct
    {
        private Dictionary<String, int> numbers;
        //private Dictionary<String, String> strings;

        public Inventory inventory; 

        public PersistedStats()
        {
            numbers = new Dictionary<String, int>();
        }

        public PersistedStats(int maxHP, int currentHP)
        {
            numbers = new Dictionary<String, int>();

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

            throw new InvalidOperationException("That number... " + key + "... does not exist!!! I cannot return it! Sorry.");
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
        
        public void addTo(String key, int number)
        {
            checkExists(key);

            numbers[key] += number;
        }

        // -1: number smaller than value, 0: number equal to value, 1: number greater than value
        public int compare(String key, int value) 
        {
            checkExists(key);

            return get(key).CompareTo(value);
        }

        // -1: first smaller than second, 0: first equal to second, 1: first greater than second
        public int compare(String key, String otherKey)
        {
            checkExists(key);

            return get(key).CompareTo(get(otherKey));
        }

        private void checkExists(String key)
        {
            if (numbers.ContainsKey(key))
            {
                return;
            }

            throw new InvalidOperationException("That number... " + key + "... does not exist!!! Sorry.");
        }
    }
}
