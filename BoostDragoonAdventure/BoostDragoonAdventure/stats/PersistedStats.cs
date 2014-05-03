using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace wickedcrush.stats
{
    public class PersistedStats //totally could have been struct
    {
        private Dictionary<String, int> numbers;
        //private Dictionary<String, String> strings;

        public PersistedStats()
        {
            numbers = new Dictionary<String, int>();
        }

        public PersistedStats(int maxHP, int currentHP)
        {
            numbers = new Dictionary<String, int>();

            numbers.Add("maxHP", maxHP);
            numbers.Add("hp", currentHP);
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

        /*public String getString(String key)
        {
            if(strings.ContainsKey(key))
                return strings[key];

            throw new InvalidOperationException("That string... " + key + "... does not exist!!! I cannot return it! Sorry.");
        }

        public void setString(String key, String value)
        {
            if (strings.ContainsKey(key))
            {
                strings[key] = value;
            }
            else
            {
                strings.Add(key, value);
            }
        }*/

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
