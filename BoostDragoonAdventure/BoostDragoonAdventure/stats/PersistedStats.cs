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
        private Dictionary<String, String> strings;

        public PersistedStats()
        {
            numbers = new Dictionary<String, int>();
        }

        public PersistedStats(int maxHP, int currentHP, int maxCharge)
        {
            numbers = new Dictionary<String, int>();

            numbers.Add("maxHP", maxHP);
            numbers.Add("hp", currentHP);
            numbers.Add("maxCharge", maxHP);
        }

        public int getNumber(String key)
        {
            if(numbers.ContainsKey(key))
                return numbers[key];

            throw new InvalidOperationException("That number... " + key + "... does not exist!!! I cannot return it! Sorry.");
        }

        public void setNumber(String key, int value)
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

        public String getString(String key)
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
        }

        public void addToNumber(String key, int number)
        {
            if (!numbers.ContainsKey(key))
            {
                Debug.Print("That number... " + key + "... does not exist!!! I cannot add " + number + " to it! Sorry.");
                return;
            }

            numbers[key] += number;
        }
    }
}
