using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.utility;
using Microsoft.Xna.Framework;

namespace wickedcrush.entity.physics_entity.agent.action
{
    public struct SkillStruct
    {
        public String name;
        public Vector2 pos, size, center, velocity; //relative to parent
        public int duration, force;
        public List<KeyValuePair<int, SkillStruct>> blows;
        public List<KeyValuePair<String, int>> statIncrement;
        public String cue;

        public SkillStruct(String name, Vector2 pos, Vector2 size, Vector2 center, Vector2 velocity, int duration, int force, List<KeyValuePair<int, SkillStruct>> blows, List<KeyValuePair<String, int>> statIncrement, String cue)
        {
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.velocity = velocity;
            this.duration = duration;
            this.force = force;
            this.blows = blows;
            this.statIncrement = statIncrement;
            this.cue = cue;
        }
    }
    public static class SkillServer
    {
        public static Dictionary<String, SkillStruct> skills = new Dictionary<String, SkillStruct>();
        public static Random random = new Random();

        public static void Initialize()
        {
            skills = new Dictionary<String, SkillStruct>();

            skills.Add(
                "Weak Attack",
                new SkillStruct("Weak Attack",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    160,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) },
                    ""));

            skills.Add(
                "Strong Attack",
                new SkillStruct("Strong Attack",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(25f, 25f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -20) },
                    ""));

            skills.Add("Vertical Blow 1",
                new SkillStruct("Vertical Blow 1",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Vertical Blow 2",
                new SkillStruct("Vertical Blow 2",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Vertical Blow 3",
                new SkillStruct("Vertical Blow 3",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Vertical Blow 4",
                new SkillStruct("Vertical Blow 4",
                    new Microsoft.Xna.Framework.Vector2(40f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Vertical Blow 5",
                new SkillStruct("Vertical Blow 5",
                    new Microsoft.Xna.Framework.Vector2(50f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add(
                "Spear Attack Weak",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    90,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Vertical Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Vertical Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Vertical Blow 3"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    ""));

            skills.Add(
                "Spear Attack Full",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    130,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Vertical Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Vertical Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Vertical Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(90, skills["Vertical Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(120, skills["Vertical Blow 5"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "whsh"));

            skills.Add("Horizontal Blow 1",
                new SkillStruct("Horizontal Blow 1",
                    new Microsoft.Xna.Framework.Vector2(12f, -20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(35f, 0f),
                    120,
                    500,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Horizontal Blow 2",
                new SkillStruct("Horizontal Blow 2",
                    new Microsoft.Xna.Framework.Vector2(13f, -10f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(38f, 0f),
                    120,
                    500,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Horizontal Blow 3",
                new SkillStruct("Horizontal Blow 3",
                    new Microsoft.Xna.Framework.Vector2(15f, 0f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(41f, 0f),
                    120,
                    500,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Horizontal Blow 4",
                new SkillStruct("Horizontal Blow 4",
                    new Microsoft.Xna.Framework.Vector2(13f, 10f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(38f, 0f),
                    120,
                    500,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Horizontal Blow 5",
                new SkillStruct("Horizontal Blow 5",
                    new Microsoft.Xna.Framework.Vector2(12f, 20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(35f, 0f),
                    120,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh"));

            skills.Add("Horizontal Extension Blow 1",
                new SkillStruct("Horizontal Extension Blow 1",
                    new Microsoft.Xna.Framework.Vector2(18f, -40f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(50f, 0f),
                    120,
                    700,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "explosion"));

            skills.Add("Horizontal Extension Blow 2",
                new SkillStruct("Horizontal Extension Blow 2",
                    new Microsoft.Xna.Framework.Vector2(20f, -20f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(60f, 0f),
                    120,
                    700,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "explosion"));

            skills.Add("Horizontal Extension Blow 3",
                new SkillStruct("Horizontal Extension Blow 3",
                    new Microsoft.Xna.Framework.Vector2(25f, 0f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(65f, 0f),
                    120,
                    700,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "explosion"));

            skills.Add("Horizontal Extension Blow 4",
                new SkillStruct("Horizontal Extension Blow 4",
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(60f, 0f),
                    120,
                    700,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "explosion"));

            skills.Add("Horizontal Extension Blow 5",
                new SkillStruct("Horizontal Extension Blow 5",
                    new Microsoft.Xna.Framework.Vector2(18f, 40f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    new Vector2(50f, 0f),
                    120,
                    700,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "explosion"));

            skills.Add(
                "Longsword Attack Full",
                new SkillStruct("Longsword Attack Full",
                    new Microsoft.Xna.Framework.Vector2(25f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    250,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Horizontal Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Horizontal Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Horizontal Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(90, skills["Horizontal Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(120, skills["Horizontal Blow 5"] ),
                        new KeyValuePair<int, SkillStruct>(125, skills["Horizontal Extension Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(155, skills["Horizontal Extension Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(185, skills["Horizontal Extension Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(215, skills["Horizontal Extension Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(245, skills["Horizontal Extension Blow 5"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "ping"));

            skills.Add(
                "Longsword Attack Medium",
                new SkillStruct("Longsword Attack Medium",
                    new Microsoft.Xna.Framework.Vector2(25f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    130,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Horizontal Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Horizontal Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Horizontal Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(90, skills["Horizontal Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(120, skills["Horizontal Blow 5"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "ping"));

            skills.Add(
                "Longsword Attack Weak",
                new SkillStruct("Longsword Attack Weak",
                    new Microsoft.Xna.Framework.Vector2(25f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    70,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Horizontal Blow 2"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Horizontal Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Horizontal Blow 4"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "ping"));

        }

        private static void LoadSkillStruct(string path)
        {

        }
    }
}
