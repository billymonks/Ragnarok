using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.inventory
{
    public static class ItemServer
    {
        public static Dictionary<String, Item> items = new Dictionary<String, Item>();
        public static Random random = new Random();


        public static void Initialize()
        {
            items = new Dictionary<String, Item>();
            
            items.Add(
                "Healthsweed", 
                new Item("Healthsweed", (a, i) =>
                {
                    a.stats.set("hp", a.stats.get("maxHP"));
                    a.stats.inventory.removeItem(i);
                }, 
                (a, i) => { },
                (a, i) => { },
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>()));

            items.Add(
                "Spellbook: Fireball",
                new Item("Spellbook: Fireball", (a, i) =>
                {
                    
                },
                (a, i) => { a.stats.addTo("boost", -5); },
                (a, i) => {
                    a.fireFireball(2, 10f, 20, 300);
                    a.stats.addTo("boost", -100);
                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100)}),
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>()));

            /*items.Add("Spellbook: Fireball", new Item("Spellbook: Fireball", ItemType.UsesFuelCharge,
                (a, i) => {
                    a.fireFireball(2, 10f, 20, 300);
                }, 200, 400, 300));*/
        }
        
        public static Item getItem(String name)
        {
            return items[name];
        }

        public static Item getRandomItem()
        {
            return items.Values.ToList<Item>()[random.Next(items.Count)];
        }
    }
}
