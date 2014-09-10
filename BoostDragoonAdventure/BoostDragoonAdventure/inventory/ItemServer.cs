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
            items.Add("Healthsweed", new Item("Healthsweed", ItemType.Consumable, a => a.stats.set("hp", a.stats.get("maxHP"))));
            items.Add("Fireball", new Item("Spellbook: Fireball", ItemType.UsesFuelCharge,
                a => {
                    a.fireBolt();
                }, 200, 400, 300));
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
