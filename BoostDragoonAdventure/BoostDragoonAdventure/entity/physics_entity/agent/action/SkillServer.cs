using System;
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
        public Vector2 pos, size, center, velocity, parentVelocity; //relative to parent
        public int duration, force, directionChange;
        public List<KeyValuePair<int, SkillStruct>> blows;
        public List<KeyValuePair<String, int>> statIncrement;
        public String cue;
        public bool followParent;
        public Nullable<ParticleStruct> particle;
        public String spriterName;
        public int spriterEntityIndex;
        public String spriterAnimationName;

        public bool bounce;
        //public bool freeItemInUse;

        //public Nullable<SkillStruct> blowStruct;

        public SkillStruct(String name, Vector2 pos, Vector2 size, Vector2 center, Vector2 velocity, Vector2 parentVelocity, int duration, int force, int directionChange, List<KeyValuePair<int, SkillStruct>> blows, List<KeyValuePair<String, int>> statIncrement, String cue, bool followParent, Nullable<ParticleStruct> particle, String spriterName, int spriterEntityIndex, String spriterAnimationName, bool bounce)
        {
            this.name = name;
            this.pos = pos;
            this.size = size;
            this.center = center;
            this.velocity = velocity;
            this.parentVelocity = parentVelocity;
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

            this.bounce = bounce;
            //this.freeItemInUse = freeItemInUse;
            //this.blowStruct = null;
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
                    "attack1",
                    false));

            skills.Add(
                "Strong Attack",
                new SkillStruct("Strong Attack",
                    new Microsoft.Xna.Framework.Vector2(35f, 0f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(15f, 15f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Vertical Blow 1",
                new SkillStruct("Vertical Blow 1",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Vertical Blow 2",
                new SkillStruct("Vertical Blow 2",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Vertical Blow 3",
                new SkillStruct("Vertical Blow 3",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Vertical Blow 4",
                new SkillStruct("Vertical Blow 4",
                    new Microsoft.Xna.Framework.Vector2(40f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Vertical Blow 5",
                new SkillStruct("Vertical Blow 5",
                    new Microsoft.Xna.Framework.Vector2(50f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    Vector2.Zero,
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
                    "attack1",
                    false));

            skills.Add("Forward Attack",
                new SkillStruct("Forward Attack",
                    new Microsoft.Xna.Framework.Vector2(10f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(1f, 0f),
                    new Vector2(50f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

            skills.Add("Sword Attack",
                new SkillStruct("Sword Attack",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(30f, 30f),
                    new Microsoft.Xna.Framework.Vector2(0f, 15f),
                    Vector2.Zero,
                    new Vector2(40f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -15) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    1,
                    "sword",
                    false));

            skills.Add("Swordz Attack",
                new SkillStruct("Swordz Attack",
                    new Microsoft.Xna.Framework.Vector2(30f, 0f),
                    new Microsoft.Xna.Framework.Vector2(50f, 50f),
                    new Microsoft.Xna.Framework.Vector2(0f, 25f),
                    Vector2.Zero,
                    new Vector2(20f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -40) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    1,
                    "sword",
                    false));

            skills.Add("Bigger Sword Attack",
                new SkillStruct("Sword Attack",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(45f, 45f),
                    new Microsoft.Xna.Framework.Vector2(0f, 22.5f),
                    Vector2.Zero,
                    new Vector2(20f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -30) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    1,
                    "sword",
                    false));

            skills.Add("Biggest Sword Attack",
                new SkillStruct("Sword Attack",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(60f, 60f),
                    new Microsoft.Xna.Framework.Vector2(0f, 30f),
                    Vector2.Zero,
                    new Vector2(20f, 0f),
                    240,
                    300,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -50) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    1,
                    "sword",
                    false));

            skills.Add(
                "Spear Attack Weak",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
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
                    "",
                    false));

            skills.Add(
                "Spear Attack Full",
                new SkillStruct("Spear Attack Full",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
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
                    "",
                    false));

            skills.Add("Horizontal Blow 1",
                new SkillStruct("Horizontal Blow 1",
                    new Microsoft.Xna.Framework.Vector2(15f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(5f, 0f),
                    Vector2.Zero,
                    60,
                    50,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -5) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

            skills.Add("Horizontal Blow 2",
                new SkillStruct("Horizontal Blow 2",
                    new Microsoft.Xna.Framework.Vector2(17f, 10f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(8f, 0f),
                    Vector2.Zero,
                    60,
                    50,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -5) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

            skills.Add("Horizontal Blow 3",
                new SkillStruct("Horizontal Blow 3",
                    new Microsoft.Xna.Framework.Vector2(20f, 0f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(8f, 0f),
                    Vector2.Zero,
                    60,
                    50,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -5) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

            skills.Add("Horizontal Blow 4",
                new SkillStruct("Horizontal Blow 4",
                    new Microsoft.Xna.Framework.Vector2(17f, -10f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(8f, 0f),
                    Vector2.Zero,
                    60,
                    50,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -5) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

            skills.Add("Horizontal Blow 5",
                new SkillStruct("Horizontal Blow 5",
                    new Microsoft.Xna.Framework.Vector2(15f, -20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Microsoft.Xna.Framework.Vector2(5f, 5f),
                    new Vector2(5f, 0f),
                    Vector2.Zero,
                    60,
                    30,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -5) },
                    "whsh",
                    true,
                    null,
                    "weapons",
                    0,
                    "knife",
                    false));

                    
            skills.Add("Horizontal Extension Blow 1",
                new SkillStruct("Horizontal Extension Blow 1",
                    new Microsoft.Xna.Framework.Vector2(38f, 40f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(5f, 0f),
                    Vector2.Zero,
                    120,
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -8) },
                    "explosion",
                    true,
                    null, 
                    "all",
                    3,
                    "attack1",
                    false));

            skills.Add("Horizontal Extension Blow 2",
                new SkillStruct("Horizontal Extension Blow 2",
                    new Microsoft.Xna.Framework.Vector2(40f, 20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(6f, 0f),
                    Vector2.Zero,
                    120,
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -8) },
                    "explosion",
                    true,
                    null, 
                    "all",
                    3,
                    "attack1",
                    false));

            skills.Add("Horizontal Extension Blow 3",
                new SkillStruct("Horizontal Extension Blow 3",
                    new Microsoft.Xna.Framework.Vector2(42f, 0f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(6f, 0f),
                    Vector2.Zero,
                    120,
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -8) },
                    "explosion",
                    true,
                    null, 
                    "all",
                    3,
                    "attack1",
                    false));

            skills.Add("Horizontal Extension Blow 4",
                new SkillStruct("Horizontal Extension Blow 4",
                    new Microsoft.Xna.Framework.Vector2(40f, -20f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(6f, 0f),
                    Vector2.Zero,
                    120,
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -8) },
                    "explosion",
                    true,
                    null,
                    "all",
                    3,
                    "attack1",
                    false));

            skills.Add("Horizontal Extension Blow 5",
                new SkillStruct("Horizontal Extension Blow 5",
                    new Microsoft.Xna.Framework.Vector2(38f, -40f),
                    new Microsoft.Xna.Framework.Vector2(20f, 20f),
                    new Microsoft.Xna.Framework.Vector2(10f, 10f),
                    new Vector2(5f, 0f),
                    Vector2.Zero,
                    120,
                    100,
                    0,
                    new List<KeyValuePair<int, SkillStruct>>(),
                    new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -8) },
                    "explosion",
                    true,
                    null,
                    "all",
                    3,
                    "attack1",
                    false));

            skills.Add(
                "Longsword Attack Full",
                new SkillStruct("Longsword Attack Full",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    new Vector2(70f, 0f),
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
                    true,
                    null,
                    "",
                    0,
                    "",
                    false));

            skills.Add(
                "Longsword Attack Medium",
                new SkillStruct("Longsword Attack Medium",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    new Vector2(70f, 0f),
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
                    true,
                    null,
                    "",
                    0,
                    "",
                    false));

            skills.Add(
                "Longsword Attack Weak",
                new SkillStruct("Longsword Attack Weak",
                    new Microsoft.Xna.Framework.Vector2(5f, 0f),
                    new Microsoft.Xna.Framework.Vector2(2f, 2f),
                    new Microsoft.Xna.Framework.Vector2(1f, 1f),
                    Vector2.Zero,
                    new Vector2(70f, 0f),
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
                    true,
                    null,
                    "",
                    0,
                    "",
                    false));

        }

        public static SkillStruct GenerateSkillStruct(Vector2 parentVelocity, Vector2 velocity, int spreadDuration, int blowCount, int blowPerSpread, int scatterCount, int spread, bool followParent, float blowVelocity, int blowDuration, int blowReleaseDelay, Nullable<ParticleStruct> particle, bool bounce)
        {
            SkillStruct skill = new SkillStruct();

            skill.name = "Generated Skill";
            skill.pos = new Vector2(5f, 0f);
            skill.size = new Vector2(10f, 10f);
            skill.center = new Vector2(5f, 5f);
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();

            skill.statIncrement = new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10 * blowCount) };
            skill.velocity = velocity;
            skill.duration = spreadDuration * blowCount + blowReleaseDelay;
            skill.force = 50;
            skill.directionChange = 0;
            skill.cue = "";
            skill.followParent = followParent;
            skill.particle = particle;
            skill.spriterAnimationName = "attack1";
            skill.spriterEntityIndex = 3;
            skill.spriterName = "all";
            skill.parentVelocity = parentVelocity;

            skill.bounce = bounce;
            //skill.freeItemInUse = true;

            //if (skill.blowStruct.HasValue)
                //AddBlowsToSkillStruct(skill, blowCount, blowPerSpread, scatterCount, spreadDuration, spread, blowVelocity, spreadDuration * blowCount + blowReleaseDelay + blowDuration, blowReleaseDelay, 1f);
                
            
            
            return skill;
        }

        public static SkillStruct GenerateSkillStruct(Vector2 parentVelocity, Vector2 velocity, int spreadDuration, 
            int blowCount, int blowPerSpread, int scatterCount, int spread, bool followParent, float blowVelocity, 
            int blowDuration, int blowReleaseDelay, float releaseModifier, Nullable<ParticleStruct> particle,
            String spriterAnimationName, int spriterEntityIndex, String spriterName, SkillStruct blowStruct, bool bounce)
        {
            SkillStruct skill = new SkillStruct();

            skill.name = "Generated Skill";
            skill.pos = new Vector2(5f, 0f);
            skill.size = new Vector2(2f, 2f);
            skill.center = new Vector2(1f, 1f);
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();

            //skill.statIncrement = new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10 * blowCount) };
            skill.statIncrement = blowStruct.statIncrement;
            skill.velocity = velocity;
            skill.duration = spreadDuration * blowCount + blowReleaseDelay;
            skill.force = 50;
            skill.directionChange = 0;
            skill.cue = "";
            skill.followParent = followParent;
            skill.particle = particle;
            skill.spriterAnimationName = spriterAnimationName;
            skill.spriterEntityIndex = spriterEntityIndex;
            skill.spriterName = spriterName;
            skill.parentVelocity = parentVelocity;
            skill.bounce = bounce;

            AddBlowsToSkillStruct(skill, blowCount, blowPerSpread, scatterCount, spreadDuration, spread, blowReleaseDelay, releaseModifier, blowStruct);



            return skill;
        }

        public static SkillStruct GenerateProjectile(Vector2 size, Vector2 velocity, int hpIncrement, int force, int duration, Nullable<ParticleStruct> particle, String cue,
            String spriterAnimationName, int spriterEntityIndex, String spriterName, Vector2 parentVelocity, bool bounce)
        {
            SkillStruct skill = new SkillStruct();
            skill.name = "Generated Projectile";
            skill.pos = new Vector2(size.X / 2f, 0f);
            skill.size = size;
            skill.center = size / 2f;
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();
            skill.statIncrement = new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", hpIncrement) };
            skill.velocity = velocity;
            skill.duration = duration;
            skill.force = force;
            skill.directionChange = 0;
            skill.cue = cue;
            skill.followParent = false;
            skill.particle = particle;
            skill.spriterAnimationName = spriterAnimationName;
            skill.spriterEntityIndex = spriterEntityIndex;
            skill.spriterName = spriterName;
            skill.parentVelocity = parentVelocity;

            skill.bounce = bounce;

            return skill;
        }

        private static void AddBlowsToSkillStruct(SkillStruct skill, int blowCount, int blowPerSpread, int scatterCount, int spreadDuration, int spread, 
            int blowReleaseDelay, float releaseModifier, SkillStruct blowStruct)
        {
            SkillStruct temp;
            for (int i = 0; i < blowCount; i++)
            {
                
                
                   
                temp = GenerateSkillStructDirectionChange(spread, i, blowPerSpread, blowStruct);
                

                
                if (scatterCount > 0)
                {    
                    AddBlowsToSkillStruct(temp, blowCount, blowPerSpread, (scatterCount - 1), spreadDuration, spread, (int)(blowReleaseDelay * releaseModifier), releaseModifier, blowStruct);
                }

               
                
                skill.blows.Add(new KeyValuePair<int, SkillStruct>(i * spreadDuration + blowReleaseDelay, temp));
            }
        }

        private static SkillStruct GenerateSkillStructDirectionChange(int spread, int n, int m, SkillStruct blowStruct)
        {
            SkillStruct skill = new SkillStruct();

            skill.name = "Generated Skill DC";
            /*skill.pos = new Vector2(5f, 0f);
            skill.size = new Vector2(10f, 10f);
            skill.center = new Vector2(5f, 5f);
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();
            skill.statIncrement = new List<KeyValuePair<String, int>>() { new KeyValuePair<string, int>("hp", -10) } ;
            
            skill.velocity = new Vector2(velocity, 0f);
            skill.duration = duration;
            skill.force = 50;
            skill.directionChange = m != 1 ? -(spread / 2) + (n % m) * (spread / (m - 1)) : 0;
            skill.cue = "whsh";
            skill.followParent = false;
            skill.particle = new Nullable<ParticleStruct>(new ParticleStruct(Vector3.Zero, Vector3.Zero, new Vector3(-0.3f, -0.3f, -0.3f), new Vector3(0.6f, 0.6f, 0.6f), new Vector3(0f, -0.03f, 0f), 0f, 0f, 500, "particles", 0, "white_to_blue"));*/

            skill.pos = blowStruct.pos;
            skill.size = blowStruct.size;
            skill.center = blowStruct.center;
            skill.blows = new List<KeyValuePair<int, SkillStruct>>();
            skill.statIncrement = blowStruct.statIncrement;
            skill.velocity = blowStruct.velocity;
            skill.duration = blowStruct.duration;
            skill.force = blowStruct.force;

            skill.directionChange = m != 1 ? -(spread / 2) + (n % m) * (spread / (m - 1)) : 0;

            skill.cue = blowStruct.cue;
            skill.followParent = blowStruct.followParent;
            skill.particle = blowStruct.particle;

            skill.spriterAnimationName = blowStruct.spriterAnimationName;
            skill.spriterEntityIndex = blowStruct.spriterEntityIndex;
            skill.spriterName = blowStruct.spriterName;
            skill.parentVelocity = blowStruct.parentVelocity;
            skill.bounce = blowStruct.bounce;
            //skill.freeItemInUse = false;

            return skill;
        }
        

        private static void LoadSkillStruct(string path)
        {

        }
    }
}
