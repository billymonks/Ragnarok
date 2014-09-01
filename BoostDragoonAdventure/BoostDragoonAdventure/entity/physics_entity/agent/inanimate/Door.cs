using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.factory.entity;
using wickedcrush.manager.audio;

namespace wickedcrush.entity.physics_entity.agent.inanimate
{
    public class Door : Agent
    {
        public Door(World w, Vector2 pos, Direction facing, EntityFactory factory, SoundManager sound) 
            : base(w, pos, new Vector2(80f, 80f), new Vector2(40f, 40f), false, factory, sound)
        {
            this.facing = facing;
        }
    }
}
