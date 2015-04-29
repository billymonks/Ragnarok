﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.utility;
using Microsoft.Xna.Framework;
using wickedcrush.particle;

namespace wickedcrush.entity.physics_entity.agent.action
{
    public struct SkillStruct
    {
        public String name;
        public Vector2 pos, size, center, velocity; //relative to parent
        public int duration, force, directionChange;
        public List<KeyValuePair<int, SkillStruct>> blows;
        public List<KeyValuePair<String, int>> statIncrement;
        public String cue;
        public bool followParent;
        public Nullable<ParticleStruct> particle;
        public String spriterName;
        public int spriterEntityIndex;
        public String spriterAnimationName;

        public SkillStruct(String name, Vector2 pos, Vector2 size, Vector2 center, Vector2 velocity, int duration, int force, int directionChange, List<KeyValuePair<int, SkillStruct>> blows, List<KeyValuePair<String, int>> statIncrement, String cue, bool followParent, Nullable<ParticleStruct> particle, String spriterName, int spriterEntityIndex, String spriterAnimationName)
        {
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.velocity = velocity;
            this.duration = duration;
            this.force = force;
            this.directionChange = directionChange;
            this.blows = blows;
            this.statIncrement = statIncrement;
            this.cue = cue;
            this.followParent = followParent;
            this.particle = particle;
            this.spriterName = spriterName;
            this.spriterEntityIndex = spriterEntityIndex;
            this.spriterAnimationName = spriterAnimationName;
        }
    }
    public static class SkillServer
    {
        public static Dictionary<String, SkillStruct> skills = new Dictionary<String, SkillStruct>();
        public static Random random = new Random();

        static List<ParticleStruct> temp = new List<ParticleStruct>();
        
        //temp.Add(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, 2f, -0.3f), new Vector3(0.6f, 1f, 0.6f), new Vector3(0f, -0.3f, 0f), 0f, 0f, 400, "particles", 0, "white_to_yellow"));

