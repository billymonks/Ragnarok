using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using wickedcrush.inventory;
using wickedcrush.entity.physics_entity.agent;

namespace wickedcrush.stats
{
    public enum GearStat
    {
        MaxHP = 0,
        MaxEP = 1,
        PhysicalDMG = 2,
        EtheralDMG = 3,
        HPConversion = 4,
        EPConversion = 5,
        EPRefill = 6,
        BoostSpeed = 7,
        Potency = 8
    }
    public class PersistedStats
    {
        public Dictionary<String, int> numbers;
        public Dictionary<String, String> strings;

        public Inventory inventory;

        

        public PersistedStats()
        {
            numbers = new Dictionary<String, int>();
            strings = new Dictionary<String, String>();
            inventory = new Inventory();

        }

        public PersistedStats(int maxHP, int currentHP)
        {
            numbers = new Dictionary<String, int>();
            strings = new Dictionary<String, String>();
            inventory = new Inventory();

            set("MaxHP", maxHP);
            set("hp", currentHP);
            set("staggerLimit", 100);
            set("staggerDuration", 50);
            set("stagger", 0);
            
        }

        public int getNumber(String key)
        {
            if(numbers.ContainsKey(key))
                return numbers[key];

            return 0;
            //throw new InvalidOperationException("That number... " + key + "... does not exist!!! I cannot return it! Sorry.");
        }

        public String getString(String key)
        {
            if (strings.ContainsKey(key))
                return strings[key];

            return null;
            //throw new InvalidOperationException("That string... " + key + "... does not exist!!! I cannot return it! Sorry.");
        }

        public void ApplyStats()
        {
            set("MaxHP", (10 + inventory.gear.GetGearStat(GearStat.MaxHP)) * 10);
            set("maxBoost", (60 + inventory.gear.GetGearStat(GearStat.MaxEP)) * 10);
            set("PhysicalDMG", inventory.gear.GetGearStat(GearStat.PhysicalDMG));
            set("EtheralDMG", inventory.gear.GetGearStat(GearStat.EtheralDMG));
            set("HPConversion", (1 + inventory.gear.GetGearStat(GearStat.HPConversion)));
            set("EPConversion", (10 + inventory.gear.GetGearStat(GearStat.EPConversion) * 9));
            set("fillSpeed", inventory.gear.GetGearStat(GearStat.EPRefill));
            set("boostSpeedMod", inventory.gear.GetGearStat(GearStat.BoostSpeed));
            set("potency", inventory.gear.GetGearStat(GearStat.Potency));

            EnforceMaxStats();
        }

        public void EnforceMaxStats()
        {
            if (compare("hp", "MaxHP") == 1)
                set("hp", getNumber("MaxHP"));

            if (compare("boost", "maxBoost") == 1)
                set("boost", getNumber("maxBoost"));
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

        public void set(String key, String value)
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

        public bool numbersContainsKey(String key)
        {
            return numbers.ContainsKey(key);
        }

        public bool stringsContainsKey(String key)
        {
            return strings.ContainsKey(key);
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

            return (getNumber(key) > value);
                
        }

        // -1: number smaller than value, 0: number equal to value, 1: number greater than value
        public int compare(String key, int value) 
        {
            if (!checkExists(key))
                return -2;

            return getNumber(key).CompareTo(value);
        }

        // -1: first smaller than second, 0: first equal to second, 1: first greater than second
        public int compare(String key, String otherKey)
        {
            if (!checkExists(key) || !checkExists(otherKey))
                return -2;

            return getNumber(key).CompareTo(getNumber(otherKey));
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
