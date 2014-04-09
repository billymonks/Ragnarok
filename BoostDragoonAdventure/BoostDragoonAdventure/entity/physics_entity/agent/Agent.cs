using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using wickedcrush.map.path;

namespace wickedcrush.entity.physics_entity.agent
{
    public class Agent : PhysicsEntity
    {
        Navigator navigator;
        public Agent(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
            : base(w, pos, size, center, solid)
        {
            Initialize(w, pos, size, center, solid);
        }

        private void Initialize(World w, Vector2 pos, Vector2 size, Vector2 center, bool solid)
        {
            this.name = "Agent";
        }
    }
}
