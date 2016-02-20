using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.entity.physics_entity.agent.action;
using Microsoft.Xna.Framework;
using wickedcrush.particle;
using wickedcrush.stats;

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
                    int clusters = 3 + a.potency;
                    int radius = 360 - (360 / (clusters));
                    if (a.stats.getNumber("charge") > 50)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(600f, 0f), 1, clusters, clusters, 1, radius, false, 600f, 800, 600, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(15f, 15f), new Vector2(300, 0), -25 - a.etheralDMG, 100, 500, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -500);
                    }
                    else if (a.stats.getNumber("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 1, clusters, clusters, 1, radius, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20 - a.etheralDMG, 75, 500, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -400);
                    }
                    else
                    {


                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 1, clusters, clusters, 0, radius, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -10 - a.etheralDMG, 50, 500, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
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
                    if (a.stats.getNumber("charge") > 50)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(600f, 0f), 35, 16 + a.potency * 4, 16, 0, 340, false, 600f, 800, 600, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(300, 0), -20 - a.etheralDMG, 100, 800, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -400 - (a.potency * 75));
                    }
                    else if (a.stats.getNumber("charge") > 25)
                    {
                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(500f, 0f), 35, 8 + a.potency * 4, 16, 0, 340, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(300, 0), -15 - a.etheralDMG, 75, 500, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -300 - (a.potency * 75));
                    }
                    else
                    {

                        a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(400f, 0f), 35, 8 + a.potency, 8, 0, 270, false, 600f, 600, 400, 0.3f, ParticleServer.GenerateParticle(), "attack1", 3, "all",
                            SkillServer.GenerateProjectile(new Vector2(8f, 8f), new Vector2(300, 0), -10 - a.etheralDMG, 50, 300, ParticleServer.GenerateParticle(), "whsh", "attack1", 3, "all", Vector2.Zero, true), true));
                        a.stats.addTo("boost", -200 - (a.potency * 25));
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
                   a.useActionSkill(new SkillStruct("Forward Attack",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(1f, 0f),
                    new Vector2(50f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15 - a.physicalDMG) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));
                   a.stats.addTo("boost", -50);
                   
                   
               },
               (a, i) =>
               {
                   a.stats.addTo("boost", -3);
                   a.stats.addTo("charge", 1);

                   if (a.stats.compare("charge", 25) == 0)
                   {
                       a.PlayCue("squash");
                       a.UpdateOffset("LeftElbow", new Vector3(13f, 75f, -25f));
                       a.UpdateOffset("RightElbow", new Vector3(13f, 75f, 25f));
                       a.UpdateOffset("LeftHand", new Vector3(20f, 95f, -5f));
                       a.UpdateOffset("RightHand", new Vector3(20f, 95f, 5f));
                       a.AddOverheadWeapon("Knife", "weapons", "knife", 0, new Vector3(10f, 110f, 0f), 0, 0.3f, 140f);
                       a.RemoveAngledElement("KnifeFront");
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
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3 + a.potency, 1, 0, 10, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(800, 0), -20 - a.physicalDMG, 50, 1000, null, "whsh", "knife", 0, "weapons", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -50 - (a.potency * 25));
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                       //a.stats.addTo("boost", -100);
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 1 + a.potency, 1, 0, 0, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(20f, 20f), new Vector2(800, 0), -20 - a.physicalDMG, 50, 600, null, "whsh", "knife", 0, "weapons", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -50 - (a.potency * 15));
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
               (a, i) =>
               {
                   a.UpdateOffset("LeftElbow", new Vector3(10f, 25f, -20f));
                   a.UpdateOffset("RightElbow", new Vector3(10f, 25f, 20f));
                   a.UpdateOffset("LeftHand", new Vector3(15f, 30f, 5f));
                   a.UpdateOffset("RightHand", new Vector3(15f, 30f, -5f));
                   a.AddAngledElement("KnifeFront", "weapons", "knife", 0, new Vector3(10f, 20f, 10f), 0, .3f, 280f, new Vector3(0f, -10f, 0f));
               },
               (a, i) =>
               {
                   a.RemoveAngledElement("KnifeFront");
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

                   a.UpdateOffset("LeftElbow", new Vector3(10f, 75f, -25f));
                   a.UpdateOffset("RightElbow", new Vector3(10f, 75f, 25f));
                   a.UpdateOffset("LeftHand", new Vector3(15f, 95f, -5f));
                   a.UpdateOffset("RightHand", new Vector3(15f, 95f, 5f));
                   a.AddAngledElement("LongswordAbove", "weapons", "sword", 0, new Vector3(15f, 140f, 0f), 0, .4f, 100f, new Vector3(0f, 20f, 0f));
                   
                   //a.RemoveAngledElement("LongswordFront");
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
                   a.RemoveAngledElement("LongswordAbove");
                   //a.AddAngledElement("LongswordFront", "weapons", "sword", 0, new Vector3(20f, 60f, 0f), 0, .4f, 90f);
                   a.PlayCue("smash");

                   SkillStruct swordAttack = new SkillStruct("Sword Attack",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(40f, 40f),
                    new Microsoft.Xna.Framework.Vector2(0f, 20f),
                    Vector2.Zero,
                    new Vector2(40f, 0f),
                    190,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -20 - a.physicalDMG) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    1,
                    "sword",
                    false);

                   swordAttack.piercing = true;

                   if (a.stats.compare("charge", 25) < 0)
                   {
                       a.useActionSkill(swordAttack);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Weak"]);
                   }
                   else if (a.stats.compare("charge", 50) < 0)
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), new Vector2(0f, 0f), 200, 3, 3, 0, 45, true, 300f, 100, 0, 1f, null, "", 0, "",
                           swordAttack, false));
                       //a.useActionSkill(SkillServer.skills["Bigger Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Medium"]);
                   } else
                   {
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0, 0f), new Vector2(0f, 0f), 200, 6, 3, 0, 45, true, 300f, 100, 0, 1f, null, "", 0, "",
                           swordAttack, false));
                       //a.useActionSkill(SkillServer.skills["Biggest Sword Attack"]);
                       //a.useActionSkill(SkillServer.skills["Longsword Attack Full"]);
                   }
                   a.stats.addTo("boost", -100);
                   a.stats.set("charge", 0);
               },
               (a, i) =>
               {
                   a.UpdateOffset("LeftHand", new Vector3(40f, 40f, -5f));
                   a.UpdateOffset("RightHand", new Vector3(40f, 40f, 5f));
                   a.UpdateOffset("LeftElbow", new Vector3(20f, 35f, -15f));
                   a.UpdateOffset("RightElbow", new Vector3(20f, 35f, 15f));
                   a.AddAngledElement("LongswordFront", "weapons", "sword", 0, new Vector3(40f, 80f, 0f), 0, .4f, 80f, new Vector3(0f, 20f, 0f));
               },
               (a, i) =>
               {
                   a.RemoveAngledElement("LongswordFront");
               },

               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>(new KeyValuePair<string, int>[] { new KeyValuePair<string, int>("boost", 0) }),
               new List<KeyValuePair<string, int>>()));

            weapons.Add(
               "Scattershot",
               new Weapon("Scattershot", "Highly compressed etheral tang. When released, travels at a high velocity over a wide range.", (a, i) =>
               {
                   a.PlayCue("volleyball");
                   a.stats.set("charge", 1);
                   a.addTimer("scattertimer", 120 + 4500);
                   a.getTimer("scattertimer").resetAndStart();
               },
               (a, i) =>
               {
                   a.stats.addTo("charge", 1);

                   a.getTimer("scattertimer").setInterval(120 + 4500 / Math.Min(a.stats.getNumber("charge"), 100));

                   if (a.getTimer("scattertimer").isDone())
                   {
                       a.getTimer("scattertimer").resetAndStart();
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(0f, 0f), new Vector2(0f, 0f), (150 + 3500 / Math.Min(a.stats.getNumber("charge") + 1, 100)) / (a.potency + 1), 1 + a.potency, 1 + a.potency, 0, 1 + a.potency * 22, true, 300f, 100, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, ((float)random.NextDouble() * 200f)-100f), -5 - a.etheralDMG, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, false), false));
                       
                       a.stats.addTo("boost", -20 - (20 * a.potency));
                   
                   }
               },
               (a, i) =>
               {
                   a.stats.set("charge", 0);
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
                       SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10 - a.etheralDMG, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, true), true));

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
                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 70, 5 + a.potency, 5, 0, 0, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -125 - (a.potency * 50));
                   }
                   else if (a.stats.compare("charge", 25) > 0)
                   {

                       a.useActionSkill(SkillServer.GenerateSkillStruct(new Vector2(-40, 0f), new Vector2(0f, 0f), 100, 3 + a.potency, 3, 0, 15, true, 500f, 500, 0, 1f, null, "", 0, "",
                           SkillServer.GenerateProjectile(new Vector2(10f, 10f), new Vector2(500f, 0f), -10, 100, 800, ParticleServer.GenerateParticle(), "explosion", "attack1", 3, "all", Vector2.Zero, true), true));
                       a.stats.addTo("boost", -100 - (a.potency * 40));
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

                    if (a.stats.getNumber("hp") > a.stats.getNumber("MaxHP"))
                        a.stats.set("hp", a.stats.getNumber("MaxHP"));

                    a.factory.DisplayMessage("Devoured the Pizza!\nRestored 60 HP.");

                    //a.stats.inventory.removeItem(i);
                }));

            consumables.Add("Pizza Pie", new Consumable("Pizza Pie", "A flaky crust emitting a distinct pizza scent.\n\nClick to open.",
                (a, i) =>
                {

                    a.stats.inventory.receiveItem(getConsumable("Pizza"));

                    a.factory.DisplayMessage("You dig into the crust with your grubby mitts.\nThere was a Pizza inside!");
                }));

            consumables.Add("Canned Pizza", new Consumable("Canned Pizza", "This ancient elixir is renowned for its nourishment of body and spirit.\n\nHP+40",
                (a, i) =>
                {
                    a.stats.addTo("hp", 40);

                    if (a.stats.getNumber("hp") > a.stats.getNumber("MaxHP"))
                        a.stats.set("hp", a.stats.getNumber("MaxHP"));

                    //a.stats.inventory.removeItem(i);

                    a.factory.DisplayMessage("You drink the Pizza...Refreshing!\nRestored 40 HP.");
                }));

            consumables.Add("Pizza Rolls", new Consumable("Pizza Rolls", "Twisted remains of a deceased pizza.\n\nHP+20",
                (a, i) =>
                {
                    a.stats.addTo("hp", 20);

                    if (a.stats.getNumber("hp") > a.stats.getNumber("MaxHP"))
                        a.stats.set("hp", a.stats.getNumber("MaxHP"));

                    //a.stats.inventory.removeItem(i);

                    a.factory.DisplayMessage("You bite into the Pizza Rolls!\nRestored 20 HP.");
                }));

            #endregion

            #region Parts

            parts.Add("Basic Core",
                new Core("Basic Core",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, true)
                            },
                        new Dictionary<GearStat, int>() { })));

            parts.Add("KR101 Core",
                new Core("KR101 Core",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(0, 1) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true)
                            },
                        new Dictionary<GearStat, int>() { { GearStat.Potency, 1 } })));

            parts.Add("Blast Core",
                new Core("Blast Core",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(0, 1) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 1), 90, ConnectionType.Circle, true)
                            },
                        new Dictionary<GearStat, int>() { { GearStat.Potency, 2 } })));

            parts.Add("Dinky Chamber",
                new Part("Dinky Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() { 
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxHP, 1 } })));

            parts.Add("Pro-grade Chamber",
                new Part("Pro-grade Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Square, true)},
                        new Dictionary<GearStat, int>() { { GearStat.MaxHP, 2 } })));

            parts.Add("Kinetic Chamber",
                new Part("Kinetic Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Circle, true), 
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxHP, 1 } })));

            parts.Add("High Performance Chamber",
                new Part("High Performance Chamber",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Triangle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxHP, 3 } })));

            parts.Add("Dinky Belt",
                new Part("Dinky Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxEP, 1 } })));

            parts.Add("Pro-grade Belt",
                new Part("Pro-grade Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxEP, 2 } })));

            parts.Add("Kinetic Belt",
                new Part("Kinetic Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Triangle, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxEP, 1 } })));

            parts.Add("High Performance Belt",
                new Part("High Performance Belt",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Triangle, true) },
                        new Dictionary<GearStat, int>() { { GearStat.MaxEP, 3 } })));

            parts.Add("Dinky Carburetor",
                new Part("Dinky Carburetor",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.PhysicalDMG, 1 } })));

            parts.Add("Pro Carburetor A",
                new Part("Pro Carburetor A",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.PhysicalDMG, 2 } })));

            parts.Add("Pro Carburetor B",
                new Part("Pro Carburetor B",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.PhysicalDMG, 2 } })));

            parts.Add("High Performance Carburetor",
                new Part("High Performance Carburetor",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.PhysicalDMG, 3 } })));

            parts.Add("Kinetic Carburetor",
                new Part("Kinetic Carburetor",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.PhysicalDMG, 1 } })));

            parts.Add("Dinky Piston",
                new Part("Dinky Piston",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EtheralDMG, 1 } })));

            parts.Add("Pro Piston A",
                new Part("Pro Piston A",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EtheralDMG, 2 } })));

            parts.Add("Pro Piston B",
                new Part("Pro Piston B",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EtheralDMG, 2 } })));

            parts.Add("High Performance Piston",
                new Part("High Performance Piston",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(2, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EtheralDMG, 3 } })));

            parts.Add("Kinetic Piston",
                new Part("Kinetic Piston",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(1, 0), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 270, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EtheralDMG, 1 } })));

            parts.Add("Dinky Crankshaft A",
                new Part("Dinky Crankshaft A",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPConversion, 1 } })));

            parts.Add("Dinky Crankshaft B",
                new Part("Dinky Crankshaft B",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPConversion, 1 } })));

            parts.Add("Pro Crankshaft",
                new Part("Pro Crankshaft",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPConversion, 2 } })));

            parts.Add("Kinetic Crankshaft",
                new Part("Kinetic Crankshaft",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPConversion, 1 } })));

            parts.Add("Dinky Pump A",
                new Part("Dinky Pump A",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.HPConversion, 1 } })));

            parts.Add("Dinky Pump B",
                new Part("Dinky Pump B",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.HPConversion, 1 } })));

            parts.Add("Pro Pump",
                new Part("Pro Pump",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.HPConversion, 2 } })));

            parts.Add("Kinetic Pump",
                new Part("Kinetic Pump",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 0), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.HPConversion, 1 } })));

            parts.Add("Alpha Generator",
                new Part("Alpha Generator",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Circle, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Square, true),
                            new PartConnection(new Point(0, 1), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPRefill, 1 } })));

            parts.Add("Omega Generator",
                new Part("Omega Generator",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Square, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 1), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Triangle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.EPRefill, 2 } })));

            parts.Add("Fusion Drive",
                new Part("Fusion Drive",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Circle, true),
                            new PartConnection(new Point(0, 1), 90, ConnectionType.Circle, true),
                            new PartConnection(new Point(1, 0), 0, ConnectionType.Square, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.BoostSpeed, 1 } })));

            parts.Add("Hyper Drive",
                new Part("Hyper Drive",
                    new PartStruct(
                        new List<Point>() { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) },
                        new List<PartConnection>() {
                            new PartConnection(new Point(0, 0), 180, ConnectionType.Triangle, false),
                            new PartConnection(new Point(0, 0), 270, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 1), 90, ConnectionType.Square, true),
                            new PartConnection(new Point(1, 1), 0, ConnectionType.Circle, true)
                        },
                        new Dictionary<GearStat, int>() { { GearStat.BoostSpeed, 2 } })));

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

        public static Item getRareItem()
        {
            List<Item> tempList = new List<Item>();
            tempList.Add(parts["Fusion Drive"]);
            tempList.Add(parts["Hyper Drive"]);
            tempList.Add(parts["Omega Generator"]);
            tempList.Add(parts["Alpha Generator"]);

            return tempList[random.Next(tempList.Count)];
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
