using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent.action;
using Microsoft.Xna.Framework;
using wickedcrush.particle;

namespace wickedcrush.inventory
{
    public static class InventoryServer
    {
        public static Dictionary<String, Weapon> weapons = new Dictionary<String, Weapon>();
        public static Dictionary<String, Consumable> consumables = new Dictionary<String, Consumable>();
        public static Dictionary<String, Part> parts = new Dictionary<String, Part>();

        public static Random random = new Random();


        public static void Initialize()
        {
            weapons = new Dictionary<String, Weapon>();
            consumables = new Dictionary<String, Consumable>();
            parts = new Dictionary<String, Part>();

            #region Weapons
            weapons.Add(
                "Spellbook: Fireball",
                new Weapon("Spellbook: Fireball", "The severed tooth of a Whirlwind Centibeast. Incinerates anything in its path.", (a, i) =>
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
                            SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -500);
                    }
                    else if (a.stats.get("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 1, 4, 4, 1, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20, 75, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -400);
                    }
                    else
                    {
                        
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 1, 4, 4, 0, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -10, 50, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -200);
                    }

                    a.stats.set("charge", 0);
                    
                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>()));

            weapons.Add(
                "Spellbook: Etheral Surge",
                new Weapon("Spellbook: Etheral Surge", "", (a, i) =>
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
                            SkillServer.GenerateProjectile(new Vector2(1f, 1f), new Vector2(300, 0), -20, 100, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -500);
                    }
                    else if (a.stats.get("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 35, 16, 16, 0, 340, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20, 75, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -400);
                    }
                    else
                    {

                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 35, 8, 8, 0, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -10, 50, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -200);
                    }

                    a.stats.set("charge", 0);

                },
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
                new List<KeyValuePair<string, int>>()));



            weapons.Add(
               "Knife",
               new Weapon("Knife", "A small blade marked with the royal crest of El Legante.", (a, i) =>
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
                       a.AddOverheadWeapon("Knife", "weapons", "knife", 0, new Vector2(0f, 130f), 0.3f, 90f);
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
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(500, 0), -20, 50, 400, null, "whsh", "knife", 0, "weapons", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -150);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                       //a.stats.addTo("boost", -100);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 1, 1, 0, 0, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(500, 0), -20, 50, 400, null, "whsh", "knife", 0, "weapons", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -150);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                       //a.stats.addTo("boost", -100);
                   }
                   else
                   {
                       a.weaponInUse = null;
                   }

                   a.RemoveOverheadWeapon("Knife");
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            weapons.Add(
               "Longsword",
               new Weapon("Longsword", "A weapon of this size could only be wielded with the strength of a combat frame.", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   //a.AddHudElement("warning", "warning", 4, Vector2.Zero);
                   a.AddOverheadWeapon("Longsword", "weapons", "sword", 0, new Vector2(0f, 130f), .4f, 90f);
                  
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
                           SkillServer.skills["Sword Attack"], false));
                       //a.useActionSkill(SkillServer.skills["Bigger Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   } else
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), new Vector2(0f, 0f), 200, 6, 3, 0, 45, true, 300f, 100, 0, 1f, null, "attack1", 3, "all",
                           SkillServer.skills["Sword Attack"], false));
                       //a.useActionSkill(SkillServer.skills["Biggest Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            weapons.Add(
               "Scattershot",
               new Weapon("Scattershot", "Highly compressed etheral tang. When released, travels at a high velocity over a wide range.", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   a.addTimer("scattertimer", 120);
                   a.getTimer("scattertimer").resetAndStart();
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);

                   if (a.getTimer("scattertimer").isDone())
                   {
                       a.getTimer("scattertimer").resetAndStart();
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), 100, 1, 1, 0, 45, true, 300f, 100, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, ((float)random.NextDouble() * 200f)-100f), -5, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, false), false));
                   }
               },
               (a, i) =>
               {
                   
                   a.removeTimer("scattertimer");
                   a.weaponInUse = null;
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            weapons.Add(
               "Rifle",
               new Weapon("Rifle", "A reliable long range weapon, able to accurately hit ranged targets.", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 0);
                   a.stats.addTo("boost", -50);
                   a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 10, 1, 1, 0, 15, true, 500f, 500, 0, 1f, null, "", 0, "",
                       SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));

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
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -125);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {

                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3, 3, 0, 15, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -100);
                   }
                   else
                   {
                       a.weaponInUse = null;

                   }
                   
                   a.stats.set("charge", 0);
               },
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            #endregion

            #region Consumables

            consumables.Add("Pizza", new Consumable("Pizza", "A mysterious food disk once thought to be a weapon.\n\nHP+60",
                (a, i) =>
                {
                    a.stats.addTo("hp", 60);

                    if (a.stats.get("hp") > a.stats.get("maxHP"))
                        a.stats.set("hp", a.stats.get("maxHP"));

                    a.factory.DisplayMessage("Devoured the Pizza!\nRestored 60 HP.");

                    a.stats.inventory.removeItem(i);
                }));

            consumables.Add("Pizza Pie", new Consumable("Pizza Pie", "A flaky crust emitting a distinct pizza scent.\n\nClick to open.",
                (a, i) =>
                {
                    //a.stats.addTo("hp", 60);

                    //if (a.stats.get("hp") > a.stats.get("maxHP"))
                        //a.stats.set("hp", a.stats.get("maxHP"));

                    

                    a.stats.inventory.removeItem(i);
                    a.stats.inventory.receiveItem(getConsumable("Pizza"));

                    a.factory.DisplayMessage("You dig into the crust with your grubby mitts.\nThere was a Pizza inside!");
                }));

            consumables.Add("Canned Pizza", new Consumable("Canned Pizza", "This ancient elixir is renowned for its nourishment of body and spirit.\n\nHP+40",
                (a, i) =>
                {
                    a.stats.addTo("hp", 40);

                    if (a.stats.get("hp") > a.stats.get("maxHP"))
                        a.stats.set("hp", a.stats.get("maxHP"));

                    a.stats.inventory.removeItem(i);

                    a.factory.DisplayMessage("You drink the Pizza...Refreshing!\nRestored 40 HP.");
                }));

            consumables.Add("Pizza Rolls", new Consumable("Pizza Rolls", "Twisted remains of a deceased pizza.\n\nHP+20",
                (a, i) =>
                {
                    a.stats.addTo("hp", 20);

                    if (a.stats.get("hp") > a.stats.get("maxHP"))
                        a.stats.set("hp", a.stats.get("maxHP"));

                    a.stats.inventory.removeItem(i);

                    a.factory.DisplayMessage("You bite into the Pizza Rolls!\nRestored 20 HP.");
                }));

            #endregion

            #region Parts

            parts.Add("Basic Core",
                new Part("Basic Core",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Circle, true)
                            },
                        new Dictionary<string, int>() {  })));

            parts.Add("Dinky Chamber",
                new Part("Dinky Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<string, int>() { { "hp", 1 } })));

            parts.Add("Pro-grade Chamber",
                new Part("Pro-grade Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Square, true)},
                        new Dictionary<string, int>() { { "hp", 2 } })));

            parts.Add("Kinetic Chamber",
                new Part("Kinetic Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Circle, true), 
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true) },
                        new Dictionary<string, int>() { { "hp", 1 } })));

            parts.Add("High Performance Chamber",
                new Part("High Performance Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true) },
                        new Dictionary<string, int>() { {"hp", 3} })));

            parts.Add("Dinky Belt",
                new Part("Dinky Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<string, int>() { { "ep", 1 } })));

            parts.Add("Pro-grade Belt",
                new Part("Pro-grade Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<string, int>() { { "ep", 2 } })));

            parts.Add("Kinetic Belt",
                new Part("Kinetic Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Triangle, true) },
                        new Dictionary<string, int>() { { "ep", 1 } })));

            parts.Add("High Performance Belt",
                new Part("High Performance Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Triangle, true) },
                        new Dictionary<string, int>() { { "ep", 1 } })));

            #endregion
        }
        
        public static Weapon getWeapon(String name)
        {
            return weapons[name];
        }

        public static Consumable getConsumable(String name)
        {
            return consumables[name];
        }

        public static Part getPart(String name)
        {
            return parts[name];
        }

        public static Weapon getRandomWeapon()
        {
            return weapons.Values.ToList<Weapon>()[random.Next(weapons.Count)];
        }

        public static Item getRandomItem()
        {
            List<Item> tempList = new List<Item>();

            foreach (KeyValuePair<string, Consumable> pair in consumables)
                tempList.Add(pair.Value);

            foreach (KeyValuePair<string, Part> pair in parts)
                tempList.Add(pair.Value);

            return tempList[random.Next(tempList.Count)];
        }
    }
}
