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
                (a, i) => { a.stats.addTo("boost", -3); },
                (a, i) => {
                    //a.fireFireball(2, 10f, 20, 500);
                    a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 10, 4, 4, 1, 180, false, 600f, 600, 400, null));
                    a.stats.addTo("boost", -10);
                    //a.stats.inventory.removeItem(i);
                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100)}),
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>()));

            items.Add(
               "Shortsword",
               new Item("Shortsword", (a, i) =>
               {
                   a.stats.set("charge", 0);
               },
               (a, i) => 
               { 
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Weak Attack"]);
                   }
                   else
                   {
                       a.useActionSkill(SkillServer.skills["Strong Attack"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Spear",
               new Item("Spear", (a, i) =>
               {
                   a.stats.set("charge", 0);
                   a.useActionSkill(SkillServer.skills["Forward Attack"]);
                   a.stats.addTo("boost", -50);
                   
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if (a.stats.compare("charge", 25) == 0)
                   {
                       a.PlayCue("squash");
                       a.AddOverheadWeapon("knife", "weapons", "knife", 0, new Vector2(0f, 80f), 2f);
                   }

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                       a.stats.addTo("boost", -100);
                       a.RemoveOverheadWeapon("knife");
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                       a.stats.addTo("boost", -100);
                       a.RemoveOverheadWeapon("knife");
                   }
                   
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Longsword",
               new Item("Longsword", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   a.AddHudElement("warning", "warning", 4, Vector2.Zero);
                   a.AddOverheadWeapon("longsword", "weapons", "sword", 0, new Vector2(0f, 80f), 3f);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if (a.stats.compare("charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   a.RemoveHudElement("warning");
                   a.RemoveOverheadWeapon("longsword");
                   a.PlayCue("smash");

                   if (a.stats.compare("charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Weak"]);
                   }
                   else if (a.stats.compare("charge", 50) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   } else
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Scattershot",
               new Item("Scattershot", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 10, 1, 1, 0, 0, true, 300f, 300, 0, null));
                   a.stats.addTo("boost", -100);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if(a.stats.compare("charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 70, 8, 8, 0, 45, true, 300f, 100, 0, null));
                       a.stats.addTo("boost", -150);
                       a.stats.set("charge", 0);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       //a.useActionSkill(SkillServer.GenerateSkillStruct(10, 0, 0, 1, 1, 0, 0));
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 45, true, 300f, 100, 0, null));
                       a.stats.addTo("boost", -100);
                       a.stats.set("charge", 0);

                   }
                   
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            items.Add(
               "Rifle",
               new Item("Rifle", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   //a.useActionSkill(SkillServer.GenerateSkillStruct(10, 0, 0, 1, 1, 0));
                   //a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 1, 1, 1, 0, 0, true, 300f, 1200, 0, null));
                   //a.stats.addTo("boost", -100);
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if (a.stats.compare("charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 70, 5, 5, 0, 0, true, 500f, 500, 0, null));
                       a.stats.addTo("boost", -200);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {

                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 15, true, 500f, 500, 0, null));
                       a.stats.addTo("boost", -150);
                   }
                   else
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 1, 1, 0, 15, true, 500f, 500, 0, null));
                   }
                   
                   a.stats.set("charge", 0);
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
