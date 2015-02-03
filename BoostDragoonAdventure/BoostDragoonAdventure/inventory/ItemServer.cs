using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent.action;

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

            items.Add(
               "Shortsword",
               new Item("Shortsword", (a, i) =>
               {
                   a.stats.set("shortsword charge", 0);
               },
               (a, i) => 
               { 
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("shortsword charge", 1);
               },
               (a, i) =>
               {
                   if (a.stats.compare("shortsword charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Strong Attack"]);
                   }
                   else
                   {
                       a.useActionSkill(SkillServer.skills["Weak Attack"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("shortsword charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Spear",
               new Item("Spear", (a, i) =>
               {
                   a.stats.set("spear charge", 0);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("spear charge", 1);
               },
               (a, i) =>
               {
                   if (a.stats.compare("spear charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Spear Attack Weak"]);
                   }
                   else
                   {
                       a.useActionSkill(SkillServer.skills["Spear Attack Full"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("spear charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Longsword",
               new Item("Longsword", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("longsword charge", 0);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("longsword charge", 1);

                   if (a.stats.compare("longsword charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("longsword charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   a.PlayCue("smash");

                   if (a.stats.compare("longsword charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Weak"]);
                   }
                   else if (a.stats.compare("longsword charge", 50) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   } else
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("longsword charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));
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
