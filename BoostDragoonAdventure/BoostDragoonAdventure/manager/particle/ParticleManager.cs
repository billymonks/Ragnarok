using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wickedcrush.particle;
using Microsoft.Xna.Framework;
using wickedcrush.display._3d;
using wickedcrush.manager.gameplay;

namespace wickedcrush.manager.particle
{
    public class ParticleManager : GameComponent
    {
        List<Particle> particleList = new List<Particle>();
        List<Particle> addList = new List<Particle>();
        List<Particle> removeList = new List<Particle>();

        public Stack<Particle> particlePool;
        private static int MAX_POOL_CAPACITY = 256;
        public bool usingPool = true;

        public ParticleManager(GameBase game)
            : base(game)
        {
            particlePool = new Stack<Particle>(MAX_POOL_CAPACITY);
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {

            updateParticles(gameTime);


            base.Update(gameTime);
        }

        public void Draw(bool depthPass, Dictionary<string, PointLightStruct> lightList, GameplayManager gameplay)
        {
            foreach (Particle p in particleList)
            {
                p.Draw(depthPass);
            }
        }

        public void AddParticle(Particle p)
        {
            //addList.Add(p);
            particleList.Add(p);
        }

        public void RemoveAll()
        {
            foreach (Particle p in particleList)
            {
                p.Remove();
                removeList.Add(p);
            }
        }

        private void updateParticles(GameTime gameTime)
        {
            foreach (Particle e in particleList)
            {
                if (e.readyForRemoval)
                    removeList.Add(e);
                else
                    e.Update(gameTime);
            }

            performRemoval();
            performAdd();
        }

        private void performAdd()
        {
            if (addList.Count > 0)
            {
                foreach (Particle e in addList)
                {
                    particleList.Add(e);
                }

                addList.Clear();
            }
        }

        private void performRemoval()
        {
            if (removeList.Count > 0)
            {
                foreach (Particle e in removeList)
                {
                    if (usingPool && particlePool.Count < MAX_POOL_CAPACITY)
                    {
                        particlePool.Push(e);
                    }

                    e.Remove();
                    particleList.Remove(e);
                }

                removeList.Clear();
            }
        }
    }
}
