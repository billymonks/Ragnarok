using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wickedcrush.inventory
{
    public static class ItemServer
    {
        public static Dictionary<String, Item> items = new Dictionary<String, Item>();

        public static void Initialize()
        {
            items = new Dictionary<String, Item>();
            items.Add("Healthsweed", new Item("Healthsweed", a => a.stats.set("hp", a.stats.get("maxHP"))));
        }
        
        public static Item getItem(String name)
        {
            return items[name];
        }
    }
}
