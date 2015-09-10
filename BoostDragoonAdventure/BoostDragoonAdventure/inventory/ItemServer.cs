﻿using System;
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
                    a.itemInUse = null;
                },
                (a, i) => { a.itemInUse = null; },
                (a, i) => 
                { 
                    a.itemInUse = null;
                    
                
                },
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>()));

            items.Add("Remote Transmitter",
                new Item("Remote Transmitter", (a, i) =>
                {
                    a.PlayCue("volleyball");
                    a.stats.set("charge", 0);
                    a.AddHudElement("warning", "warning", 4, Vector2.Zero);
                    a.AddOverheadWeapon("Remote", "trap", "unpressed_002", 0, new Vector2(0f, 0f), 0.1f, 0f);
                },
                (a, i) =>
                {
                    a.stats.addTo("boost", -3);
                    a.stats.addTo("charge", 1);

                    if (a.stats.compare("charge", 25) == 0)
                    {
                        a.PlayCue("squash");
                        //a.AddOverheadWeapon("Knife", "weapons", "Knife", 0, new Vector2(0f, 80f), 2f);
                    }

                    if (a.stats.compare("charge", 50) == 0)
                        a.PlayCue("ping2");

                    if (a.stats.get("charge") < 50)
                    {
                        a.activeRange = a.stats.get("charge") * 5;
                        a.ScaleOverheadWeapon("Remote", a.activeRange / 43f);
                    }
                    else
                    {
                        a.activeRange = 250;
                        a.ScaleOverheadWeapon("Remote", a.activeRange / 43f);
                    }

                    if (a.stats.compare("charge", 75) == 0)
                    {
                        i.Release(a);
                    }
                },
                (a, i) =>
                {
                    a.RemoveHudElement("warning");
                    a.RemoveOverheadWeapon("Remote");
                    //a.PlayCue("smash");
                    

                    if (a.stats.compare("charge", 25) < 0)
                    {
                        //a.useActionSkill(SkillServer.skills["Sword Attack"]);
                        //a.TriggerInProximity();
                        
                    }
                    else if (a.stats.compare("charge", 50) < 0)
                    {
                        //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                        //a.TriggerInProximity();
                    }
                    else
                    {
                        //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                        
                    }
                    a.PlayCue("Jump7");
                    a.TriggerInProximity();
                    a.stats.addTo("boost", -100);
                    a.stats.set("charge", 0);
                    a.itemInUse = null;
                },
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>(),
                new List<KeyValuePair<string, int>>()));

            items.Add(
                "Spellbook: Fireball",
                new Item("Spellbook: Fireball", (a, i) =>
                {
                    a.stats.set("charge", 0);
                },
                (a, i) => { 
                    a.stats.addTo("boost", -3);
                    a.stats.addTo("charge", 1);

                    if (a.stats.compare("charge", 25) == 0)
                    {
                        a.PlayCue("squash");
                    }

                    if (a.stats.compare("charge", 50) == 0)
                        a.PlayCue("ping2");

                    if (a.stats.compare("charge", 75) == 0)
                    {
                        i.Release(a);
                    }
                },
                (a, i) => {
                    if (a.stats.get("charge") > 50)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(600f, 0f), 1, 4, 4, 1, 270, false, 600f, 800, 600, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -500);
                    }
                    else if (a.stats.get("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 1, 4, 4, 1, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20, 75, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -400);
                    }
                    else
                    {
                        
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 1, 4, 4, 0, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -10, 50, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -200);
                    }

                    a.stats.set("charge", 0);
                    
                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100)}),
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>()));

            items.Add(
                "Spellbook: Etheral Surge",
                new Item("Spellbook: Etheral Surge", (a, i) =>
                {
                    a.stats.set("charge", 0);
                },
                (a, i) =>
                {
                    a.stats.addTo("boost", -3);
                    a.stats.addTo("charge", 1);

                    if (a.stats.compare("charge", 25) == 0)
                    {
                        a.PlayCue("squash");
                    }

                    if (a.stats.compare("charge", 50) == 0)
                        a.PlayCue("ping2");

                    if (a.stats.compare("charge", 75) == 0)
                    {
                        i.Release(a);
                    }
                },
                (a, i) =>
                {
                    if (a.stats.get("charge") > 50)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(600f, 0f), 35, 32, 16, 0, 340, false, 600f, 800, 600, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(1f, 1f), new Vector2(300, 0), -20, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -500);
                    }
                    else if (a.stats.get("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 35, 16, 16, 0, 340, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20, 75, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -400);
                    }
                    else
                    {

                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 35, 8, 8, 0, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -10, 50, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                        a.stats.addTo("boost", -200);
                    }

                    a.stats.set("charge", 0);

                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 100) }),
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

                   if (a.stats.compare("charge", 25) == 0)
                   {
                       a.PlayCue("squash");
                   }

                   if (a.stats.compare("charge", 50) == 0)
                   {
                       a.PlayCue("ping2");
                       
                   }

                   if (a.stats.compare("charge", 75) == 0)
                   {
                       i.Release(a);
                   }
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   }
                   else
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
               "Knife",
               new Item("Knife", (a, i) =>
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
                       a.AddOverheadWeapon("Knife", "weapons", "knife", 0, new Vector2(0f, 80f), 0.2f, 90f);
                   }

                   if (a.stats.compare("charge", 50) == 0)
                   {
                       a.PlayCue("ping2");
                   }

                   if (a.stats.compare("charge", 75) == 0)
                   {
                       i.Release(a);
                   }

                   
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 10, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(500, 0), -20, 50, 400, null, "whsh", "knife", 0, "weapons", Vector2.Zero)));
                       a.stats.addTo("boost", -150);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                       //a.stats.addTo("boost", -100);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 1, 1, 0, 0, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(500, 0), -20, 50, 400, null, "whsh", "knife", 0, "weapons", Vector2.Zero)));
                       a.stats.addTo("boost", -150);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                       //a.stats.addTo("boost", -100);
                   }
                   else
                   {
                       a.itemInUse = null;
                   }

                   a.RemoveOverheadWeapon("Knife");
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
                   //a.AddHudElement("warning", "warning", 4, Vector2.Zero);
                   a.AddOverheadWeapon("Longsword", "weapons", "sword", 0, new Vector2(0f, 80f), .3f, 90f);
                  
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if (a.stats.compare("charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");

                   if (a.stats.compare("charge", 75) == 0)
                   {
                       i.Release(a);
                   }

               },
               (a, i) =>
               {
                   //a.RemoveHudElement("warning");
                   a.RemoveOverheadWeapon("Longsword");
                   a.PlayCue("smash");

                   if (a.stats.compare("charge", 25) < 0)
                   {
                       a.useActionSkill(SkillServer.skills["Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Weak"]);
                   }
                   else if (a.stats.compare("charge", 50) < 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), new Vector2(0f, 0f), 200, 3, 3, 0, 45, true, 300f, 100, 0, 1f, null, "attack1", 3, "all",
                           SkillServer.skills["Sword Attack"]));
                       //a.useActionSkill(SkillServer.skills["Bigger Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   } else
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), new Vector2(0f, 0f), 200, 5, 5, 0, 90, true, 300f, 100, 0, 1f, null, "attack1", 3, "all",
                           SkillServer.skills["Sword Attack"]));
                       //a.useActionSkill(SkillServer.skills["Biggest Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
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
                   
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if(a.stats.compare("charge", 25) == 0)
                       a.PlayCue("squash");

                   if (a.stats.compare("charge", 50) == 0)
                       a.PlayCue("ping2");

                   if (a.stats.compare("charge", 75) == 0)
                   {
                       i.Release(a);
                   }
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 70, 8, 8, 0, 45, true, 300f, 100, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero)));
                       a.stats.addTo("boost", -150);
                       a.stats.set("charge", 0);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       //a.useActionSkill(SkillServer.GenerateSkillStruct(10, 0, 0, 1, 1, 0, 0));
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 45, true, 300f, 100, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero)));
                       a.stats.addTo("boost", -100);
                       a.stats.set("charge", 0);

                   }
                   else
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 10, 1, 1, 0, 0, true, 300f, 300, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero)));
                       a.stats.addTo("boost", -100);
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
                   a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 10, 1, 1, 0, 15, true, 500f, 500, 0, 1f, null, "", 0, "",
                       SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
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

                   if (a.stats.compare("charge", 75) == 0)
                   {
                       i.Release(a);
                   }
               },
               (a, i) =>
               {
                   if (a.stats.compare("charge", 50) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 70, 5, 5, 0, 0, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                       a.stats.addTo("boost", -200);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {

                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 15, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero)));
                       a.stats.addTo("boost", -150);
                   }
                   else
                   {
                       a.itemInUse = null;

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
