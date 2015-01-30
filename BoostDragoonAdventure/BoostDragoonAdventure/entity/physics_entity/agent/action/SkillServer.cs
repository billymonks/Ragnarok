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
        public Vector2 pos; //relative to parent
        public Vector2 size;
        public Vector2 center;
        public int duration, force;
        public List<KeyValuePair<int, SkillStruct>> blows;
        public List<KeyValuePair<String, int>> statIncrement;

        public SkillStruct(String name, Vector2 pos, Vector2 size, Vector2 center, int duration, int force, List<KeyValuePair<int, SkillStruct>> blows, List<KeyValuePair<String, int>> statIncrement)
        {
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.duration = duration;
            this.force = force;
            this.blows = blows;
            this.statIncrement = statIncrement;
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
                    0,
                    160,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) }));

            skills.Add(
                "Strong Attack",
                new SkillStruct("Strong Attack",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(25f, 25f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    0,
                    300,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -20) }));

            skills.Add("Chain Blow 1",
                new SkillStruct("Chain Blow 1",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    0,
                    100,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) }));

            skills.Add("Chain Blow 2",
                new SkillStruct("Chain Blow 2",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    0,
                    100,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) }));

            skills.Add("Chain Blow 3",
                new SkillStruct("Chain Blow 3",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    0,
                    100,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) }));

            skills.Add(
                "Spear Attack",
                new SkillStruct("Spear Attack",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>() { 
                        new KeyValuePair<int, SkillStruct>(0, skills["Chain Blow 1"] ), 
                        new KeyValuePair<int, SkillStruct>(30, skills["Chain Blow 2"] ),
                        new KeyValuePair<int, SkillStruct>(60, skills["Chain Blow 3"] )},
                    new List<KeyValuePair<String, int>>()));



        }
    }
}