        public static void Initialize()
        {
            skills = new Dictionary<String, SkillStruct>();

            skills.Add(
                "Weak Attack",
                new SkillStruct("Weak Attack",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    160,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add(
                "Strong Attack",
                new SkillStruct("Strong Attack",
                    new Microsoft.Xna.Framework.Vector2(35f, 0f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -20) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Vertical Blow 1",
                new SkillStruct("Vertical Blow 1",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Vertical Blow 2",
                new SkillStruct("Vertical Blow 2",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Vertical Blow 3",
                new SkillStruct("Vertical Blow 3",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Vertical Blow 4",
                new SkillStruct("Vertical Blow 4",
                    new Microsoft.Xna.Framework.Vector2(40f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Vertical Blow 5",
                new SkillStruct("Vertical Blow 5",
                    new Microsoft.Xna.Framework.Vector2(50f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
                    120,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add(
                "Spear Attack Weak",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    90,
                    0,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Vertical Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Vertical Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Vertical Blow 3"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "",
                    false,
                    null,
                    "",
                    0,
                    ""));

            skills.Add(
                "Spear Attack Full",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    130,
                    0,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Vertical Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Vertical Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Vertical Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(90, skills["Vertical Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(120, skills["Vertical Blow 5"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>(),
                    "whsh",
                    false,
                    null,
                    "",
                    0,
                    ""));

            skills.Add("Horizontal Blow 1",
                new SkillStruct("Horizontal Blow 1",
                    new Microsoft.Xna.Framework.Vector2(15f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(35f, 0f),
                    60,
                    500,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Blow 2",
                new SkillStruct("Horizontal Blow 2",
                    new Microsoft.Xna.Framework.Vector2(17f, 10f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(38f, 0f),
                    60,
                    500,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Blow 3",
                new SkillStruct("Horizontal Blow 3",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(41f, 0f),
                    60,
                    500,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Blow 4",
                new SkillStruct("Horizontal Blow 4",
                    new Microsoft.Xna.Framework.Vector2(17f, -10f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(38f, 0f),
                    60,
                    500,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Blow 5",
                new SkillStruct("Horizontal Blow 5",
                    new Microsoft.Xna.Framework.Vector2(15f, -20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(35f, 0f),
                    60,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

                    
            skills.Add("Horizontal Extension Blow 1",
                new SkillStruct("Horizontal Extension Blow 1",
                    new Microsoft.Xna.Framework.Vector2(18f, 40f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(50f, 0f),
                    120,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "explosion",
                    false,
                    null, 
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Extension Blow 2",
                new SkillStruct("Horizontal Extension Blow 2",
                    new Microsoft.Xna.Framework.Vector2(40f, 20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(60f, 0f),
                    120,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "explosion",
                    false,
                    null, 
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Extension Blow 3",
                new SkillStruct("Horizontal Extension Blow 3",
                    new Microsoft.Xna.Framework.Vector2(42f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(65f, 0f),
                    120,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "explosion",
                    false,
                    null, 
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Extension Blow 4",
                new SkillStruct("Horizontal Extension Blow 4",
                    new Microsoft.Xna.Framework.Vector2(40f, -20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(60f, 0f),
                    120,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "explosion",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add("Horizontal Extension Blow 5",
                new SkillStruct("Horizontal Extension Blow 5",
                    new Microsoft.Xna.Framework.Vector2(18f, -40f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(50f, 0f),
                    120,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "explosion",
                    false,
                    null,
                    "all",
                    3,
                    "attack1"));

            skills.Add(
                "Longsword Attack Full",
                new SkillStruct("Longsword Attack Full",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    250,
                    1000,
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
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -18) },
                    "ping",
                    false,
                    null,
                    "",
                    0,
                    ""));

            skills.Add(
                "Longsword Attack Medium",
                new SkillStruct("Longsword Attack Medium",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    130,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Horizontal Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Horizontal Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Horizontal Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(90, skills["Horizontal Blow 4"] ),
                        new KeyValuePair<int, SkillStruct>(120, skills["Horizontal Blow 5"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "",
                    false,
                    null,
                    "",
                    0,
                    ""));

            skills.Add(
                "Longsword Attack Weak",
                new SkillStruct("Longsword Attack Weak",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    70,
                    1000,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Horizontal Blow 2"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Horizontal Blow 3"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Horizontal Blow 4"] )
                    
                    
                    },
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) },
                    "",
                    false,
                    null,
                    "",
                    0,
                    ""));

        }

        public static SkillStruct GenerateSkillStruct(int durationModifier, int velocity, int c, int d, int e, int f, int spread)
        {
            SkillStruct skill = new SkillStruct();

            skill.name = "Generated Skill";
            skill.pos = new Vector2(5f, 0f);
            skill.size = new Vector2(10f, 10f);
            skill.center = new Vector2(5f, 5f);
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();
            
            skill.statIncrement = new List<KeyValuePair<String, int>>();
            skill.velocity = new Vector2(velocity * 100f, 0f);
            skill.duration = 300 + durationModifier * 100;
            skill.force = 50;
            skill.directionChange = 0;
            skill.cue = "";
            skill.followParent = true;
            skill.particle = null;
            skill.spriterAnimationName = "";
            skill.spriterEntityIndex = 0;
            skill.spriterName = "";
            
            AddBlowsToSkillStruct(skill, d, e, f, c, spread);
                
            
            
            return skill;
        }

        private static void AddBlowsToSkillStruct(SkillStruct skill, int a, int b, int c, int blowsSpreadDuration, int spread)
        {
            SkillStruct temp;
            for (int i = 0; i < a; i++)
            {
                if (a % 2 == 0)
                {
                    temp = GenerateSkillStructDirectionChange(spread, i, b);
                }
                else
                {
                    temp = GenerateSkillStructDirectionChange(spread, i, b);
                }

                if(c>0)
                    AddBlowsToSkillStruct(temp, a + 1, b, c - 1, blowsSpreadDuration, spread);


                skill.blows.Add(new KeyValuePair<int, SkillStruct>(100 + (i + 1) * blowsSpreadDuration, temp));
            }
        }

        private static SkillStruct GenerateSkillStructDirectionChange(int spread, int n, int m)
        {
            SkillStruct skill = new SkillStruct();

            skill.name = "Generated Skill DC";
            skill.pos = new Vector2(5f, 0f);
            skill.size = new Vector2(10f, 10f);
            skill.center = new Vector2(5f, 5f);
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();
            skill.statIncrement = new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) } ;
            
            skill.velocity = new Vector2(185f + 15 * 10f, 0f);
            skill.duration = 700;
            skill.force = 50;
            skill.directionChange = m != 1 ? -(spread / 2) + (n % m) * (spread / (m - 1)) : 0;
            skill.cue = "whsh";
            skill.followParent = false;
            skill.particle = new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 500, "particles", 0, "white_to_blue"));
            skill.spriterAnimationName = "";
            skill.spriterEntityIndex = 0;
            skill.spriterName = "";

            return skill;
        }

        private static void LoadSkillStruct(string path)
        {

        }
    }
}
