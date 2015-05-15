using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent.action;
using Microsoft.Xna.Framework;
using wickedcrush.particle;

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
                    ParticleStruct ps = new ParticleStruct(new Vector3(a.pos.X + a.center.X, 20, a.pos.Y + a.center.Y), Vector3.Zero, new Vector3(-0.5f, -1f, -0.5f), new Vector3(1f, 2f, 1f), new Vector3(0, .03f, 0), 0f, 0f, 1000, "particles", 0, "white_to_green");
                    a.EmitParticles(ps, 10);
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
                    a.fireFireball(2, 10f, 20, 500);
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
                       a.useActionSkill(SkillServer.skills["Weak Attack"]);
                   }
                   else
                   {
                       a.useActionSkill(SkillServer.skills["Strong Attack"]);
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
                   a.useActionSkill(SkillServer.skills["Forward Attack"]);
                   a.stats.addTo("boost", -50);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("spear charge", 1);

                   if (a.stats.compare("spear charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("spear charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("spear charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                       a.stats.addTo("boost", -100);
                   }
                   else if (a.stats.compare("spear charge", 25) > 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                       a.stats.addTo("boost", -100);
                   }
                   
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
                   a.AddHudElement("warning", "warning", 4, Vector2.Zero);
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
                   a.RemoveHudElement("warning");
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

            items.Add(
               "Scattershot",
               new Item("Scattershot", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("scatter charge", 0);
                   a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 10, 1, 1, 0, 0, true, 300f, 600));
                   a.stats.addTo("boost", -100);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("scatter charge", 1);

                   if(a.stats.compare("scatter charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("scatter charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("scatter charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 70, 8, 8, 0, 45, true, 300f, 600));
                       a.stats.addTo("boost", -150);
                       a.stats.set("scatter charge", 0);
                   }
                   else if (a.stats.compare("scatter charge", 25) > 0)
                   {
                       //a.useActionSkill(SkillServer.GenerateSkillStruct(10, 0, 0, 1, 1, 0, 0));
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 100, 3, 3, 0, 45, true, 300f, 600));
                       a.stats.addTo("boost", -100);
                       a.stats.set("scatter charge", 0);

                   }
                   
                   a.stats.set("scatter charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Rifle",
               new Item("Rifle", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("rifle charge", 0);
                   //a.useActionSkill(SkillServer.GenerateSkillStruct(10, 0, 0, 1, 1, 0));
                   a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 0, 1, 1, 0, 0, true, 300f, 1200));
                   a.stats.addTo("boost", -100);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("rifle charge", 1);

                   if (a.stats.compare("rifle charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("rifle charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("rifle charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 70, 5, 5, 0, 0, true, 300f, 1200));
                       a.stats.addTo("boost", -200);
                   }
                   else if (a.stats.compare("rifle charge", 25) > 0)
                   {

                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), 0, 100, 3, 3, 0, 15, true, 300f, 1200));
                       a.stats.addTo("boost", -150);
                   }
                   
                   a.stats.set("rifle charge", 0);
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
