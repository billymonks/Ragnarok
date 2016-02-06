using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using wickedcrush.manager.audio;
using wickedcrush.factory.entity;
using wickedcrush.utility;

namespace wickedcrush.entity.physics_entity.agent.attack.cluster
{
    public class FireCluster : Agent
    {
        int clusterCount;

        public FireCluster(World w, Vector2 pos, Vector2 size, Entity parent, int direction, int damage, int force, int clusterCount, SoundManager sound, EntityFactory factory)
            : base(-1, w, pos, size, size / 2f, false, factory, sound)
        {
            this.clusterCount = clusterCount;

            timers.Add("release", new Timer(100));
            timers.Add("duration", new Timer(200));

            timers["release"].resetAndStart();
            timers["duration"].resetAndStart();
        }

        public void Update()
        {
            if (timers["release"].isDone())
            {
                this.Remove();
            }

            if (timers["duration"].isDone())
            {
                if (clusterCount > 0)
                {
                    //add next 
                }
            }
        }
    }
}
