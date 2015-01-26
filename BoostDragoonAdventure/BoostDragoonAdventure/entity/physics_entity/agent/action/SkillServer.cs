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
        public int duration;
        public List<KeyValuePair<int, SkillStruct>> blows;

        public SkillStruct(String name, Vector2 pos, Vector2 size, Vector2 center, int duration, List<KeyValuePair<int, SkillStruct>> blows)
        {
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.duration = duration;
            this.blows = blows;
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
                "ForwardAttack",
                new SkillStruct("Forward Attack",
                    new Microsoft.Xna.Framework.Vector2(1f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    0,
                    new List<KeyValuePair<int, SkillStruct>>()));


        }
    }
}
